using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BattleController;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] public List<Card> heldCards = new List<Card>();
    [SerializeField] public List<Card> selectedCards = new List<Card>();    
    [SerializeField] public List<Card> playedCards = new List<Card>();
    [SerializeField] public List<Card> swappedCards = new List<Card>();

    public Transform minPos, maxPos;
    public List<Vector3> cardPositions = new List<Vector3>();
    
    // Start is called before the first frame update
    void Start()
    {
        SetCardPositionsInHand();
    }

    // Update is called once per frame
    void Update()
    {
    }   

    // Display cards in front of player
    public void SetCardPositionsInHand()
    {
        cardPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;
        
        // Calculate space between cards
        if (heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxPos.position - minPos.position) / (heldCards.Count - 1);
        }

        // Evenly space cards based on minimum and maximum positions
        for (int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(minPos.position + (distanceBetweenPoints * i));

            heldCards[i].MoveToPoint(cardPositions[i], minPos.rotation);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }

    public void SetCardPositionsOnTable()
    {
        CardPlacePoint[] placePoints = FindObjectsOfType<CardPlacePoint>();
        List<CardPlacePoint> playerPlacePoints = new List<CardPlacePoint>();

        for (int i = 0; i < placePoints.Length; i++)
        {
            if (placePoints[i].isPlayerPoint)
            {
                playerPlacePoints.Add(placePoints[i]);
            }
        }

        List<CardPlacePoint> sortedPlayerPlacePoints = playerPlacePoints.OrderBy(p => p.transform.position.x).ToList();

        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].MoveToPoint(sortedPlayerPlacePoints[i].transform.position, minPos.rotation * Quaternion.Euler(0, 0, -45));            
        }
    }

    public void RemoveCardFromHand(Card cardToRemove)
    {
        // Check card to remove is at correct hand position
        if (heldCards[cardToRemove.handPosition] == cardToRemove)
        {        
            heldCards.RemoveAt(cardToRemove.handPosition);
        }
        else
        {
            Debug.LogError("Card at position " + cardToRemove.handPosition + " is not the card being removed from hand");            
        }
    }

    public void AddCardToHand(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        SetCardPositionsInHand();  
    }

    public void AddCardToTable(Card cardToAdd)
    {
        playedCards.Add(cardToAdd);
        SetCardPositionsOnTable();
    }

    public void SelectCard(Card cardToSelect) { selectedCards.Add(cardToSelect); }

    public void SortSelectedCards()
    {
        List<Card> sortedSelectedCards = selectedCards.OrderBy(c => c.value).ToList();
        selectedCards = sortedSelectedCards;
    }

    public void PlayHand()
    {
        foreach (Card card in selectedCards.OrderByDescending(c => c.handPosition).ToList())
        {
            AddCardToTable(card);
            RemoveCardFromHand(card);
        }
            
        selectedCards.Clear();
        SetCardPositionsInHand();
    }

    public void SwapCards()
    {
        // Store swapped cards to check when reshuffling deck (don't swap a card for currently selected)
        swappedCards.Clear();
        swappedCards.AddRange(selectedCards);
        
        foreach (Card card in selectedCards.OrderByDescending(c => c.handPosition).ToList())
        {
            // Remove selected cards from hand first to prevent being moved into hand when cards are drawn
            RemoveCardFromHand(card);
            // Move to discard position
            card.MoveToPoint(BattleController.instance.playerDiscardPosition.position, BattleController.instance.playerDiscardPosition.rotation);
        }

        // Damage player for amount of swapped cards - free swaps
        int numFreeSwaps = selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.FreeSwap).ToList().Count;        
        PlayerHealth.instance.TakeDamage(swappedCards.Count - numFreeSwaps);

        // Draw new cards
        DeckController.instance.DrawMultipleCards(selectedCards.Count);

        selectedCards.Clear();
    }

    public void SetTransparency(Card card, string action)
    {        
        // Get cards that should or should not be made transparent
        List<int> handPositionsToCheck = new List<int>();

        if (card.handPosition == 0) // If card is leftmost in hand, check card to right
        {
            handPositionsToCheck.Add(card.handPosition + 1);
        }
        else if (card.handPosition == heldCards.Count - 1) // If card is rightmost in hand, check card to left
        {
            handPositionsToCheck.Add(card.handPosition - 1);
        }
        else // If card in middle of hand, check left and right neighbours
        {
            handPositionsToCheck.Add(card.handPosition + 1);
            handPositionsToCheck.Add(card.handPosition - 1);
        }

        bool anyNeighbourNotSelected = false;
        bool anyNeighbourSelected = false;

        if (action == "mouse over" || action == "mouse exit" || action == "return")
        {
            foreach (int handPosition in handPositionsToCheck)
            {
                // Make a card transparent on mouse over, mouse exit, return to hand if any neighbour card is not selected 
                if (cardPositions.Count == BattleController.instance.startingCardsAmount &&
                    Mathf.Round(heldCards[handPosition].transform.position.x * 10) * 0.1 == Mathf.Round(cardPositions[0].x * 10) * 0.1)
                {
                    anyNeighbourNotSelected = true;
                }
            }

            if (anyNeighbourNotSelected)
            {
                heldCards[card.handPosition].MakeTransparent();
            }
        }
        else // action == "select"
        {
            foreach (int handPosition in handPositionsToCheck)
            {
                // Make a card transparent on select if any neighbour is selected
                if (heldCards[handPosition].isSelected)
                {
                    anyNeighbourSelected = true;
                }
            }

            if (anyNeighbourSelected)
            {
                heldCards[card.handPosition].MakeTransparent();
            }
        }
    }
}
