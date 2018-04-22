using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public enum GameState
{
    PREPARATION_PHASE,
    DAMAGE_PHASE,
    KO_PHASE,
    WIN_PHASE
}

public class Game : MonoBehaviour {

    public static Game Instance;

    public HealthBarController playerHealthController;
    public HealthBarController enemyHealthController;

    public CombatTextController playerCombatTextController;
    public CombatTextController enemyCombatTextController;

    public HealthTextController playerHealthTextController;
    public HealthTextController enemyHealthTextController;

    public EventSystem eventSystem;

    public Transform handReference;

    public List<Card> availableInitCards;
    public List<Card> availableMiddleCards;
    public List<Card> availableFinisherCards;

    public CanvasGroup guardConfirmation;
    public CanvasGroup invalidPlay;
    public CanvasGroup finalState;

    public int initProportions = 36;
    public int finisherProportions = 9;

    public int deckSize = 100;

    public EventTrigger readyButton;
    public EventTrigger guardButton;

    public Text guardConfirmationText;
    public Text invalidPlayText;

    public Transform deckReference;

    private GameState currentState;

    private Fighter player;
    private Fighter enemy;
    private PCFighter pcFighter;

    private int playerDamage;
    private int enemyDamage;

    private Stack<Card> playerDeck;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }

    //DELETE
    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        finalState.gameObject.SetActive(false);
        ToggleCardsAndActions(true);

        playerDamage = 0;
        enemyDamage = 0;

        // prepare player and enemy (life, cards, etc)
        player = new Fighter();
        enemy = new Fighter();
        pcFighter = new PCFighter(enemy);

        playerHealthController.Init();
        enemyHealthController.Init();

        // change game state to preparation
        playerDeck = GenerateRandomDeck();
        Stack<Card> enemyDeck = GenerateRandomDeck();

        player.Init(Shuffle(playerDeck), "Player");
        enemy.Init(Shuffle(enemyDeck), "Computer");

        playerHealthTextController.ShowText(player.Health.ToString());
        enemyHealthTextController.ShowText(enemy.Health.ToString());

        DisplayPlayerHand(player.Hand);

        StartCoroutine(AnimateNewCards(player.Hand));

        DisplayDeck(playerDeck);

        ChangeState(GameState.PREPARATION_PHASE);
    }

    public void ReadyButton()
    {
        List<CardMap> selectedCards = GetSelectedCards();

        if (selectedCards.Count > 0)
        {
            if (player.CheckCards(selectedCards))
            {
                playerDamage = player.PlayCards(selectedCards);

                RemoveFromDisplayDeck(selectedCards.Count);

                HideNewCards(selectedCards.ToArray());

                DisplayPlayerHand(player.Hand);

                StartCoroutine(AnimateNewCards(selectedCards.ToArray()));

                enemyDamage = pcFighter.Play();

                ChangeState(GameState.DAMAGE_PHASE);
            }
            else
            {
                StartCoroutine(ShowInvalidPlayDialog("Wrong card combination."));
                Debug.Log("[ERROR] Wrong card combination.");
            }
        }
        else
        {
            StartCoroutine(ShowInvalidPlayDialog("No cards Selected."));
            Debug.Log("[ERROR] No cards Selected.");
        }
    }

    public void DoGuard()
    {
        List<CardMap> selectedCards = GetSelectedCards();

        Debug.Log("[GUARD] Player - GUARD YES");
        //With Guard button you will use your selected cards as defense, so you will have some defense points and you wont attack this turn. The more you discard, the less you defend.
        ToggleCardsAndActions(true);
               
        player.Guard(selectedCards);

        RemoveFromDisplayDeck(selectedCards.Count);

        HideNewCards(selectedCards.ToArray());

        DisplayPlayerHand(player.Hand);

        StartCoroutine(AnimateNewCards(selectedCards.ToArray()));

        enemyDamage = pcFighter.Play();

        DismissGuard();

        ChangeState(GameState.DAMAGE_PHASE);
    }

    public void GuardButton()
    {
        ToggleCardsAndActions(false);

        List<CardMap> selectedCards = GetSelectedCards();

        if (selectedCards.Count > 0)
        {
            guardConfirmationText.text = (1f / (2 + selectedCards.Count) * 100).ToString("n2") + "% defense discarding " + selectedCards.Count + " cards. Continue?";
        }
        else
        {
            guardConfirmationText.text = "50% defense discarding 0 cards. Continue?";
        }

        guardConfirmation.gameObject.SetActive(true);
        guardConfirmation.alpha = 1; //TODO: animation? if not, remove.

    }

    public void DismissGuard()
    {
        ToggleCardsAndActions(true);

        guardConfirmation.gameObject.SetActive(false);
        guardConfirmation.alpha = 0;
    }

    private void HideNewCards(CardMap[] selectedCards)
    {
        for (int i = 0; i < selectedCards.Length; ++i)
        {
            Transform newCard = handReference.GetChild(selectedCards[i].index);
            newCard.GetComponent<CardDisplayer>().FlipCard(true);
        }
    }

    private IEnumerator AnimateNewCards(CardMap[] selectedCards)
    {
        ToggleCardsAndActions(false);

        for (int i = 0; i < selectedCards.Length; ++i)
        {
            Transform newCard = handReference.GetChild(selectedCards[i].index);

            newCard.DOScale(1.2f, .2f);

            yield return new WaitForSeconds(0.2f);

            newCard.GetComponent<CardDisplayer>().FlipCard(false);

            newCard.DOScale(1f, .2f);
        }

        ToggleCardsAndActions(true);
    }

    private void ChangeState(GameState state)
    {
        //Entry point for state.
        switch (state)
        {
            case GameState.PREPARATION_PHASE:
                eventSystem.enabled = true;
                Debug.Log("[GAMESTATE] Preparation phase");
                break;
            case GameState.DAMAGE_PHASE:
                eventSystem.enabled = false;
                Debug.Log("[GAMESTATE] Damage phase");

                //Fancy animations and apply damage to both players and also health bars
                Debug.Log("[DAMAGE] Player did " + playerDamage + " to Computer");
                enemyHealthController.PrintDamage(enemy.ApplyDamage(playerDamage));
                enemyHealthTextController.ShowText(enemy.Health.ToString());
                string enemyText = playerDamage + "";
                if (enemy.DefensePoints != 1)
                    enemyText += "\nBLOCKED (" + (enemy.DefensePoints * 100).ToString("n2") + "%)";
                enemyCombatTextController.ShowText(enemyText);
                state = enemy.CheckFighterState();

                if (state == GameState.KO_PHASE || state == GameState.WIN_PHASE) { ChangeState(state); break; }

                Debug.Log("[DAMAGE] Computer did " + enemyDamage + " to Player");
                playerHealthController.PrintDamage(player.ApplyDamage(enemyDamage));
                playerHealthTextController.ShowText(player.Health.ToString());
                string playerText = enemyDamage + "";
                if (player.DefensePoints != 1)
                    playerText += "\nBLOCKED (" + (player.DefensePoints * 100).ToString("n2") + "%)";
                playerCombatTextController.ShowText(playerText);
                state = player.CheckFighterState();

                if (state == GameState.KO_PHASE || state == GameState.WIN_PHASE) { ChangeState(state); break; }

                playerDamage = 0;
                enemyDamage = 0;

                //Check if someone dies or cant play and move to KO_PHASE if not, Preparation again.

                ChangeState(state);

                break;
            case GameState.KO_PHASE:
                eventSystem.enabled = true;
                ToggleCardsAndActions(false);
                Debug.Log("[GAMESTATE] KO phase");
                GameOver();
                break;
            case GameState.WIN_PHASE:
                eventSystem.enabled = true;
                ToggleCardsAndActions(false);
                Debug.Log("[GAMESTATE] WIN phase");
                PlayerWins();
                break;
        }

        currentState = state;
    }

    private void GameOver()
    {
        finalState.GetComponentInChildren<Text>(true).text = "Game Over";
        finalState.gameObject.SetActive(true);
    }

    private void PlayerWins()
    {
        finalState.GetComponentInChildren<Text>(true).text = "You WIN";
        finalState.gameObject.SetActive(true);
    }

    private void DisplayPlayerHand(CardMap[] hand)
    {
        CleanHand();

        for (int i = 0; i < hand.Length; ++i)
        {
            GameObject card = ObjectPooler.Instance.GetPooledObject("Card");

            card.transform.SetParent(handReference, false);
            card.GetComponent<CardDisplayer>().InitCard(hand[i]);
            card.SetActive(true);
        }
    }

    private IEnumerator ShowInvalidPlayDialog(string text)
    {
        invalidPlayText.text = text;
        invalidPlay.gameObject.SetActive(true);
        invalidPlay.alpha = 1; //TODO: animation? if not, remove.
        yield return new WaitForSeconds(1.5f);
        invalidPlay.alpha = 0;
    }

    private Stack<Card> GenerateRandomDeck()
    {
        Stack<Card> deck = new Stack<Card>();

        int totalInitCards = initProportions * deckSize / 100;
        for (int i = 0; i < totalInitCards; ++i)
        {
            int randomCard = UnityEngine.Random.Range(0, availableInitCards.Count);
            deck.Push(availableInitCards[randomCard]);
        }

        int totalFinisherstCards = finisherProportions * deckSize / 100;
        for (int i = 0; i < totalFinisherstCards; ++i)
        {
            int randomCard = UnityEngine.Random.Range(0, availableFinisherCards.Count);
            deck.Push(availableFinisherCards[randomCard]);
        }

        int totalMiddleCards = deckSize - (totalInitCards + totalFinisherstCards);
        for (int i = 0; i < totalMiddleCards; ++i)
        {
            int randomCard = UnityEngine.Random.Range(0, availableMiddleCards.Count);
            deck.Push(availableMiddleCards[randomCard]);
        }

        return deck;
    }

    private List<CardMap> GetSelectedCards()
    {
        List<CardMap> returnList = new List<CardMap>();
        CardDisplayer[] cards = handReference.GetComponentsInChildren<CardDisplayer>();
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].selected) returnList.Add(cards[i].cardMap);
        }
        return returnList;
    }

    private Stack<Card> Shuffle(Stack<Card> stack)
    {
        System.Random rnd = new System.Random();
        return new Stack<Card>(stack.OrderBy(x => rnd.Next()));
    }

    private void CleanHand()
    {
        CardDisplayer[] cards = handReference.GetComponentsInChildren<CardDisplayer>();
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].selected = false;
            cards[i].gameObject.SetActive(false);
        }
    }

    public void ToggleCardsAndActions(bool toggle)
    {
        EventTrigger[] cards = handReference.GetComponentsInChildren<EventTrigger>();
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].enabled = toggle;
        }

        readyButton.enabled = toggle;
        guardButton.enabled = toggle;
    }

    private void DisplayDeck(Stack<Card> deck)
    {
        for (int i = 0; i < deck.Count; ++i)
        {
            GameObject cardBack = ObjectPooler.Instance.GetPooledObject("DummyCard");
            cardBack.transform.SetParent(deckReference, false);
            Vector3 save = cardBack.transform.position;
            Vector3 offset = new Vector3(save.x + 0.25f * i, save.y + 0.25f * i, save.z);
            cardBack.transform.position = offset;

            cardBack.SetActive(true);
        }
        
    }

    private void RemoveFromDisplayDeck(int amount)
    {
        for (int i = 0; i < amount; ++i)
        {
            Transform lastDeckCard = deckReference.GetChild(deckReference.childCount - i - 1); // -1 because there ir a transparent card
            lastDeckCard.DOLocalMoveY(-500f, .5f).OnComplete(() =>
            {
                Destroy(lastDeckCard.gameObject);
            });
        }
    }
}
