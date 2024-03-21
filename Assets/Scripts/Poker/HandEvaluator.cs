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

    public string CheckTie(List<Card> playerHand, List<Card> enemyHand)
    {
        HandRank rank = EvaluateHand(playerHand, false);

        if (rank == HandRank.RoyalFlush) return "Tie";
        
        if (rank == HandRank.StraightFlush || rank == HandRank.Straight)
        {
            var sortedPlayerValues = playerHand.Select(card => card.value).OrderBy(value => value).ToList();
            var sortedEnemyValues = enemyHand.Select(card => card.value).OrderBy(value => value).ToList();
            
            if (sortedPlayerValues.Last() == 14 && sortedPlayerValues.First() == 6)
            {
                // Handle A-6-7-8-9 as a valid straight (wheel)
                sortedPlayerValues.Remove(sortedPlayerValues.Last());
                sortedPlayerValues.Insert(0, 5);
            }

            if (sortedEnemyValues.Last() == 14 && sortedEnemyValues.First() == 6)
            {
                // Handle A-6-7-8-9 as a valid straight (wheel)
                sortedEnemyValues.Remove(sortedEnemyValues.Last());
                sortedEnemyValues.Insert(0, 5);
            }

            if (sortedPlayerValues.Last() > sortedEnemyValues.Last())
            {
                return "Player Wins";
            }
            else if (sortedPlayerValues.Last() < sortedEnemyValues.Last())
            {
                return "Enemy Wins";
            }
            else
            {
                return "Tie";
            }
        }

        int playerFourOfAKindValue = 0;
        int playerKickerValue = 0;
        int enemyFourOfAKindValue = 0;
        int enemyKickerValue = 0;

        if (rank == HandRank.FourOfAKind)
        {
            var valueGroups = playerHand.GroupBy(card => card.value);
            
            foreach (var group in valueGroups)
            {
                if (group.Count() == 4)
                {
                    playerFourOfAKindValue = group.ToList().Select(card => card.value).ToList().First();
                }
                else
                {
                    playerKickerValue = group.ToList().Select(card => card.value).ToList().First();
                }
            }

            valueGroups = enemyHand.GroupBy(card => card.value);

            foreach (var group in valueGroups)
            {
                if (group.Count() == 4)
                {
                    enemyFourOfAKindValue = group.ToList().Select(card => card.value).ToList().First();
                }
                else
                {
                    enemyKickerValue = group.ToList().Select(card => card.value).ToList().First();
                }
            }

            if (playerFourOfAKindValue > enemyFourOfAKindValue)
            {
                return "Player Wins";
            }
            else if (playerFourOfAKindValue < enemyFourOfAKindValue)
            {
                return "Enemy Wins";
            }
            else
            {
                if (playerKickerValue > enemyKickerValue)
                {
                    return "Player Wins";
                }
                else if (playerKickerValue < enemyKickerValue)
                {
                    return "Enemy Wins";
                }
                else
                {
                    return "Tie";
                }
            }
        }

        return "Tie";
    }

    private bool IsRoyalFlush(List<Card> hand)
    {
        return IsStraightFlush(hand) && hand.All(card => card.value >= 10);
    }

    private bool IsStraightFlush(List<Card> hand)
    {
        return IsFlush(hand) && IsStraight(hand);
    }

    private bool IsFourOfAKind(List<Card> hand)
    {
        var valueGroups = hand.GroupBy(card => card.value);
        return valueGroups.Any(group => group.Count() == 4);
    }

    private bool IsFullHouse(List<Card> hand)
    {
        var valueGroups = hand.GroupBy(card => card.value);
        return valueGroups.Any(group => group.Count() == 3) && valueGroups.Any(group => group.Count() == 2);
    }

    private bool IsFlush(List<Card> hand)
    {
        return hand.GroupBy(card => card.suit).Count() == 1;
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

    private bool IsTwoPair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.value);
        return rankGroups.Count(group => group.Count() == 2) == 2;
    }

    private bool IsOnePair(List<Card> hand)
    {
        var rankGroups = hand.GroupBy(card => card.value);
        return rankGroups.Any(group => group.Count() == 2);
    }
}
