using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleController;

public class PowerCardController : MonoBehaviour
{
    public static PowerCardController instance;

    private void Awake()
    {
        instance = this;
    }

    public enum PowerCardType { None, Wildcard, FreeSwap, HandSwap, HalfClubs, HalfSpades, HalfHearts, HalfDiamonds }

    public Material noneMaterial;
    public Material wildcardMaterial;
    public Material freeSwapMaterial;
    public Material handSwapMaterial;
    public Material halfClubsMaterial;
    public Material halfSpadesMaterial;
    public Material halfHeartsMaterial;
    public Material halfDiamondsMaterial;

    public List<Card> EvaluatePowerCards(List<Card> hand)
    {
        List<Card> newHand = hand;
        
        // Change each power card based on its type
        for (int i = 0; i < hand.Count; i++)
        {
            // Wildcards
            if (hand[i].powerCardType == PowerCardType.Wildcard) { newHand = ChooseWildcardSuit(newHand, i); }

            // Half and half cards
            if (hand[i].powerCardType == PowerCardType.HalfClubs || hand[i].powerCardType == PowerCardType.HalfSpades ||
                hand[i].powerCardType == PowerCardType.HalfHearts || hand[i].powerCardType == PowerCardType.HalfDiamonds)
            {
                newHand = ChooseHalfAndHalfSuit(newHand, i);
            }
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

    List<Card> ChooseHalfAndHalfSuit(List<Card> hand, int cardIndex)
    {
        List<Card> newHand = hand;
        HandEvaluator.HandRank handRank = HandEvaluator.instance.EvaluateHand(hand, false);

        // Half and half only affects straight and high card hands
        if (handRank == HandEvaluator.HandRank.Straight ||
            handRank == HandEvaluator.HandRank.HighCard)
        {
            // Get the suit that appears the most in the hand
            var suitGroups = hand.GroupBy(card => card.suit).OrderByDescending(group => group.Count());
            string topSuit = suitGroups.First().ToList().First().suit;
            
            // Get added suit
            string addedSuit = newHand[cardIndex].powerCardType.ToString().Replace("Half","");

            // If the original suit of the card isn't the top suit already and the added suit is the same as top suit, make the original suit the top suit
            if (newHand[cardIndex].suit != topSuit && addedSuit == topSuit)
            {
                newHand[cardIndex].suit = topSuit; // suit will be reset back to original on cardSO when card is drawn again
            }
        }

        return newHand;
    }
}
