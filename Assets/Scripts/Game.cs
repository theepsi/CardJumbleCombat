using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameState
{
    PREPARATION_PHASE,
    DAMAGE_PHASE,
    KO_PHASE
}

public class Game : MonoBehaviour {

    public EventSystem eventSystem;

    public Transform handReference;

    public List<Card> availableInitCards;
    public List<Card> availableMiddleCards;
    public List<Card> availableFinisherCards;

    public int initProportions = 41;
    public int finisherProportions = 9;

    public int deckSize = 100;

    private GameState currentState;
    private int[] debugCards;

    private Fighter player;
    private Fighter enemy;

    private int playerDamage;
    private int enemyDamage;

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

                //Enemy damage;

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

    private void DisplayPlayerHand(List<Card> hand)
    {
        CleanHand();

        for (int i = 0; i < hand.Count; ++i)
        {
            GameObject card = ObjectPooler.Instance.GetPooledObject("Card");

            card.transform.SetParent(handReference);
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
            cards[i].gameObject.SetActive(false);
        }
    }
}
