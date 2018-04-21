using System.Collections.Generic;
using UnityEngine;

public class PCFighter {

    private Fighter fighter;

    public PCFighter (Fighter fighter)
    {
        this.fighter = fighter;
    }

    public int SelectCombo()
    {
        List<List<Card>> possibleCombinations = GenerateAllCombinations();

        int randomCombo = Random.Range(0, possibleCombinations.Count);
        return fighter.PlayCards(possibleCombinations[randomCombo]);

        //TODO: if no combo, play guard and decide which ones does he change
    }

    private List<List<Card>> GenerateAllCombinations()
    {
        Card[] hand = fighter.Hand;
        List<List<Card>> possibleCombinations = new List<List<Card>>();

        List<Card> initCards = new List<Card>();
        List<Card> middleCards = new List<Card>();
        List<Card> finisherCards = new List<Card>();

        for (int i = 0; i < hand.Length; ++i)
        {
            if (hand[i].type == CardType.COMBO_INIT) initCards.Add(hand[i]);
            else if (hand[i].type == CardType.COMBO_MIDDLE) middleCards.Add(hand[i]);
            else finisherCards.Add(hand[i]);
        }

        Debug.Log("Middle cards");
        for (int i = 0; i < middleCards.Count; ++i)
        {
            middleCards[i].Print();
        }

        // Get all possible combinations of middle cards
        List<List<Card>> middleCardsCombinations = GenerateCombinations(middleCards, new List<List<Card>>());

        for (int i = 0; i < middleCardsCombinations.Count; ++i)
        {
            Debug.Log("Combination " + i);
            for (int j = 0; j < middleCardsCombinations[i].Count; ++j)
                middleCardsCombinations[i][j].Print();
        }

        for (int i = 0; i < initCards.Count; ++i)
        {
            List<Card> comboInit = new List<Card>();
            comboInit.Add(initCards[i]);
            possibleCombinations.Add(comboInit);

            for (int j = 0; j < middleCardsCombinations.Count; ++j)
            {
                List<Card> comboInitMiddle = new List<Card>(comboInit);

                for (int k = 0; k < middleCardsCombinations[j].Count; ++k)
                {
                    comboInitMiddle.Add(middleCardsCombinations[j][k]);
                }

                possibleCombinations.Add(comboInitMiddle);

                for (int k = 0; k < finisherCards.Count; ++k)
                {
                    List<Card> comboInitMiddleFinisher = new List<Card>(comboInitMiddle);
                    comboInitMiddleFinisher.Add(finisherCards[k]);
                    possibleCombinations.Add(comboInitMiddleFinisher);
                }
            }
        }

        return possibleCombinations;
    }

    private List<List<Card>> GenerateCombinations(List<Card> list, List<List<Card>> listCombinations)
    {
        List<List<Card>> allCombinations = new List<List<Card>>(listCombinations);
        List<List<Card>> combinationsGenerated = new List<List<Card>>();

        if (listCombinations.Count == 0)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                List<Card> simple = new List<Card>();
                simple.Add(list[i]);
                allCombinations.Add(simple);
                combinationsGenerated.Add(simple);
            }
        }
        else
        {

            for (int i = 0; i < listCombinations.Count; ++i)
            {
                for (int j = 0; j < list.Count; ++j)
                {
                    if (!listCombinations[i].Contains(list[j]))
                    {
                        List<Card> combination = new List<Card>(listCombinations[i]);
                        combination.Add(list[j]);
                        if (!ListInListOfLists(combination, allCombinations))
                        {
                            allCombinations.Add(combination);
                            combinationsGenerated.Add(combination);
                        }
                    }
                }
            }
        }

        if (combinationsGenerated.Count != 0)
        {
            List<List<Card>> recursivelyGenerated = GenerateCombinations(list, allCombinations);
            for (int i = 0; i < recursivelyGenerated.Count; ++i)
            {
                combinationsGenerated.Add(recursivelyGenerated[i]);
            }
        }

        return combinationsGenerated;
    }

    private bool ListInListOfLists(List<Card> list, List<List<Card>> listOfLists)
    {
        for (int i = 0; i < listOfLists.Count; ++i)
        {
            List<Card> listCopy = new List<Card>(list);
            bool listOfListsElementNotInList = false;

            for (int j = 0; j < listOfLists[i].Count; ++j)
            {
                if (listCopy.Contains(listOfLists[i][j]))
                {
                    listCopy.Remove(listOfLists[i][j]);
                }
                else
                {
                    listOfListsElementNotInList = true;
                }
            }
            if (listCopy.Count == 0 && !listOfListsElementNotInList)
            {
                return true;
            }
        }
        return false;
    }
}
