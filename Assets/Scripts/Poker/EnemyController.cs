using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] public List<Card> heldCards = new List<Card>();
    [SerializeField] public List<Card> selectedCards = new List<Card>();
    [SerializeField] public List<Card> playedCards = new List<Card>();
    public List<Vector3> cardPositions = new List<Vector3>();
    public float waitBetweenDrawingCards = 0.5f;
    public Transform minPos, maxPos;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();
    public Card cardToSpawn;
    public Transform drawFrom;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();
        SetCardPositionsInHand();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpDeck()
    {
        activeCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            iterations++;
        }
    }    

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
        List<CardPlacePoint> enemyPlacePoints = new List<CardPlacePoint>();

        for (int i = 0; i < placePoints.Length; i++)
        {
            if (!placePoints[i].isPlayerPoint)
            {
                enemyPlacePoints.Add(placePoints[i]);
            }
        }

        List<CardPlacePoint> sortedEnemyPlacePoints = enemyPlacePoints.OrderBy(p => p.transform.position.x).ToList();

        for (int i = 0; i < selectedCards.Count; i++)
        {
            selectedCards[i].MoveToPoint(sortedEnemyPlacePoints[i].transform.position, Quaternion.Euler(180, 0, 0));
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

    public void SelectCard(Card cardToSelect)
    {
        selectedCards.Add(cardToSelect);
    }

    public void SortSelectedCards()
    {
        List<Card> sortedEnemySelectedCards = selectedCards.OrderBy(c => c.value).ToList();
        selectedCards = sortedEnemySelectedCards;
    }

    public void SelectHand()
    {
        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, 4 - i);
            Card selectedCard = heldCards[r];
            SelectCard(selectedCard);
            heldCards.Remove(selectedCard);
        }
    }

    public void PlayHand()
    {
        SelectHand();
        SortSelectedCards();

        foreach (Card card in selectedCards)
        {
            AddCardToTable(card);
            heldCards.Remove(card);
        }

        selectedCards.Clear();
        SetCardPositionsInHand();
    }
    public void DrawCardToHand()
    {        
        // Create a new card based on the card prefab
        Card newCard = Instantiate(cardToSpawn, drawFrom.position, drawFrom.rotation);
        // Use the next active card to set up the card to spawn
        newCard.cardSO = activeCards[0];
        newCard.isPlayer = false;
        newCard.SetUpCard();        
        // Remove used active card
        activeCards.RemoveAt(0);

        AddCardToHand(newCard);
    }

    public void DrawMultipleCards(int amountToDraw)
    {
        if (activeCards.Count < amountToDraw)
        {
            SetUpDeck();
        }

        StartCoroutine(DrawMultipleCo(amountToDraw));
    }

    // Draw multiple cards coroutine
    IEnumerator DrawMultipleCo(int amountToDraw)
    {        
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand();

            // Stop loop and suspend coroutine for wait time
            yield return new WaitForSeconds(waitBetweenDrawingCards);
        }
    }
}
