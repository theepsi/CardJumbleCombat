using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCFighter {

    private Fighter fighter;

    public PCFighter (Fighter fighter)
    {
        this.fighter = fighter;
    }

    public void SelectCombo()
    {
        List<List<Card>> possibleCombinations = GenerateAllCombinations();

        int randomCombo = Random.Range(0, possibleCombinations.Count);
        fighter.PlayCards(possibleCombinations[randomCombo]);
    }

    private List<List<Card>> GenerateAllCombinations()
    {
        List<Card> hand = fighter.Hand;
        List<List<Card>> possibleCombinations = new List<List<Card>>();

        List<Card> initCards = new List<Card>();
        List<Card> middleCards = new List<Card>();
        List<Card> finisherCards = new List<Card>();

        for (int i = 0; i < hand.Count; ++i)
        {
            if (hand[i].type == CardType.COMBO_INIT) initCards.Add(hand[i]);
            else if (hand[i].type == CardType.COMBO_MIDDLE) middleCards.Add(hand[i]);
            else finisherCards.Add(hand[i]);
        }

        // TODO: Get all possible combinations of middle cards
        List<List<Card>> middleCardsCombinations = GetCombination(middleCards);

        for (int i = 0; i < initCards.Count; ++i)
        {
            List<Card> comboInit = new List<Card>();
            comboInit.Add(initCards[i]);
            possibleCombinations.Add(comboInit);

            for (int j = 0; j < middleCardsCombinations.Count; ++i)
            {
                List<Card> comboInitMiddle = new List<Card>(comboInit);

                for (int k = 0; k < middleCardsCombinations[j].Count; k++)
                {
                    comboInitMiddle.Add(middleCardsCombinations[j][k]);
                }

                possibleCombinations.Add(comboInitMiddle);

                for (int k = 0; k < finisherCards.Count; ++i)
                {
                    List<Card> comboInitMiddleFinisher = new List<Card>(comboInitMiddle);
                    comboInitMiddleFinisher.Add(finisherCards[k]);
                    possibleCombinations.Add(comboInitMiddleFinisher);
                }
            }
        }

        return possibleCombinations;
    }

    private List<List<Card>> GetCombination(List<Card> list)
    {
        List<List<Card>> listCombinations = new List<List<Card>>();

        for (int i = 0; i < list.Count; ++i)
        {
            List<Card> simple = new List<Card>();
            simple.Add(list[i]);
            listCombinations.Add(simple);
        }

        for (int i = 0; i < listCombinations.Count; ++i)
        {
            for (int j = 0; j < list.Count; ++i)
            {
                if (!listCombinations[i].Contains(list[j]))
                {
                    List<Card> combination = new List<Card>(listCombinations[i]);
                    combination.Add(list[j]);
                    listCombinations.Add(combination);
                }
            }
        }

        for (int i = 0; i < listCombinations.Count; ++i)
        {
            Debug.Log("List-" + i);

            for (int j = 0; j < listCombinations[i].Count; ++j)
            {
                listCombinations[i][j].Print();
            }
        }

        return listCombinations;
    }
}
