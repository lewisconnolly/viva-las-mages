using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerCardController : MonoBehaviour
{
    public static PowerCardController instance;

    private void Awake()
    {
        instance = this;
    }

    public enum PowerCardType { None, Wildcard, FreeSwap, HandSwap }

    public Material wildcardMaterial;
    public Material freeSwapMaterial;
    public Material handSwapMaterial;

    public List<Card> EvaluatePowerCards(List<Card> hand)
    {
        List<Card> newHand = hand;
        
        // Change each power card based on its type
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].powerCardType == PowerCardType.Wildcard) { newHand = ChooseWildcardSuit(newHand, i); }
        }

        return newHand;
    }

    List<Card> ChooseWildcardSuit(List<Card> hand, int cardIndex)
    {
        List<Card> newHand = hand;
        HandEvaluator.HandRank handRank = HandEvaluator.instance.EvaluateHand(hand, false);

        // Wildcard only affects straight and high card hands
        if (handRank == HandEvaluator.HandRank.Straight ||
            handRank == HandEvaluator.HandRank.HighCard)
        {
            // Get the suit that appears the most in the hand
            var suitGroups = hand.GroupBy(card => card.suit).OrderByDescending(group => group.Count());
            string topSuit = suitGroups.First().ToList().First().suit;

            // Set suit of wildcard to be the top suit
            newHand[cardIndex].suit = topSuit;
        }

        return newHand;
    }
}
