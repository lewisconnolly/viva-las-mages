using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static BattleController;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    public AK.Wwise.Event CardSound;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] public List<Card> heldCards = new List<Card>();
    [SerializeField] public List<Card> selectedCards = new List<Card>();
    [SerializeField] public List<Card> playedCards = new List<Card>();
    [SerializeField] public List<Card> swappedCards = new List<Card>();

    public Transform minHandPos, maxHandPos;
    public Transform minTablePos, maxTablePos;
    public List<Vector3> cardHandPositions = new List<Vector3>();
    public List<Quaternion> cardHandRotations = new List<Quaternion>();
    public List<Vector3> cardTablePositions = new List<Vector3>();

    public int numCardsRequiredToPlay = 5;

    Camera mainCam;
    public AnimationCurve cardFanWidthCurve;
    public AnimationCurve cardFanHeightCurve;
    public AnimationCurve cardFanRotationCurve;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Display cards in front of player
    public void SetCardPositionsInHand()
    {
        cardHandPositions.Clear();

        Vector3 distanceBetweenPoints = Vector3.zero;

        // Calculate space between cards
        if (heldCards.Count > 1)
        {
            distanceBetweenPoints = (maxHandPos.position - minHandPos.position) / (heldCards.Count - 1);
        }

        // Evenly space cards based on minimum and maximum positions
        for (int i = 0; i < heldCards.Count; i++)
        {
            cardHandPositions.Add(minHandPos.position + (distanceBetweenPoints * i));

            heldCards[i].MoveToPoint(cardHandPositions[i], minHandPos.rotation);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }

    public void SetCardPositionsInHandCentered(bool justPlayedHand)
    {
        cardHandPositions.Clear();
        cardHandRotations.Clear();
        
        Vector3 startingPos = mainCam.WorldToViewportPoint(minHandPos.position);
        Vector3 cardPos = startingPos;
        Quaternion startingRotation = minHandPos.rotation;
        Quaternion cardRotation;

        for (int i = 0; i < heldCards.Count; i++)
        {
            float handRatio = 0.0f;

            if (heldCards.Count > 1) { handRatio = (float)i / ((float)heldCards.Count - 1); }

            cardPos.x = startingPos.x + cardFanWidthCurve.Evaluate(handRatio) * 0.65f;
            cardPos.y = startingPos.y + cardFanHeightCurve.Evaluate(handRatio) * 0.05f;
            cardPos.z = startingPos.z - cardFanWidthCurve.Evaluate(handRatio) * 0.05f;
            cardRotation = startingRotation * Quaternion.Euler(0, cardFanRotationCurve.Evaluate(handRatio) * 10f, 0);
            
            if (justPlayedHand) { cardPos += new Vector3(0, -0.2f, 0); }

            cardHandPositions.Add(mainCam.ViewportToWorldPoint(cardPos));
            cardHandRotations.Add(cardRotation);

            heldCards[i].MoveToPoint(cardHandPositions[i], cardHandRotations[i]);

            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
        }
    }

    public void SetCardPositionsOnTable()
    {
        cardTablePositions.Clear();

        float distanceBetweenPoints = (maxTablePos.position.z - minTablePos.position.z) / numCardsRequiredToPlay;
        Vector3 midTablePos = new Vector3(minTablePos.position.x, minTablePos.position.y, minTablePos.position.z + (maxTablePos.position.z - minTablePos.position.z) / 2);

        // Shift mid point to right by half a card if number of cards being played is even
        midTablePos = new Vector3(midTablePos.x, midTablePos.y, midTablePos.z + distanceBetweenPoints / 2 * Mathf.Max(0, 1 - selectedCards.Count % 2));

        // Start from mid point and move left times the number of cards /2
        Vector3 firstCardPos = new Vector3(midTablePos.x, midTablePos.y, midTablePos.z - distanceBetweenPoints * Mathf.Floor(selectedCards.Count / 2));

        for (int i = 0; i < selectedCards.Count; i++)
        {
            cardTablePositions.Add(new Vector3(firstCardPos.x, firstCardPos.y, firstCardPos.z + distanceBetweenPoints * i));
            selectedCards[i].MoveToPoint(cardTablePositions[i], minHandPos.rotation * Quaternion.Euler(0, 0, -45));
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
        SetCardPositionsInHandCentered(false);
        CardSound.Post(gameObject);
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
        SetCardPositionsInHandCentered(true);
    }

    public void SwapCards()
    {
        int numHandSwaps = selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.HandSwap).ToList().Count;
        
        // If swapping whole hand, add unselected cards to selected and don't take damage
        if (numHandSwaps > 0)
        {
            foreach (Card card in heldCards) { if (!card.isSelected) { SelectCard(card); } }            
        }
        else
        {
            // Damage player for amount of swapped cards - free swaps
            int numFreeSwaps = selectedCards.Where(card => card.powerCardType == PowerCardController.PowerCardType.FreeSwap).ToList().Count;
                        
            PlayerHealth.instance.TakeDamage(selectedCards.Count - numFreeSwaps);
        }
                
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
                if (cardHandPositions.Count == BattleController.instance.startingCardsAmount &&
                    Mathf.Round(heldCards[handPosition].transform.position.x * 10) * 0.1 == Mathf.Round(cardHandPositions[0].x * 10) * 0.1)
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
