using System.Collections.Generic;
using UnityEngine;

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

    public string name = "";

    public CardMap[] Hand { get { return hand; } }

    public float DefensePoints { get { return defensePoints; } }

    public Sprite artWork;

    public bool IsPrepared { get; set; }

    private int currentHealth;
    private int currentGauge;

    private Stack<Card> deck;
    private CardMap[] hand;
    private int maxCardHand = 6;
    private int cardsHand = 0;

    private float defensePoints = 1;

    public void Init(Stack<Card> deck, string name)
    {
        this.deck = deck;
        this.name = name;
        hand = new CardMap[maxCardHand];

        for (int i = 0; i < hand.Length; ++i)
        {
            hand[i] = new CardMap(i, null);
        }

        currentHealth = maxHealth;
        currentGauge = 0;

        GetCards();
    }

    public int PlayCards(List<CardMap> cardsToPlay)
    {
        int damage = 0;
        defensePoints = 1f;

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

    public GameState CheckFighterState()
    {
        if (currentHealth <= 0)
        {
            Debug.Log("[END] Fighter has lost: " + name);
            if (name == "Computer") return GameState.WIN_PHASE;
            return GameState.KO_PHASE;
        }

        else if (!hasOneCardOfType(CardType.COMBO_INIT, hand) && deck.Count == 0)
        {
            Debug.Log("[END] Fighter has lost: " + name);
            if (name == "Computer") return GameState.WIN_PHASE;
            return GameState.KO_PHASE;
        }
        
        else if (!hasOneCardOfType(CardType.COMBO_INIT, hand) && deck.Count > 0)
        {
            //TODO force guard (?)
        }
        return GameState.PREPARATION_PHASE;
    }

    public void Guard(List<CardMap> cardsToDiscard)
    {
        defensePoints = 1f / (2 + cardsToDiscard.Count);
        Debug.Log("[DEFENSE] Defense points updated to: " + defensePoints);
        DiscardCards(cardsToDiscard);
        GetCards();
    }

    public int ApplyDamage(int damage)
    {
        int recivedDamage = Mathf.RoundToInt(damage * defensePoints);
        currentHealth -= recivedDamage;
        Debug.Log("[DAMAGE] " + name + " recived " + recivedDamage);
        return recivedDamage;
    }

    private void DiscardCards(List<CardMap> cardsToDiscard)
    {
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
