using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Fighter {

    public int maxHealth = 150;
    public int maxGauge = 50;

    public List<Card> Hand { get { return hand; } }

    public Sprite artWork;

    public bool IsPrepared { get; set; }

    private int currentHealth;
    private int currentGauge;

    private Stack<Card> deck;
    private List<Card> hand;
    private int maxCardHand = 6;
    private int cardsHand = 0;

    private float defensePoints = 1;

    public void Init(Stack<Card> deck)
    {
        this.deck = deck;
        hand = new List<Card>();
        GetCards();
    }

    public int PlayCards(List<Card> cardsToPlay)
    {
        int damage = 0;
        defensePoints = 1;

        for (int i = 0; i < cardsToPlay.Count; ++i)
        {
            damage += cardsToPlay[i].damage;
        }

        hand = hand.Except(cardsToPlay).ToList();

        currentGauge += damage;

        cardsHand = hand.Count;

        GetCards();

        return damage;
    }

    public bool CheckCards(List<Card> cardsToPlay)
    {
        if (!hasOneCardOfType(CardType.COMBO_INIT, cardsToPlay)) return false;
        else
        {
            if (hasOneCardOfType(CardType.COMBO_FINISHER, cardsToPlay) && !hasMiddleCards(cardsToPlay)) return false;
        }

        return true;
    }

    public void CheckFighterState()
    {
        if (!hasOneCardOfType(CardType.COMBO_INIT, hand) && deck.Count == 0)
        {
            //TODO fighter looooooose
        }
        
        else if (!hasOneCardOfType(CardType.COMBO_INIT, hand) && deck.Count > 0)
        {
            //TODO force guard (?)
        }
    }

    public void Guard(List<Card> cardsToDiscard)
    {
        this.defensePoints = 1 / (2 + cardsToDiscard.Count);
        DiscardCards(cardsToDiscard);
        GetCards();
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= Mathf.RoundToInt(damage * defensePoints);
    }

    private void DiscardCards(List<Card> cardsToDiscard)
    {
        //TODO: Discard or queue to deck(?)
        hand = hand.Except(cardsToDiscard).ToList();
        cardsHand = hand.Count;
    }

    private void GetCards()
    {
        while (cardsHand < maxCardHand && deck.Count > 0)
        {
            hand.Add(deck.Pop());
            cardsHand++;
        }

        Debug.Log("Just draw cards, remaining cards: " + deck.Count);
        PrintHand();
    }

    private bool hasOneCardOfType(CardType type, List<Card> list)
    {
        int count = 0;

        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].type == type) count++;

            if (count > 1) return false;
        }

        if (count == 0)
            return false;
        return true;
    }

    private bool hasMiddleCards(List<Card> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].type == CardType.COMBO_MIDDLE) return true;
        }

        return false;
    }

    private void PrintHand()
    {
        for (int i = 0; i < hand.Count; ++i)
        {
           hand[i].Print();
        }
    }
}
