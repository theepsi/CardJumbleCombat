using System.Collections.Generic;
using UnityEngine;

public class Fighter {

    public int maxHealth = 150;
    public int maxGauge = 100;

    public Sprite artWork;

    public bool IsPrepared { get; set; }

    private int currentHealth;
    private int currentGauge;

    private Queue<Card> deck;
    private List<Card> hand;
    private int maxCardHand = 6;
    private int cardsHand = 0;

    private float defensePoints = 0;

    public void Init(Queue<Card> deck)
    {
        this.deck = deck;
        getCards();
    }

    public int PlayCards(List<Card> cardsToPlay)
    {
        int damage = 0;

        for (int i = 0; i < cardsToPlay.Count; ++i)
        {
            damage += cardsToPlay[i].damage;
        }

        hand.RemoveAll(x => cardsToPlay.Contains(x));

        return damage;
    }

    public bool CheckCards(List<Card> cardsToPlay)
    {
        if (!hasOneCardOfType(CardType.COMBO_INIT)) return false;
        else
        {
            if (hasOneCardOfType(CardType.COMBO_FINISHER) && !hasMiddleCards()) return false;
        }

        return true;
    }

    public void CheckFighterState()
    {
        if (!hasOneCardOfType(CardType.COMBO_INIT) && deck.Count == 0)
        {
            //TODO fighter looooooose
        }
        
        else if (!hasOneCardOfType(CardType.COMBO_INIT) && deck.Count > 0)
        {
            //TODO force guard (?)
        }
    }

    public void Guard(List<Card> cardsToDiscard)
    {
        this.defensePoints = 1 / (2 + cardsToDiscard.Count);
        DiscardCards(cardsToDiscard);
        getCards();
    }

    private void DiscardCards(List<Card> cardsToDiscard)
    {
        //TODO: Discard or queue to deck(?)
        hand.RemoveAll(x => cardsToDiscard.Contains(x));
    }

    private void getCards()
    {
        while (cardsHand < maxCardHand && deck.Count > 0)
        {
            hand.Add(deck.Dequeue());
        }
    }

    private bool hasOneCardOfType(CardType type)
    {
        int count = 0;

        for (int i = 0; i < hand.Count; ++i)
        {
            if (hand[i].type == type) count++;

            if (count > 1) return false;
        }

        return true;
    }

    private bool hasMiddleCards()
    {
        for (int i = 0; i < hand.Count; ++i)
        {
            if (hand[i].type == CardType.COMBO_MIDDLE) return true;
        }

        return false;
    }
}
