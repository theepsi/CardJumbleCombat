using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum GameState
{
    PREPARATION_PHASE,
    DAMAGE_PHASE,
    KO_PHASE
}

public class Game : MonoBehaviour {

    public static Game Instance;

    public HealthBarController playerHealthController;
    public HealthBarController enemyHealthController;

    public EventSystem eventSystem;

    public Transform handReference;

    public List<Card> availableInitCards;
    public List<Card> availableMiddleCards;
    public List<Card> availableFinisherCards;

    public CanvasGroup guardConfirmation;

    public int initProportions = 41;
    public int finisherProportions = 9;

    public int deckSize = 100;

    public EventTrigger readyButton;
    public EventTrigger guardButton;

    public Text guardConfirmationText;

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
        playerDamage = 0;
        enemyDamage = 0;

        // prepare player and enemy (life, cards, etc)
        player = new Fighter();
        enemy = new Fighter();
        pcFighter = new PCFighter(enemy);

        // change game state to preparation
        playerDeck = GenerateRandomDeck();
        Stack<Card> enemyDeck = GenerateRandomDeck();

        player.Init(Shuffle(playerDeck));
        enemy.Init(Shuffle(enemyDeck));

        DisplayPlayerHand(player.Hand);

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

                DisplayPlayerHand(player.Hand);

                enemyDamage = pcFighter.Play();

                ChangeState(GameState.DAMAGE_PHASE);
            }
            else
            {
                Debug.Log("[ERROR] Wrong card combination.");
            }
        }
        else
        {
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

        DisplayPlayerHand(player.Hand);

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
                state = enemy.CheckFighterState();

                if (state == GameState.KO_PHASE) break;

                Debug.Log("[DAMAGE] Computer did " + enemyDamage + " to Player");
                playerHealthController.PrintDamage(player.ApplyDamage(enemyDamage));
                state = player.CheckFighterState();

                if (state == GameState.KO_PHASE) break;

                playerDamage = 0;
                enemyDamage = 0;

                //Check if someone dies or cant play and move to KO_PHASE if not, Preparation again.

                ChangeState(state);

                break;
            case GameState.KO_PHASE:
                eventSystem.enabled = false;
                Debug.Log("[GAMESTATE] KO phase");
                break;
        }

        currentState = state;
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

    private Stack<Card> GenerateRandomDeck()
    {
        Stack<Card> deck = new Stack<Card>();

        int totalInitCards = initProportions * deckSize / 100;
        for (int i = 0; i < totalInitCards; ++i)
        {
            int randomCard = Random.Range(0, availableInitCards.Count);
            deck.Push(availableInitCards[randomCard]);
        }

        int totalFinisherstCards = finisherProportions * deckSize / 100;
        for (int i = 0; i < totalFinisherstCards; ++i)
        {
            int randomCard = Random.Range(0, availableFinisherCards.Count);
            deck.Push(availableFinisherCards[randomCard]);
        }

        int totalMiddleCards = deckSize - (totalInitCards + totalFinisherstCards);
        for (int i = 0; i < totalMiddleCards; ++i)
        {
            int randomCard = Random.Range(0, availableMiddleCards.Count);
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

    private void ToggleCardsAndActions(bool toggle)
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
            Destroy(deckReference.GetChild(deckReference.childCount - i - 1).gameObject); // -1 because there ir a transparent card
        }
    }
}
