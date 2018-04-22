using System.Collections.Generic;
using UnityEngine;

public class PCFighter {

    private Fighter fighter;

    private int aiDifficulty = 1;

    public PCFighter (Fighter fighter)
    {
        this.fighter = fighter;
    }

    public int Play()
    {
        aiDifficulty = EnemyDifficulty.AIDifficulty;

        List<List<CardMap>> possibleCombinations = GenerateAllCombinations();

        if (possibleCombinations.Count == 0)
        {
            // TODO: Play guard and decide which ones does he change
            Debug.Log("[GUARD] Computer - GUARD YES");
            fighter.Guard(GetCardToDiscard());
            return 0;

        } else
        {
            return fighter.PlayCards(SelectCombination(possibleCombinations));
        }
    }

    private List<CardMap> SelectCombination(List<List<CardMap>> possibleCombinations)
    {
        List<CardMap> selectedCombination = new List<CardMap>();

        if (aiDifficulty == 0)
        {
            // IA is stupid
            return GetBadCombo(possibleCombinations);
        }
        else if (aiDifficulty == 1)
        {
            // IA is random
            int randomCombo = Random.Range(0, possibleCombinations.Count);
            return possibleCombinations[randomCombo];
        } else if (aiDifficulty == 2)
        {
            // IA is a genious
            return GetBestCombo(possibleCombinations);
        }

        return selectedCombination;
    }

    private List<CardMap> GetBadCombo(List<List<CardMap>> possibleCombinations)
    {
        int threshold = 20;
        List<List<CardMap>> worstCombos = new List<List<CardMap>>();

        for (int i = 0; i < possibleCombinations.Count; ++i)
        {
            int damage = 0;

            for (int j = 0; j < possibleCombinations[i].Count; ++j)
            {
                damage += possibleCombinations[i][j].card.damage;
            }

            if (damage < threshold)
            {
                worstCombos.Add(possibleCombinations[i]);
            }
        }

        int randomCombo = Random.Range(0, worstCombos.Count);
        return worstCombos[randomCombo];
    }

    private List<CardMap> GetBestCombo(List<List<CardMap>> possibleCombinations)
    {
        int highestValue = 0;
        List<CardMap> bestCombo = new List<CardMap>();

        for (int i = 0; i < possibleCombinations.Count; ++i)
        {
            int damage = 0;

            for (int j = 0; j < possibleCombinations[i].Count; ++j)
            {
                damage += possibleCombinations[i][j].card.damage;
            }

            if (damage > highestValue)
            {
                highestValue = damage;
                bestCombo = possibleCombinations[i];
            }
        }

        return bestCombo;
    }

    private List<CardMap> GetCardToDiscard()
    {
        List<CardMap> discardCards = new List<CardMap>();

        if (aiDifficulty == 0 || aiDifficulty == 1)
        {
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
        }
        else if (aiDifficulty == 2)
        {
            // IA is a genious
            int threshold = 4;

            for (int i = 0; i < fighter.Hand.Length; ++i)
            {
                if (fighter.Hand[i].card.type == CardType.COMBO_MIDDLE && fighter.Hand[i].card.damage <= threshold)
                {
                    discardCards.Add(fighter.Hand[i]);
                }
            }

            if (discardCards.Count == 0)
            {
                int lowestDamage = 50;
                CardMap lowestCard = new CardMap();
                for (int i = 0; i < fighter.Hand.Length; ++i)
                {
                    if (fighter.Hand[i].card.damage < lowestDamage)
                    {
                        lowestDamage = fighter.Hand[i].card.damage;
                        lowestCard = fighter.Hand[i];
                    }
                }

                discardCards.Add(lowestCard);
            }
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
