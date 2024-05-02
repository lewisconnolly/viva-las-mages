using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PowerCardController;
using UnityEngine.XR;

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

    public int pcntChanceOfRandomHand = 50;

    Camera mainCam;
    public AnimationCurve cardFanWidthCurve;
    public AnimationCurve cardFanHeightCurve;
    public AnimationCurve cardFanRotationCurve;

    // Start is called before the first frame update
    void Start()
    {
        SetUpDeck();

        pcntChanceOfRandomHand = BattleController.instance.activeEnemy.pcntChanceOfRandomHand;

        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUpDeck()
    {
        List<CardScriptableObject> deckToDeal = deckToUse;

        // Don't shuffle in cards still in hand
        for (int i = 0; i < heldCards.Count; i++)
        {
            for (int j = 0; j < deckToDeal.Count; j++)
            {
                if (deckToDeal[j].name == heldCards[i].cardSO.name) { deckToDeal.RemoveAt(j); }
            }
        }

        activeCards.Clear();

        // Add reward card to enemy deck
        //for (int i = 0; i < deckToDeal.Count; i++)
        //{
        //    if (deckToDeal[i].value == RewardCardUI.instance.rewardCard.cardSO.value &&
        //        deckToDeal[i].suit == RewardCardUI.instance.rewardCard.cardSO.suit)
        //    {
        //        deckToDeal[i].powerCardType = RewardCardUI.instance.rewardCard.cardSO.powerCardType;
        //    }
        //    else
        //    {
        //        deckToDeal[i].powerCardType = PowerCardController.PowerCardType.None;
        //    }
        //}

        List<CardScriptableObject> tempDeck = new List<CardScriptableObject>();
        tempDeck.AddRange(deckToDeal);

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

    public void SetCardPositionsInHandCentered()
    {
        cardHandPositions.Clear();
        cardHandRotations.Clear();

        Vector3 startingPos = mainCam.WorldToViewportPoint(maxHandPos.position);
        Vector3 cardPos = startingPos;
        Quaternion startingRotation = maxHandPos.rotation;
        Quaternion cardRotation;

        for (int i = 0; i < heldCards.Count; i++)
        {
            float handRatio = 0.0f;

            if (heldCards.Count > 1) { handRatio = (float)i / ((float)heldCards.Count - 1); }

            cardPos.x = startingPos.x + cardFanWidthCurve.Evaluate(handRatio) * 0.285f;
            cardPos.y = startingPos.y + cardFanHeightCurve.Evaluate(handRatio) * 0.025f;
            cardPos.z = startingPos.z - cardFanWidthCurve.Evaluate(handRatio) * 0.125f;
            cardRotation = startingRotation * Quaternion.Euler(0, cardFanRotationCurve.Evaluate(handRatio) * -10f, 0);

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
        selectedCards = new List<Card>();

        int randomNumToTest = Random.Range(1, 101);

        List<Card> duplicards = new List<Card>();
        List<Card> handToCheck = heldCards;
        // Duplicate duplicards before checking then add back in later
        for (int i = 0; i < heldCards.Count; i++)
        {
            if (heldCards[i].powerCardType == PowerCardType.Duplicard)
            {
                duplicards.Add(heldCards[i]);
                handToCheck = ReplaceDuplicard(handToCheck, heldCards[i]);
            }
        }

        List<List<Card>> handCombinations = GenerateSelections(handToCheck);

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

        List<List<Card>> combinationsToRank = handCombinations;
        // Remove hand combinations that use only one of the duplicated duplicards (invalid)
        if (duplicards.Count > 0)
        {
            combinationsToRank = FilterCombinations(duplicards, combinationsToRank);
        }

        List<int> handRanks = new List<int>(new int[combinationsToRank.Count]);
        // Rank each hand combination
        for (int i = 0; i < combinationsToRank.Count; i++)
        {
            var handRank = HandEvaluator.instance.EvaluateHand(combinationsToRank[i], true);
            handRanks[i] = (int)handRank;
        }

        int handIndex;
        if (randomNumToTest > pcntChanceOfRandomHand)
        {
            // Play hand with highest rank
            handIndex = handRanks.IndexOf(handRanks.Max());
        }
        else
        {
            // Play random hand
            handIndex = Random.Range(0, handRanks.Count);
        }

        List<Card> cardsToSelect = combinationsToRank[handIndex];
        // Check for duplicards in chosen hand
        if (duplicards.Count > 0)
        {
            // Deduplicate duplicated duplicards if in chosen hand
            cardsToSelect = ReAddDuplicard(duplicards, cardsToSelect);
        }

        for (int i = 0; i < cardsToSelect.Count; i++)
        {
            selectedCards.Add(cardsToSelect[i]);
            heldCards.Remove(cardsToSelect[i]);
        }
    }

    List<Card> ReplaceDuplicard(List<Card> hand, Card card)
    {
        List<Card> newHand = hand;

        // Duplicate auto pair card and add to hand
        // (not a member of heldCards, selectedCards, or playedCards so won't be destroyed until scene unloaded and has no game object)
        Card duplicateCard = Instantiate(DeckController.instance.cardToSpawn, PowerCardController.instance.autoPairSpawnPosition.position, PowerCardController.instance.autoPairSpawnPosition.rotation);
        duplicateCard.isPlayer = false;
        duplicateCard.value = card.value;
        duplicateCard.suit = card.suit;
        duplicateCard.powerCardType = PowerCardType.None;

        // Remove power up from original
        int index = newHand.IndexOf(newHand.Where(duplicard => duplicard.value == card.value && duplicard.suit == card.suit).ToList().First());
        newHand[index].powerCardType = PowerCardType.None;

        // Add duplicate        
        newHand.Add(duplicateCard);

        return newHand;
    }

    List<Card> ReAddDuplicard(List<Card> duplicards, List<Card> handToCheck)
    {
        List<Card> newHand = new List<Card>();

        List<Card> toRemove = new List<Card>();

        for (int i = 0; i < duplicards.Count; i++)
        {
            if (handToCheck.Where(card => card.suit == duplicards[i].suit && card.value == duplicards[i].value).ToList().Count > 0)
            {
                toRemove.Add(duplicards[i]);
            }
        }

        if (toRemove.Count == 0)
        {
            return handToCheck;
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            // Remove all duplicard copies
            newHand = handToCheck.Where(card => card.suit != toRemove[i].suit || card.value != toRemove[i].value).ToList();

            // Add back in one of duplicard
            toRemove[i].powerCardType = PowerCardType.Duplicard;
            newHand.Add(toRemove[i]);
        }

        return newHand;
    }

    List<List<Card>> FilterCombinations(List<Card> duplicards, List<List<Card>> combinations)
    {
        List<List<Card>> newCombinations = new List<List<Card>> { };

        // For each combination
        for (int i = 0; i < combinations.Count; i++)
        {
            bool isValid = true;

            // Check combination for duplicards
            for (int j = 0; j < duplicards.Count; j++)
            {
                // Get duplicards in hand
                List<Card> duplicardsInHand = combinations[i].Where(card => card.suit == duplicards[j].suit && card.value == duplicards[j].value).ToList();

                if (duplicardsInHand.Count > 0)
                {
                    // If duplicard in hand but only one, then hand is invalid
                    if (duplicardsInHand.Count < 2)
                    {
                        isValid = false;
                    }
                }
            }

            // If contains no duplicards or two of duplicard then a valid combination
            if (isValid)
            {
                newCombinations.Add(combinations[i]);
            }
        }

        return newCombinations;
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
