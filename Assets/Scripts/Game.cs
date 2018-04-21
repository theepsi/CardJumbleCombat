using System.Collections;
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

    //DELETE
    private void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        // prepare player and enemy (life, cards, etc)
        player = new Fighter();
        enemy = new Fighter();
        // change game state to preparation
        debugCards = new int[availableInitCards.Count + availableMiddleCards.Count + availableFinisherCards.Count];
        Stack<Card> deck = GenerateRandomDeck();

        player.Init(Shuffle(deck));
        enemy.Init(Shuffle(deck));

        DisplayPlayerHand(player.Hand);


        //for (int i = 0; i < debugCards.Length; ++i)
        //{
        //    Debug.Log("card-" + (i + 1) + ": " + debugCards[i]);
        //}

        eventSystem.enabled = true;
    }

    private void DisplayPlayerHand(List<Card> hand)
    {
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

    public Stack<Card> Shuffle(Stack<Card> stack)
    {
        System.Random rnd = new System.Random();
        return new Stack<Card>(stack.OrderBy(x => rnd.Next()));
    }
}
