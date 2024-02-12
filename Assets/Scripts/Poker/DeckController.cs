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

    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();

    public Card cardToSpawn;

    public int drawCardCost = 1;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();
        DrawCardToHand();
        DrawCardToHand();
        DrawCardToHand();
        DrawCardToHand();
        DrawCardToHand();
        DrawCardToHand();
        DrawCardToHand();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DrawCardToHand();
        }
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

    public void SwapCardsForHearts()
    {

    }
}
