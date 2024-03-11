using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }

    public HandRank EvaluateHand(List<Card> hand)
    {
        if (IsRoyalFlush(hand)) return HandRank.RoyalFlush;
        if (IsStraightFlush(hand)) return HandRank.StraightFlush;
        if (IsFourOfAKind(hand)) return HandRank.FourOfAKind;
        if (IsFullHouse(hand)) return HandRank.FullHouse;
        if (IsFlush(hand)) return HandRank.Flush;
        if (IsStraight(hand)) return HandRank.Straight;
        if (IsThreeOfAKind(hand)) return HandRank.ThreeOfAKind;
        if (IsTwoPair(hand)) return HandRank.TwoPair;
        if (IsOnePair(hand)) return HandRank.OnePair;
        return HandRank.HighCard;
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
