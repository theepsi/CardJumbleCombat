using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct CardMap
{
    public int index;
    public Card card;

    public CardMap(int index, Card card)
    {
        this.index = index;
        this.card = card;
    }
}

public class Fighter {

    public int maxHealth = 150;
    public int maxGauge = 50;

    public CardMap[] Hand { get { return hand; } }

    public Sprite artWork;

    public bool IsPrepared { get; set; }

    private int currentHealth;
    private int currentGauge;

    private Stack<Card> deck;
    private CardMap[] hand;
    private int maxCardHand = 6;
    private int cardsHand = 0;

    private float defensePoints = 1;

    public void Init(Stack<Card> deck)
    {
        this.deck = deck;
        hand = new CardMap[maxCardHand];

        for (int i = 0; i < hand.Length; ++i)
        {
            hand[i] = new CardMap(i, null);
        }

        GetCards();
    }

    public int PlayCards(List<CardMap> cardsToPlay)
    {
        int damage = 0;
        defensePoints = 1;

        for (int i = 0; i < cardsToPlay.Count; ++i)
        {
            damage += cardsToPlay[i].card.damage;
        }

        DiscardCards(cardsToPlay);

        currentGauge += damage;

        GetCards();

        return damage;
    }

    public bool CheckCards(List<CardMap> cardsToPlay)
    {
        if (!hasOneCardOfType(CardType.COMBO_INIT, cardsToPlay.ToArray())) return false;
        else
        {
            if (hasOneCardOfType(CardType.COMBO_FINISHER, cardsToPlay.ToArray()) && !hasMiddleCards(cardsToPlay)) return false;
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

    public void Guard(List<CardMap> cardsToDiscard)
    {
        defensePoints = 1f / (2 + cardsToDiscard.Count);
        Debug.Log("Defense points updated to: " + defensePoints);
        DiscardCards(cardsToDiscard);
        GetCards();
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= Mathf.RoundToInt(damage * defensePoints);
    }

    private void DiscardCards(List<CardMap> cardsToDiscard)
    {
        //TODO: Discard or queue to deck(?)
        //hand = hand.Except(cardsToDiscard).ToList();
        for (int i = 0; i < cardsToDiscard.Count; ++i)
        {
            hand[cardsToDiscard[i].index].card = null;
            cardsHand--;
        }
    }

    private void GetCards()
    {
        List<int> emptyIndex = new List<int>();
        for (int i = 0; i < hand.Length; ++i)
        {
            if (hand[i].card == null)
            {
                emptyIndex.Add(hand[i].index);
            }
        }

        int index = 0;

        while (cardsHand < maxCardHand && deck.Count > 0)
        {
            hand[emptyIndex[index]].card = deck.Pop();
            hand[emptyIndex[index]].index = emptyIndex[index];
            index++;
            cardsHand++;
        }

        Debug.Log("Just draw cards, remaining cards: " + deck.Count);
        PrintHand();
    }

    private bool hasOneCardOfType(CardType type, CardMap[] list)
    {
        int count = 0;

        for (int i = 0; i < list.Length; ++i)
        {
            if (list[i].card.type == type) count++;

            if (count > 1) return false;
        }

        if (count == 0)
            return false;
        return true;
    }

    private bool hasMiddleCards(List<CardMap> list)
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].card.type == CardType.COMBO_MIDDLE) return true;
        }

        return false;
    }

    private void PrintHand()
    {
        for (int i = 0; i < hand.Length; ++i)
        {
           hand[i].card.Print();
        }
    }
}
