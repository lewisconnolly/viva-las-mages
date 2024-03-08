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
    [SerializeField] public List<Card> enemyHeldCards = new List<Card>();
    [SerializeField] public List<Card> selectedCards = new List<Card>();
    [SerializeField] public List<Card> selectedEnemyCards = new List<Card>();
    [SerializeField] public List<Card> playedCards = new List<Card>();
    [SerializeField] public List<Card> enemyPlayedCards = new List<Card>();
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

    public void SetEnemyCardPositionsOnTable()
    {
        CardPlacePoint[] placePoints = FindObjectsOfType<CardPlacePoint>();
        List<CardPlacePoint> enemyPlacePoints = new List<CardPlacePoint>();

        for (int i = 0; i < placePoints.Length; i++)
        {
            if (!placePoints[i].isPlayerPoint)
            {
                enemyPlacePoints.Add(placePoints[i]);
            }
        }

        List<CardPlacePoint> sortedEnemyPlacePoints = enemyPlacePoints.OrderBy(p => p.transform.position.x).ToList();

        for (int i = 0; i < selectedEnemyCards.Count; i++)
        {
            selectedEnemyCards[i].MoveToPoint(sortedEnemyPlacePoints[i].transform.position, minPos.rotation);
        }
    }

    //public void SetCardPositionsOnTable(bool isEnemyCard)
    //{
    //    CardPlacePoint[] allPlacePoints = FindObjectsOfType<CardPlacePoint>();
    //    List<CardPlacePoint> placePoints = new List<CardPlacePoint>();

    //    for (int i = 0; i < allPlacePoints.Length; i++)
    //    {
    //        if (placePoints[i].isPlayerPoint && !isEnemyCard)
    //        {
    //            placePoints.Add(placePoints[i]);
    //        }
    //        else if (!placePoints[i].isPlayerPoint && isEnemyCard)
    //        {
    //            placePoints.Add(placePoints[i]);
    //        }
    //    }

    //    List<CardPlacePoint> sortedPlacePoints = placePoints.OrderBy(p => p.transform.position.x).ToList();
    //    List<Card> cardsToMove = selectedCards;
        
    //    if (isEnemyCard)
    //    {
    //        cardsToMove = selectedEnemyCards;
    //    }

    //    for (int i = 0; i < cardsToMove.Count; i++)
    //    {
    //        cardsToMove[i].MoveToPoint(sortedPlacePoints[i].transform.position, minPos.rotation);
    //    }
    //}

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

    public void AddCardToEnemyHand(Card cardToAdd)
    {
        enemyHeldCards.Add(cardToAdd);
    }

    public void AddCardToTable(Card cardToAdd)
    {
        playedCards.Add(cardToAdd);
        SetCardPositionsOnTable();
    }

    public void AddEnemyCardToTable(Card cardToAdd)
    {
        enemyPlayedCards.Add(cardToAdd);
        SetEnemyCardPositionsOnTable();
    }

    public void SelectCard(Card cardToSelect)
    {
        selectedCards.Add(cardToSelect);
    }

    public void SelectEnemyCard(Card cardToSelect)
    {
        selectedEnemyCards.Add(cardToSelect);
    }

    public void SortSelectedCards()
    {
        List<Card> sortedSelectedCards = selectedCards.OrderBy(c => c.value).ToList();
        selectedCards = sortedSelectedCards;
    }

    public void SortEnemySelectedCards()
    {
        List<Card> sortedEnemySelectedCards = selectedEnemyCards.OrderBy(c => c.value).ToList();
        selectedEnemyCards = sortedEnemySelectedCards;
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

    public void SelectEnemyHand()
    {
        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, 4 - i);
            Card selectedCard = enemyHeldCards[r];
            SelectEnemyCard(selectedCard);
            enemyHeldCards.Remove(selectedCard);
        }
    }

    public void PlayEnemyHand()
    {
        SelectEnemyHand();
        SortEnemySelectedCards();

        foreach (Card card in selectedEnemyCards)
        {
            AddEnemyCardToTable(card);            
        }

        selectedEnemyCards.Clear();
    }
}
