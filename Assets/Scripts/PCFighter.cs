using System.Collections.Generic;
using UnityEngine;

public class PCFighter {

    private Fighter fighter;

    public PCFighter (Fighter fighter)
    {
        this.fighter = fighter;
    }

    public int Play()
    {
        List<List<CardMap>> possibleCombinations = GenerateAllCombinations();

        if (possibleCombinations.Count == 0)
        {
            // TODO: Play guard and decide which ones does he change
            List<CardMap> discardCards = GetCardToDiscard();
            Debug.Log("[GUARD] Computer - GUARD YES");

            fighter.Guard(discardCards);
            return 0;

        } else
        {
            int randomCombo = Random.Range(0, possibleCombinations.Count);
            return fighter.PlayCards(possibleCombinations[randomCombo]);
        }
    }

    private List<CardMap> GetCardToDiscard()
    {
        List<CardMap> discardCards = new List<CardMap>();
        List<int> visitedCards = new List<int>();
        int randomDiscard = Random.Range(1, fighter.Hand.Length);
        int randomCard = Random.Range(0, fighter.Hand.Length);

        for (int i = 0; i < randomDiscard; ++i)
        {
            while (visitedCards.Contains(randomCard))
                randomCard = Random.Range(0, fighter.Hand.Length);
            visitedCards.Add(randomCard);
            discardCards.Add(fighter.Hand[randomCard]);
        }

        return discardCards;
    }

    private List<List<CardMap>> GenerateAllCombinations()
    {
        CardMap[] hand = fighter.Hand;
        List<List<CardMap>> possibleCombinations = new List<List<CardMap>>();

        List<CardMap> initCards = new List<CardMap>();
        List<CardMap> middleCards = new List<CardMap>();
        List<CardMap> finisherCards = new List<CardMap>();

        for (int i = 0; i < hand.Length; ++i)
        {
            if (hand[i].card.type == CardType.COMBO_INIT) initCards.Add(hand[i]);
            else if (hand[i].card.type == CardType.COMBO_MIDDLE) middleCards.Add(hand[i]);
            else finisherCards.Add(hand[i]);
        }

        // Get all possible combinations of middle cards
        List<List<CardMap>> middleCardsCombinations = GenerateCombinations(middleCards, new List<List<CardMap>>());

        for (int i = 0; i < initCards.Count; ++i)
        {
            List<CardMap> comboInit = new List<CardMap>();
            comboInit.Add(initCards[i]);
            possibleCombinations.Add(comboInit);

            for (int j = 0; j < middleCardsCombinations.Count; ++j)
            {
                List<CardMap> comboInitMiddle = new List<CardMap>(comboInit);

                for (int k = 0; k < middleCardsCombinations[j].Count; ++k)
                {
                    comboInitMiddle.Add(middleCardsCombinations[j][k]);
                }

                possibleCombinations.Add(comboInitMiddle);

                for (int k = 0; k < finisherCards.Count; ++k)
                {
                    List<CardMap> comboInitMiddleFinisher = new List<CardMap>(comboInitMiddle);
                    comboInitMiddleFinisher.Add(finisherCards[k]);
                    possibleCombinations.Add(comboInitMiddleFinisher);
                }
            }
        }

        return possibleCombinations;
    }

    private List<List<CardMap>> GenerateCombinations(List<CardMap> list, List<List<CardMap>> listCombinations)
    {
        List<List<CardMap>> allCombinations = new List<List<CardMap>>(listCombinations);
        List<List<CardMap>> combinationsGenerated = new List<List<CardMap>>();

        if (listCombinations.Count == 0)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                List<CardMap> simple = new List<CardMap>();
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
                        List<CardMap> combination = new List<CardMap>(listCombinations[i]);
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
            List<List<CardMap>> recursivelyGenerated = GenerateCombinations(list, allCombinations);
            for (int i = 0; i < recursivelyGenerated.Count; ++i)
            {
                combinationsGenerated.Add(recursivelyGenerated[i]);
            }
        }

        return combinationsGenerated;
    }

    private bool ListInListOfLists(List<CardMap> list, List<List<CardMap>> listOfLists)
    {
        for (int i = 0; i < listOfLists.Count; ++i)
        {
            List<CardMap> listCopy = new List<CardMap>(list);
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
