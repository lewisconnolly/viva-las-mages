using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();
    
    public List<CardScriptableObject> enemyDeckToUse = new List<CardScriptableObject>();

    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();
    
    private List<CardScriptableObject> enemyActiveCards = new List<CardScriptableObject>();

    public Card cardToSpawn;

    public int drawCardCost = 1;

    public float waitBetweenDrawingCards = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();
        SetUpEnemyDeck();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Shuffle deck into active cards
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

    // Shuffle deck into enemy active cards
    public void SetUpEnemyDeck()
    {
        enemyActiveCards.Clear();

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(enemyDeckToUse);

        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            enemyActiveCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            iterations++;
        }
    }

    public void DrawCardToHand()
    {
        if (activeCards.Count == 0)
        {
            SetUpDeck();
        }

        // Create a new card based on the card prefab
        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        // Use the next active card to set up the card to spawn
        newCard.cardSO = activeCards[0];
        newCard.SetUpCard();        
        // Remove used active card
        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);
    }

    public void DrawEnemyCards(int amountToDraw)
    {
        for (int i = 0; i < amountToDraw; i++)
        {
            Card newCard = Instantiate(cardToSpawn, transform.position + new Vector3(0, -5f, 0), transform.rotation);
            newCard.cardSO = enemyActiveCards[0];
            newCard.SetUpCard();
            // Remove used active card
            enemyActiveCards.RemoveAt(0);

            HandController.instance.AddCardToEnemyHand(newCard);
        }
    }

    public void SwapCardsForHearts()
    {

    }

    public void DrawMultipleCards(int amountToDraw)
    {
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
