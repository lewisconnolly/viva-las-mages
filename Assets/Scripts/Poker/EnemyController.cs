using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

        Vector3 distanceBetweenPoints = (maxHandPos.position - minHandPos.position) / BattleController.instance.startingCardsAmount;
        Vector3 midHandPos = new Vector3(minHandPos.position.x, maxHandPos.position.y, minHandPos.position.z + (maxHandPos.position.z - minHandPos.position.z) / 2);

        // Shift mid point to right by half a card if even number of cards held
        midHandPos = new Vector3(midHandPos.x, midHandPos.y, midHandPos.z) + distanceBetweenPoints / 2 * Mathf.Max(0, 1 - heldCards.Count % 2);

        // Start from mid point and move left times the number of cards        
        Vector3 firstCardPos = new Vector3(midHandPos.x, midHandPos.y, midHandPos.z) - distanceBetweenPoints * Mathf.Floor(heldCards.Count / 2);

        Vector3 cardPos = firstCardPos;
        Quaternion cardRotation;

        // Rotate cards in x to prevent z-fighting
        float startingFanAngle = 0.4f;
        float fanAngle;

        // Rotation lerp variables
        float startingRotation = 7.5f;
        float rotation = startingRotation;
        float t2 = 0;

        for (int i = 0; i < heldCards.Count; i++)
        {
            // Gradually offset cards in x and y to create fan. Offset first card by more so that it looks right
            if (i == 0)
            {
                cardPos = new Vector3(firstCardPos.x + 0.009f, firstCardPos.y - 0.025f * Mathf.Pow(1.5f, (float)i), firstCardPos.z) + distanceBetweenPoints * i;
            }
            else
            {
                cardPos = new Vector3(firstCardPos.x - 0.0075f * i, firstCardPos.y - 0.0075f * Mathf.Pow(1.5f, (float)i), firstCardPos.z) + distanceBetweenPoints * i;
            }

            // Gradually rotate cards in opposite direction and x-axis
            rotation = Mathf.Lerp(rotation, -startingRotation, t2);
            fanAngle = startingFanAngle + i * 0.2f;
            cardRotation = minHandPos.rotation * Quaternion.Euler(fanAngle, rotation, 0);

            // Increase interpolation degree
            if (heldCards.Count > 1) { t2 += 1.0f / (float)(heldCards.Count - 1); }

            cardHandPositions.Add(cardPos);
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
        List<List<Card>> handCombinations = GenerateSelections(heldCards);
        List<int> handRanks = new List<int>(new int[handCombinations.Count]);
        
        List<List<Card>> GenerateSelections(List<Card> cardsInHand)
        {
            // Make an array to tell whether
            // an item is in the current selection.
            bool[] inSelection = new bool[cardsInHand.Count];

            // Make a result list.
            List<List<Card>> handCombinations = new List<List<Card>>();

            // Build the combinations recursively.
            SelectItems(cardsInHand, inSelection, handCombinations, numCardsRequiredToPlay, 0);

            // Return the results.
            return handCombinations;
        }

        // Recursively select n additional items with indexes >= firstItem.
        // If n == 0, add the current combination to the results.
        void SelectItems(List<Card> cardsInHand, bool[] inSelection, List<List<Card>> handCombinations, int numCards, int firstItem)
        {
            if (numCards == 0)
            {
                // Add the current selection to the results.
                List<Card> selection = new List<Card>();
                for (int i = 0; i < cardsInHand.Count; i++)
                {
                    // If this item is selected, add it to the selection.
                    if (inSelection[i]) selection.Add(cardsInHand[i]);
                }
                handCombinations.Add(selection);
            }
            else
            {
                // Try adding each of the remaining items.
                for (int i = firstItem; i < cardsInHand.Count; i++)
                {
                    // Try adding this item.
                    inSelection[i] = true;

                    // Recursively add the rest of the required items.
                    SelectItems(cardsInHand, inSelection, handCombinations, numCards - 1, i + 1);

                    // Remove this item from the selection.
                    inSelection[i] = false;
                }
            }
        }

        for (int i = 0; i < handCombinations.Count; i++)
        {
            //int card1i = heldCards.IndexOf(handCombinations[i][0]);
            //int card2i = heldCards.IndexOf(handCombinations[i][1]);
            //int card3i = heldCards.IndexOf(handCombinations[i][2]);
            //int card4i = heldCards.IndexOf(handCombinations[i][3]);
            //int card5i = heldCards.IndexOf(handCombinations[i][4]);

            //Debug.Log(i + ": " + card1i + card2i + card3i + card4i + card5i);

            var handRank = HandEvaluator.instance.EvaluateHand(handCombinations[i], true);
            handRanks[i] = (int)handRank;
        }

        int handIndex = handRanks.IndexOf(handRanks.Max());

        for (int i = 0; i < handCombinations[handIndex].Count; i++)
        {
            SelectCard(handCombinations[handIndex][i]);
            heldCards.Remove(handCombinations[handIndex][i]);
        }

        //for (int i = 0; i < 5; i++)
        //{
        //    int r = Random.Range(0, 4 - i);
        //    Card selectedCard = heldCards[r];
        //    SelectCard(selectedCard);
        //    heldCards.Remove(selectedCard);
        //} 
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
