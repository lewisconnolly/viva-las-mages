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

    public List<Vector3> cardHandPositions = new List<Vector3>();
    public List<Quaternion> cardHandRotations = new List<Quaternion>();
    public List<Vector3> cardTablePositions = new List<Vector3>();
    public float waitBetweenDrawingCards = 0.5f;
    public Transform minHandPos, maxHandPos;
    public Transform minTablePos, maxTablePos;

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();
    public Card cardToSpawn;
    public Transform drawFrom;

    private int numCardsRequiredToPlay = 5;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();
        //SetCardPositionsInHand();
        SetCardPositionsInHandCentered();
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

    //public void SetCardPositionsOnTable()
    //{
    //    CardPlacePoint[] placePoints = FindObjectsOfType<CardPlacePoint>();
    //    List<CardPlacePoint> enemyPlacePoints = new List<CardPlacePoint>();

    //    for (int i = 0; i < placePoints.Length; i++)
    //    {
    //        if (!placePoints[i].isPlayerPoint)
    //        {
    //            enemyPlacePoints.Add(placePoints[i]);
    //        }
    //    }

    //    List<CardPlacePoint> sortedEnemyPlacePoints = enemyPlacePoints.OrderBy(p => p.transform.position.x).ToList();

    //    for (int i = 0; i < selectedCards.Count; i++)
    //    {
    //        selectedCards[i].MoveToPoint(sortedEnemyPlacePoints[i].transform.position, selectedCards[i].transform.rotation * Quaternion.Euler(0, 0, -90));
    //    }
    //}

    public void SetCardPositionsInHandCentered()
    {
        cardHandPositions.Clear();
        cardHandRotations.Clear();

        float distanceBetweenPoints = (maxHandPos.position.z - minHandPos.position.z) / BattleController.instance.startingCardsAmount;
        Vector3 midHandPos = new Vector3(minHandPos.position.x, maxHandPos.position.y, minHandPos.position.z + (maxHandPos.position.z - minHandPos.position.z) / 2);

        // Shift mid point to right by half a card if even number of cards held
        midHandPos = new Vector3(midHandPos.x, midHandPos.y, midHandPos.z + distanceBetweenPoints / 2 * Mathf.Max(0, 1 - heldCards.Count % 2));

        // Start from mid point and move left times the number of cards
        Vector3 firstCardPos = new Vector3(midHandPos.x, midHandPos.y, midHandPos.z - distanceBetweenPoints * Mathf.Floor(heldCards.Count / 2));

        // Rotation lerp variables
        float startingRotation = 7.5f;
        float rotation = startingRotation;
        float t = 0;

        // Y offsets required because as each subsequent card is moved back to be under the previous, it appears higher up due to perspective
        // Additional first and last offsets required to create fan of cards
        float yOffset = 0.0f;
        float yOffsetFirstAndLast;
        float yPos;
        int j = 0;

        for (int i = 0; i < heldCards.Count; i++)
        {
            // Start moving cards down in Y after middle card
            if (i > Mathf.Ceil((heldCards.Count - 1) / 2))
            {
                yOffset = 0.0325f;
                j++;
            }

            if (i == 0 || i == heldCards.Count - 1) { yOffsetFirstAndLast = 0.0125f / 2; } else { yOffsetFirstAndLast = 0; }

            // Gradually move rotate cards in opposite direction
            rotation = Mathf.Lerp(rotation, -startingRotation, t);
            if (heldCards.Count > 1) { t += 1.0f / (float)(heldCards.Count - 1); }

            yPos = firstCardPos.y - yOffset * j - yOffsetFirstAndLast;

            cardHandPositions.Add(new Vector3(firstCardPos.x - 0.0125f * i, yPos, firstCardPos.z + distanceBetweenPoints * i));
            cardHandRotations.Add(minHandPos.rotation * Quaternion.Euler(0, rotation, 0));

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

        // Start from mid point and move left times the number of cards
        Vector3 firstCardPos = new Vector3(midTablePos.x, midTablePos.y, midTablePos.z - distanceBetweenPoints * Mathf.Floor(selectedCards.Count / 2));

        for (int i = 0; i < selectedCards.Count; i++)
        {
            cardTablePositions.Add(new Vector3(firstCardPos.x, firstCardPos.y, firstCardPos.z + distanceBetweenPoints * i));
            selectedCards[i].MoveToPoint(cardTablePositions[i], minHandPos.rotation * Quaternion.Euler(0, 0, -90));
        }
    }


    public void AddCardToHand(Card cardToAdd)
    {
        heldCards.Add(cardToAdd);
        //SetCardPositionsInHand();
        SetCardPositionsInHandCentered();
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
        //SetCardPositionsInHand();
        SetCardPositionsInHandCentered();
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
