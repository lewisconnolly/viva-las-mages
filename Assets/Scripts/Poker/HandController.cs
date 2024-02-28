using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
    public Transform minPos, maxPos;
    public List<Vector3> cardPositions = new List<Vector3>();
    public List<Vector3> cardTablePositions = new List<Vector3>();
    
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
            selectedCards[i].MoveToPoint(sortedPlayerPlacePoints[i].transform.position, minPos.rotation);            
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

        //SetCardPositionsInHand();
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

    public void SelectCard(Card cardToSelect)
    {
        selectedCards.Add(cardToSelect);
    }

    public void SortSelectedCards()
    {
        List<Card> sortedSelectedCards = selectedCards.OrderBy(c => c.value).ToList();
        selectedCards = sortedSelectedCards;
    }

    public void PlayHand()
    {
        foreach (Card card in selectedCards)
        {
            AddCardToTable(card);
            heldCards.Remove(card);
        }
            
        selectedCards.Clear();
        SetCardPositionsInHand();
    }
}
