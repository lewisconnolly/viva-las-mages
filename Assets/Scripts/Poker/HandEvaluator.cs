using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;

public class HandEvaluator : MonoBehaviour
{
    public static HandEvaluator instance;

    private void Awake()
    {
        instance = this;
    }

    public enum HandRank
    {
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        FullHouse,
        Flush,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public enum TieWinner
    {
        Player,
        Enemy,
        Tie
    }

    public HandRank EvaluateHand(List<Card> hand, bool evaluatePowerCards)
    {
        List<Card> newHand = hand;

        if (evaluatePowerCards)
        {
            newHand = PowerCardController.instance.EvaluatePowerCards(hand);
        }
        
        if (IsRoyalFlush(newHand)) return HandRank.RoyalFlush;
        if (IsStraightFlush(newHand)) return HandRank.StraightFlush;
        if (IsFourOfAKind(newHand)) return HandRank.FourOfAKind;
        if (IsFullHouse(newHand)) return HandRank.FullHouse;
        if (IsFlush(newHand)) return HandRank.Flush;
        if (IsStraight(newHand)) return HandRank.Straight;
        if (IsThreeOfAKind(newHand)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(newHand)) return HandRank.TwoPair;
        if (IsOnePair(newHand)) return HandRank.OnePair;
        return HandRank.HighCard;
    }

    public TieWinner BreakTie(List<Card> playerHand, List<Card> enemyHand, HandRank rank)
    {
        if (rank == HandRank.RoyalFlush) return TieWinner.Tie;
        if (rank == HandRank.StraightFlush) return BreakStraightTie(playerHand, enemyHand);
        if (rank == HandRank.FourOfAKind) return BreakFourOfAKindTie(playerHand, enemyHand);
        if (rank == HandRank.FullHouse) return BreakFullHouseTie(playerHand, enemyHand);
        if (rank == HandRank.Flush || rank == HandRank.HighCard) return BreakFlushOrHighCardTie(playerHand, enemyHand);
        if (rank == HandRank.Straight) return BreakStraightTie(playerHand, enemyHand);
        if (rank == HandRank.ThreeOfAKind) return BreakThreeOfAKindTie(playerHand, enemyHand);
        if (rank == HandRank.TwoPair) return BreakTwoPairTie(playerHand, enemyHand);
        if (rank == HandRank.OnePair) return BreakOnePairTie(playerHand, enemyHand);
        return TieWinner.Tie;
    }

    private bool IsRoyalFlush(List<Card> hand)
    {
        return IsStraightFlush(hand) && hand.All(card => card.value >= 10);
    }

    private bool IsStraightFlush(List<Card> hand)
    {
        return IsFlush(hand) && IsStraight(hand);
    }

    private TieWinner BreakStraightTie(List<Card> PH, List<Card> EH)
    {
        // Check special case of ace-low straight flush
        // Check one or two ace-low hands
        // Player ace-low, enemy not
        if ((PH[0].value == 6 && PH[4].value == 14) && !(EH[0].value == 6 && EH[4].value == 14))
        {
            return TieWinner.Enemy;
        }

        // Enemy ace-low, player not
        if ((EH[0].value == 6 && EH[4].value == 14) && !(PH[0].value == 6 && PH[4].value == 14))
        {
            return TieWinner.Player;
        }

        // Both ace-low
        if ((EH[0].value == 6 && EH[4].value == 14) && (PH[0].value == 6 && PH[4].value == 14))
        {
            return TieWinner.Tie;
        }

        // No ace-low hands, check highest card
        if (PH[4].value < EH[4].value)
        {
            return TieWinner.Enemy;
        }
        else if (PH[4].value > EH[4].value)
        {
            return TieWinner.Player;
        }
        else
        {
            return TieWinner.Tie;
        }
    }

    private bool IsFourOfAKind(List<Card> hand)
    {
        var valueGroups = hand.GroupBy(card => card.value);
        return valueGroups.Any(group => group.Count() == 4);
    }

    private TieWinner BreakFourOfAKindTie(List<Card> PH, List<Card> EH)
    {
        // Get four of a kind value for player and enemy
        int pFoakVal = PH.GroupBy(card => card.value).Where(group => group.Count() == 4).SelectMany(group => group).ToList().First().value;
        int eFoakVal = EH.GroupBy(card => card.value).Where(group => group.Count() == 4).SelectMany(group => group).ToList().First().value;

        // Higher four of a kind value wins, if same then check remaining card
        if (pFoakVal < eFoakVal)
        {
            return TieWinner.Enemy;
        }
        else if (pFoakVal > eFoakVal)
        {
            return TieWinner.Player;
        }
        else
        {
            int pKicker = PH.Where(card => card.value != pFoakVal).First().value;
            int eKicker = EH.Where(card => card.value != eFoakVal).First().value;

            if (pKicker < eKicker)
            {
                return TieWinner.Enemy;
            }
            else if (pKicker > eKicker)
            {
                return TieWinner.Player;
            }
            else
            {
                return TieWinner.Tie;
            }
        }
    }

    private bool IsFullHouse(List<Card> hand)
    {
        var valueGroups = hand.GroupBy(card => card.value);
        return valueGroups.Any(group => group.Count() == 3) && valueGroups.Any(group => group.Count() == 2);
    }

    private TieWinner BreakFullHouseTie(List<Card> PH, List<Card> EH)
    {
        // Get three of a kind value for player and enemy
        int pToakVal = PH.GroupBy(card => card.value).Where(group => group.Count() == 3).SelectMany(group => group).ToList().First().value;
        int eToakVal = EH.GroupBy(card => card.value).Where(group => group.Count() == 3).SelectMany(group => group).ToList().First().value;

        // Higher three of a kind value wins, if same then check pair
        if (pToakVal < eToakVal)
        {
            return TieWinner.Enemy;
        }
        else if (pToakVal > eToakVal)
        {
            return TieWinner.Player;
        }
        else
        {
            int pPairVal = PH.Where(card => card.value != pToakVal).First().value;
            int ePairVal = EH.Where(card => card.value != eToakVal).First().value;

            if (pPairVal < ePairVal)
            {
                return TieWinner.Enemy;
            }
            else if (pPairVal > ePairVal)
            {
                return TieWinner.Player;
            }
            else
            {
                return TieWinner.Tie;
            }
        }
    }

    private bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(card => card.suit).Count() == 1;
    }

    private TieWinner BreakFlushOrHighCardTie(List<Card> PH, List<Card> EH)
    {
        // Check each card at same index for player and enemy to see which is higher
        // (hands are aleady sorted by ascending value)
        for (int i = 0; i < 5; i++)
        {
            int index = 4 - i;

            if (PH[index].value < EH[index].value)
            {
                return TieWinner.Enemy;
            }
            else if (PH[index].value > EH[index].value)
            {
                return TieWinner.Player;
            }
        }

        return TieWinner.Tie;        
    }

    private bool IsStraight(List<Card> hand)
    {
        var sortedValues = hand.Select(card => card.value).OrderBy(value => value).ToList();
        if (sortedValues.Last() == 14 && sortedValues.First() == 6)
        {
            // Handle A-6-7-8-9 as a valid straight (wheel)
            sortedValues.Remove(sortedValues.Last());
            sortedValues.Insert(0, 5);
        }

        for (int i = 1; i < sortedValues.Count; i++)
        {
            if (sortedValues[i] != sortedValues[i - 1] + 1)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsThreeOfAKind(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.value);
        return rankGroups.Any(group => group.Count() == 3);
    }

    private TieWinner BreakThreeOfAKindTie(List<Card> PH, List<Card> EH)
    {
        // Get three of a kind value for player and enemy
        int pToakVal = PH.GroupBy(card => card.value).Where(group => group.Count() == 3).SelectMany(group => group).ToList().First().value;
        int eToakVal = EH.GroupBy(card => card.value).Where(group => group.Count() == 3).SelectMany(group => group).ToList().First().value;

        // Higher three of a kind value wins, if same then check remaining cards
        if (pToakVal < eToakVal)
        {
            return TieWinner.Enemy;
        }
        else if (pToakVal > eToakVal)
        {
            return TieWinner.Player;
        }
        else
        {
            List<Card> pKickers = PH.Where(card => card.value != pToakVal).ToList();
            List<Card> eKickers = EH.Where(card => card.value != eToakVal).ToList();

            // Check remaining cards at same index for player and enemy to see which is higher
            // (hands are aleady sorted by ascending value)
            for (int i = 0; i < 2; i++)
            {
                int index = 1 - i;

                if (PH[index].value < EH[index].value)
                {
                    return TieWinner.Enemy;
                }
                else if (PH[index].value > EH[index].value)
                {
                    return TieWinner.Player;
                }
            }

            return TieWinner.Tie;
        }
    }

    private bool IsTwoPair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.value);
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }

    private TieWinner BreakTwoPairTie(List<Card> PH, List<Card> EH)
    {
        // Get 2 pairs in each hand (a list of 4 cards sorted by value)
        List<Card> pPairs = PH.GroupBy(card => card.value).Where(group => group.Count() == 2).SelectMany(group => group).ToList();
        List<Card> ePairs = EH.GroupBy(card => card.value).Where(group => group.Count() == 2).SelectMany(group => group).ToList();

        // High pair value will be at index 2 or 3, low pair value at 0 or 1
        if (pPairs[3].value < ePairs[3].value)
        {
            return TieWinner.Enemy;
        }
        else if (pPairs[3].value > ePairs[3].value)
        {
            return TieWinner.Player;
        }
        else if (pPairs[0].value < ePairs[0].value)
        {
            return TieWinner.Enemy;
        }
        else if (ePairs[0].value > ePairs[0].value)
        {
            return TieWinner.Player;
        }
        else
        {
            int pKicker = PH.GroupBy(card => card.value).Where(group => group.Count() == 1).SelectMany(group => group).ToList().First().value;
            int eKicker = EH.GroupBy(card => card.value).Where(group => group.Count() == 1).SelectMany(group => group).ToList().First().value;

            // Check remaing card value
            if (pKicker < eKicker)
            {
                return TieWinner.Enemy;
            }
            else if (pKicker > eKicker)
            {
                return TieWinner.Player;
            }
            else
            {
                return TieWinner.Tie;
            }
        }
    }

    private bool IsOnePair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.value);
        return rankGroups.Any(group => group.Count() == 2);
    }

    private TieWinner BreakOnePairTie(List<Card> PH, List<Card> EH)
    {
        // Get pair value for player and enemy
        int pPairVal = PH.GroupBy(card => card.value).Where(group => group.Count() == 2).SelectMany(group => group).ToList().First().value;
        int ePairVal = EH.GroupBy(card => card.value).Where(group => group.Count() == 2).SelectMany(group => group).ToList().First().value;

        // Higher pair value wins, if same then check remaining cards
        if (pPairVal < ePairVal)
        {
            return TieWinner.Enemy;
        }
        else if (pPairVal > ePairVal)
        {
            return TieWinner.Player;
        }
        else
        {
            List<Card> pKickers = PH.Where(card => card.value != pPairVal).ToList();
            List<Card> eKickers = EH.Where(card => card.value != ePairVal).ToList();

            // Check remaining cards at same index for player and enemy to see which is higher
            // (hands are aleady sorted by ascending value)
            for (int i = 0; i < 3; i++)
            {
                int index = 2 - i;

                if (PH[index].value < EH[index].value)
                {
                    return TieWinner.Enemy;
                }
                else if (PH[index].value > EH[index].value)
                {
                    return TieWinner.Player;
                }
            }
            return TieWinner.Tie;
        }
    }
}
