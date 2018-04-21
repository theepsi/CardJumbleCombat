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

    private GameState currentState;
    private int[] debugCards;

    private Fighter player;
    private Fighter enemy;
    private PCFighter pcFighter;

    private int playerDamage;
    private int enemyDamage;

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
        debugCards = new int[availableInitCards.Count + availableMiddleCards.Count + availableFinisherCards.Count];
        Stack<Card> deck = GenerateRandomDeck();
        Stack<Card> enemy_deck = GenerateRandomDeck();

        player.Init(Shuffle(deck));
        enemy.Init(Shuffle(enemy_deck));

        DisplayPlayerHand(player.Hand);

        //PrintRandomDeck();

        ChangeState(GameState.PREPARATION_PHASE);
    }

    public void ReadyButton()
    {
        List<Card> selectedCards = GetSelectedCards();

        if (selectedCards.Count > 0)
        {
            if (player.CheckCards(selectedCards))
            {
                playerDamage = player.PlayCards(selectedCards);

                DisplayPlayerHand(player.Hand);

                enemyDamage = pcFighter.SelectCombo();

                ChangeState(GameState.DAMAGE_PHASE);
            }
            else
            {
                Debug.Log("Wrong card combination.");
            }
        }
        else
        {
            Debug.Log("No cards Selected.");
        }
    }

    public void DoGuard()
    {
        List<Card> selectedCards = GetSelectedCards();

        Debug.Log("GUARD YES");
        //With Guard button you will use your selected cards as defense, so you will have some defense points and you wont attack this turn. The more you discard, the less you defend.
        ToggleCardsAndActions(true);
               
        player.Guard(selectedCards);

        DisplayPlayerHand(player.Hand);

        //Enemy damage;

        DismissGuard();

        ChangeState(GameState.DAMAGE_PHASE);
    }

    public void GuardButton()
    {
        Debug.Log("GUARD");

        ToggleCardsAndActions(false);

        List<Card> selectedCards = GetSelectedCards();

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
                Debug.Log("Preparation phase");
                break;
            case GameState.DAMAGE_PHASE:
                eventSystem.enabled = false;
                Debug.Log("Damage phase");

                //Fancy animations and apply damage to both players and also health bars
                Debug.Log("Player did " + playerDamage + " to Enemy");
                enemy.ApplyDamage(playerDamage);
                player.ApplyDamage(enemyDamage);

                enemyHealthController.ApplyDamage(playerDamage);
                playerHealthController.ApplyDamage(enemyDamage);

                playerDamage = 0;
                enemyDamage = 0;

                //Check if someone dies or cant play and move to KO_PHASE if not, Preparation again.

                ChangeState(GameState.PREPARATION_PHASE);

                break;
            case GameState.KO_PHASE:
                eventSystem.enabled = false;
                Debug.Log("KO phase");
                break;
        }

        currentState = state;

        Debug.Log("ChangeState to " + currentState.ToString());
    }

    private void DisplayPlayerHand(Card[] hand)
    {
        CleanHand();

        for (int i = 0; i < hand.Length; ++i)
        {
            GameObject card = ObjectPooler.Instance.GetPooledObject("Card");

            card.transform.SetParent(handReference);
            hand[i].indexAtHand = i;
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
            debugCards[randomCard]++;
            deck.Push(availableInitCards[randomCard]);
        }

        int totalFinisherstCards = finisherProportions * deckSize / 100;
        for (int i = 0; i < totalFinisherstCards; ++i)
        {
            int randomCard = Random.Range(0, availableFinisherCards.Count);
            debugCards[randomCard + availableInitCards.Count + availableMiddleCards.Count]++;
            deck.Push(availableFinisherCards[randomCard]);
        }

        int totalMiddleCards = deckSize - (totalInitCards + totalFinisherstCards);
        for (int i = 0; i < totalMiddleCards; ++i)
        {
            int randomCard = Random.Range(0, availableMiddleCards.Count);
            debugCards[randomCard + availableInitCards.Count]++;
            deck.Push(availableMiddleCards[randomCard]);
        }

        return deck;
    }

    private List<Card> GetSelectedCards()
    {
        List<Card> returnList = new List<Card>();
        CardDisplayer[] cards = handReference.GetComponentsInChildren<CardDisplayer>();
        for (int i = 0; i < cards.Length; ++i)
        {
            if (cards[i].selected) returnList.Add(cards[i].card);
        }
        return returnList;
    }

    private Stack<Card> Shuffle(Stack<Card> stack)
    {
        System.Random rnd = new System.Random();
        return new Stack<Card>(stack.OrderBy(x => rnd.Next()));
    }

    private void PrintRandomDeck()
    {
        for (int i = 0; i < debugCards.Length; ++i)
        {
            Debug.Log("card-" + (i + 1) + ": " + debugCards[i]);
        }
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
}
