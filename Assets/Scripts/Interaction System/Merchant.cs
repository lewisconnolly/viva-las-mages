using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static PowerCardController;
using UnityEngine.SceneManagement;

public class Merchant : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public AK.Wwise.Event merchantSpeak;
    public string InteractionPrompt => prompt;

    public List<CardScriptableObject> shopCards;

    public bool cardsGenerated;

    // Prices looked up by enum int value of PowerCardType
    private int[] prices =
    {
        0, // None (not in shop)
        2, // Wildcard
        1, // FreeSwap
        2, // HandSwap
        0, // HalfClubs (not in shop)
        0, // HalfSpades (not in shop)
        0, // HalfHearts (not in shop)
        0, // HalfDiamonds (not in shop)
        3, // AutoPair
        4, // UpgradeRank
        1, // GainHeart
    };

    public bool card1Sold;
    public bool card2Sold;
    public bool card3Sold;

    public bool isTheOriginal;

    void Awake()
    {
        cardsGenerated = false;
        
        card1Sold = false;
        card2Sold = false;
        card3Sold = false;
    }

    public bool Interact(Interactor interactor)
    {
        // Generate cards on first interaction
        if (!cardsGenerated)
        {
            GenerateShopCards();
            AddCardsToShop();
            cardsGenerated = true;
        }        

        if (!WSCController.instance.merchantShopParent.activeSelf)
        {            
            MerchantShop.instance.ShowShop();
            merchantSpeak.Post(gameObject);
        }

        return true;
    }

    void Update()
    {
        if (!SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            if (WSCController.instance.merchantShopParent.activeSelf)
            {
                prompt = "";
            }
            else
            {
                if (card1Sold && card2Sold && card3Sold)
                {
                    prompt = "Thank ya!";
                }
                else
                {
                    prompt = "Got some rare things on sale, stranger";
                }
            }

            // If cards sold or player not got enough health but card not disabled, disable
            // In Update because merchant shop reloads when returning from poker scene
            if ((card1Sold || PlayerHealth.instance.currentHealth <= MerchantShop.instance.shopCard1Price) &&
                MerchantShop.instance.shopCard1Button.interactable)
            {
                MerchantShop.instance.DisableShopCard(1);
            }
            if ((card2Sold || PlayerHealth.instance.currentHealth <= MerchantShop.instance.shopCard2Price) &&
                MerchantShop.instance.shopCard2Button.interactable)
            {
                MerchantShop.instance.DisableShopCard(2);
            }
            if ((card3Sold || PlayerHealth.instance.currentHealth <= MerchantShop.instance.shopCard3Price) &&
                MerchantShop.instance.shopCard3Button.interactable)
            {
                MerchantShop.instance.DisableShopCard(3);
            }
        }
    }


    public bool ResetInteractable()
    {
        return true;
    }

    public void AddCardsToShop()
    {        
        MerchantShop.instance.shopCard1.cardSO = shopCards[0];
        MerchantShop.instance.shopCard2.cardSO = shopCards[1];
        MerchantShop.instance.shopCard3.cardSO = shopCards[2];

        MerchantShop.instance.shopCard1.SetUpCard();
        MerchantShop.instance.shopCard2.SetUpCard();
        MerchantShop.instance.shopCard3.SetUpCard();

        string shopCard1Text = shopCards[0].value + " of " + shopCards[0].suit + "\n" + shopCards[0].powerCardType.ToString();
        string shopCard2Text = shopCards[1].value + " of " + shopCards[1].suit + "\n" + shopCards[1].powerCardType.ToString();
        string shopCard3Text = shopCards[2].value + " of " + shopCards[2].suit + "\n" + shopCards[2].powerCardType.ToString();

        MerchantShop.instance.shopCard1Text.text = shopCard1Text;
        MerchantShop.instance.shopCard2Text.text = shopCard2Text;
        MerchantShop.instance.shopCard3Text.text = shopCard3Text;

        int shopCard1Price = prices[(int)shopCards[0].powerCardType];
        int shopCard2Price = prices[(int)shopCards[1].powerCardType];
        int shopCard3Price = prices[(int)shopCards[2].powerCardType];

        MerchantShop.instance.shopCard1Price = shopCard1Price;
        MerchantShop.instance.shopCard2Price = shopCard2Price;
        MerchantShop.instance.shopCard3Price = shopCard3Price;

        MerchantShop.instance.shopCard1ButtonText.text = "Buy\n(-" + shopCard1Price + " HP)";
        MerchantShop.instance.shopCard2ButtonText.text = "Buy\n(-" + shopCard2Price + " HP)";
        MerchantShop.instance.shopCard3ButtonText.text = "Buy\n(-" + shopCard3Price + " HP)";
    }

    void GenerateShopCards()
    {
        List<CardScriptableObject> chooseFrom = PlayerInventory.instance.playerDeck.Where(card => card.powerCardType == PowerCardType.None).ToList();
        
        // If no non-power cards, use any
        if (chooseFrom.Count == 0)
        {
            chooseFrom = PlayerInventory.instance.playerDeck;
        }

        PowerCardType[] powerCardTypes = (PowerCardType[])Enum.GetValues(typeof(PowerCardType));
        List<PowerCardType> powerCardTypesFiltered = powerCardTypes.Where(powerCard => !powerCard.ToString().StartsWith("TwoInOne") && powerCard.ToString() != "None").ToList();

        for (int i = 0; i < 3; i++)
        {
            // Check chooseFrom count each time in case initally less than 3 but more than 0 cards with no power ups
            if (chooseFrom.Count == 0)
            {
                chooseFrom = PlayerInventory.instance.playerDeck;
                // Remove previously selected cards
                for (int j = 0; j < shopCards.Count; i++)
                {
                    chooseFrom = chooseFrom.Where(card => card.value != shopCards[j].value && card.suit != shopCards[j].suit).ToList();
                }
            }
            
            CardScriptableObject baseRewardCard = chooseFrom[UnityEngine.Random.Range(0, chooseFrom.Count)];
            PowerCardType powerup = powerCardTypesFiltered[UnityEngine.Random.Range(0, powerCardTypesFiltered.Count)];

            //CardScriptableObject shopCard = ScriptableObject.CreateInstance<CardScriptableObject>();
            CardScriptableObject shopCard = Instantiate(baseRewardCard);
            string name = baseRewardCard.value.ToString() + baseRewardCard.suit + powerup.ToString();
            shopCard.name = name;
            shopCard.value = baseRewardCard.value;
            shopCard.suit = baseRewardCard.suit;
            shopCard.material = baseRewardCard.material;
            shopCard.powerCardType = powerup;

            shopCards.Add(shopCard);

            // Remove generated card from list to choose from
            chooseFrom = chooseFrom.Where(card => card.value != shopCard.value && card.suit != shopCard.suit).ToList();
        }
    }
}
