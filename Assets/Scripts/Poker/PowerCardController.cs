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

    public enum PowerCardType { None, Wildcard, FreeSwap, HandSwap, TwoInOneClubs, TwoInOneSpades, TwoInOneHearts, TwoInOneDiamonds, Duplicard, RankUpgrade, Health }
    
    public Material noneMaterial;
    public Material wildcardMaterial;
    public Material freeSwapMaterial;
    public Material handSwapMaterial;
    public Material halfClubsMaterial;
    public Material halfSpadesMaterial;
    public Material halfHeartsMaterial;
    public Material halfDiamondsMaterial;
    public Material autoPairMaterial;
    public Material upgradeRankMaterial;
    public Material gainHeartMaterial;

    public Transform autoPairSpawnPosition;

    public int numRanksToUpgrade;
    public int numHeartsToGain;

    public List<Card> EvaluatePowerCards(List<Card> hand)
    {
        List<Card> newHand = hand;
        numRanksToUpgrade = 0;
        numHeartsToGain = 0;

        // Change each power card based on its type
        // Separate loop for each type because power cards can be stacked/act multiplicatively

        // Auto pairs first because they change number of each suit in hand,
        // so will change what suits wild card and half and half become
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].powerCardType == PowerCardType.Duplicard) { newHand = DuplicateAutoPair(newHand, i); }
        }

        // Half and half cards next as more restricted in which suit they can choose
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].powerCardType == PowerCardType.TwoInOneClubs || hand[i].powerCardType == PowerCardType.TwoInOneSpades ||
                hand[i].powerCardType == PowerCardType.TwoInOneHearts || hand[i].powerCardType == PowerCardType.TwoInOneDiamonds)
            {
                newHand = ChooseHalfAndHalfSuit(newHand, i);
            }
        }

        // Wildcards
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].powerCardType == PowerCardType.Wildcard) { newHand = ChooseWildcardSuit(newHand, i); }
        }

        // Determine number of ranks to upgrade and hearts to gain
        for (int i = 0; i < hand.Count; i++)
        {
            if (hand[i].powerCardType == PowerCardType.RankUpgrade) { numRanksToUpgrade++; }
            if (hand[i].powerCardType == PowerCardType.Health) { numHeartsToGain++; }
        }

        return newHand;
    }

    List<Card> ChooseWildcardSuit(List<Card> hand, int cardIndex)
    {
        List<Card> newHand = hand;
        //HandEvaluator.HandRank handRank = HandEvaluator.instance.EvaluateHand(hand, false);

        // Wildcard only affects straight and high card hands
        //if (handRank == HandEvaluator.HandRank.Straight ||
        //    handRank == HandEvaluator.HandRank.HighCard)
        //{
        // Get the suit that appears the most in the hand
        var suitGroups = hand.GroupBy(card => card.suit).OrderByDescending(group => group.Count());
        string topSuit = suitGroups.First().ToList().First().suit;

        // Set suit of wildcard to be the top suit
        newHand[cardIndex].suit = topSuit;
        //}

        return newHand;
    }

    List<Card> ChooseHalfAndHalfSuit(List<Card> hand, int cardIndex)
    {
        List<Card> newHand = hand;
        //HandEvaluator.HandRank handRank = HandEvaluator.instance.EvaluateHand(hand, false);

        // Half and half only affects straight and high card hands
        //if (handRank == HandEvaluator.HandRank.Straight ||
        //    handRank == HandEvaluator.HandRank.HighCard)
        //{
        // Get the suit that appears the most in the hand
        var suitGroups = hand.GroupBy(card => card.suit).OrderByDescending(group => group.Count());
        string topSuit = suitGroups.First().ToList().First().suit;

        // Get added suit
        string addedSuit = newHand[cardIndex].powerCardType.ToString().Replace("TwoInOne", "");

        // If the original suit of the card isn't the top suit already and the added suit is the same as top suit, make the original suit the top suit
        if (newHand[cardIndex].suit != topSuit && addedSuit == topSuit)
        {
            newHand[cardIndex].suit = topSuit; // suit will be reset back to original on cardSO when card is drawn again
        }
        //}

        return newHand;
    }

    List<Card> DuplicateAutoPair(List<Card> hand, int cardIndex)
    {
        List<Card> newHand = hand;

        // Duplicate auto pair card and add to hand
        // (not a member of heldCards, selectedCards, or playedCards so won't be destroyed until scene unloaded and has no game object)
        Card duplicateCard = Instantiate(DeckController.instance.cardToSpawn, autoPairSpawnPosition.position, autoPairSpawnPosition.rotation);
        duplicateCard.value = newHand[cardIndex].value;
        duplicateCard.suit = newHand[cardIndex].suit;
        duplicateCard.powerCardType = PowerCardType.None;
        duplicateCard.isDuplicate = true;

        newHand.Add(duplicateCard);

        return newHand;
    }

    public void DestroyDuplicates()
    {
        Card[] cards = FindObjectsOfType<Card>();
        foreach (Card card in cards)
        {
            if (card.isPlayer && card.isDuplicate)
            {
                Destroy(card);
            }
        }        
    }
}
