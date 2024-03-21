using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PowerCardController;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private List<CardScriptableObject> deckToUse = new List<CardScriptableObject>();  
    private List<CardScriptableObject> activeCards = new List<CardScriptableObject>();    
    public Card cardToSpawn;
    public float waitBetweenDrawingCards = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Shuffle deck into active cards
    public void SetUpDeck()
    {
        deckToUse = PlayerInventory.instance.playerDeck;

        // Don't shuffle in cards still in hand
        for (int i = 0; i < HandController.instance.heldCards.Count; i++)
        {
            for (int j = 0; j < deckToUse.Count; j++)
            {
                if (deckToUse[j].name == HandController.instance.heldCards[i].cardSO.name) { deckToUse.RemoveAt(j); }
            }            
        }

        // Don't shuffle in cards still that are being swapped 
        for (int i = 0; i < HandController.instance.swappedCards.Count; i++)
        {
            for (int j = 0; j < deckToUse.Count; j++)
            {
                if (deckToUse[j].name == HandController.instance.swappedCards[i].cardSO.name) { deckToUse.RemoveAt(j); }
            }
        }

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
        // Create a new card based on the card prefab
        Card newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
        // Use the next active card to set up the card to spawn
        newCard.cardSO = activeCards[0];
        newCard.SetUpCard();        
        // Remove used active card
        activeCards.RemoveAt(0);

        HandController.instance.AddCardToHand(newCard);
    }

    public void AddRewardCardtoDeck()
    {
        EnemyReward reward = FindObjectOfType<EnemyReward>();
        CardScriptableObject rewardCard = reward.GetRewardCard();

        int indexOfCardToReplace = 0;

        for (int i = 0; i < deckToUse.Count; i++)
        {
            if (deckToUse[i].value == rewardCard.value && deckToUse[i].suit == rewardCard.suit)
            {
                indexOfCardToReplace = i;
            }
        }

        deckToUse[indexOfCardToReplace] = rewardCard;
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
