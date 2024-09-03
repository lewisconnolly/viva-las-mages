using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PowerCardController;

public class MerchantShop : MonoBehaviour
{
    public static MerchantShop instance;

    private void Awake()
    {
        instance = this;
    }

    //public GameObject merchantShopParentObject;
    public Card shopCard1;
    public Card shopCard2;
    public Card shopCard3;

    public TextMeshPro shopCard1Text;
    public TextMeshPro shopCard2Text;
    public TextMeshPro shopCard3Text;

    public GameObject shopCard1Light;
    public GameObject shopCard2Light;
    public GameObject shopCard3Light;

    public Button shopCard1Button;
    public Button shopCard2Button;
    public Button shopCard3Button;

    public TextMeshProUGUI shopCard1ButtonText;
    public TextMeshProUGUI shopCard2ButtonText;
    public TextMeshProUGUI shopCard3ButtonText;

    public int shopCard1Price;
    public int shopCard2Price;
    public int shopCard3Price;

    private void Start()
    {
        GameObject merchant = GameObject.FindGameObjectWithTag("Merchant");
        if (merchant != null)
        {
            if (merchant.GetComponent<Merchant>().cardsGenerated)
            {
                merchant.GetComponent<Merchant>().AddCardsToShop();
            }
        }

    }

    private void Update()
    {
        if (WSCController.instance.merchantShopParent.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void ShowShop()
    {
        if (WSCController.instance.cheatSheetParent.activeSelf)
        {
            WSCController.instance.cheatSheetParent.SetActive(false);
        }

        if (WSCController.instance.merchantShopParent.activeSelf)
        {
            WSCController.instance.merchantShopParent.SetActive(false);
        }

        WSCController.instance.merchantShopParent.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void BuyCard1()
    {        
        PlayerInventory.instance.AddRewardCardtoDeck(shopCard1.cardSO);
        Merchant merchant = GameObject.FindGameObjectWithTag("Merchant").GetComponent<Merchant>();
        merchant.card1Sold = true;

        PlayerHealth.instance.TakeDamage(shopCard1Price);
        ExitCost.instance.TakeDamage(shopCard1Price);
    }

    public void BuyCard2()
    {
        PlayerInventory.instance.AddRewardCardtoDeck(shopCard2.cardSO);
        Merchant merchant = GameObject.FindGameObjectWithTag("Merchant").GetComponent<Merchant>();
        merchant.card2Sold = true;

        PlayerHealth.instance.TakeDamage(shopCard2Price);
        ExitCost.instance.TakeDamage(shopCard2Price);
    }
    public void BuyCard3()
    {
        PlayerInventory.instance.AddRewardCardtoDeck(shopCard3.cardSO);
        Merchant merchant = GameObject.FindGameObjectWithTag("Merchant").GetComponent<Merchant>();
        merchant.card3Sold = true;

        PlayerHealth.instance.TakeDamage(shopCard3Price);
        ExitCost.instance.TakeDamage(shopCard3Price);
    }

    public void DisableShopCard(int shopCard)
    {
        switch (shopCard)
        {
            case 1:
                shopCard1Light.GetComponent<Light>().intensity /= 3;
                shopCard1Button.interactable = false;
                shopCard1Text.color = Color.gray;
                shopCard1ButtonText.color = Color.gray;
                break;
            
            case 2:
                shopCard2Light.GetComponent<Light>().intensity /= 3;
                shopCard2Button.interactable = false;
                shopCard2Text.color = Color.gray;
                shopCard2ButtonText.color = Color.gray;
                break;
            
            case 3:
                shopCard3Light.GetComponent<Light>().intensity /= 3;
                shopCard3Button.interactable = false;
                shopCard3Text.color = Color.gray;
                shopCard3ButtonText.color = Color.gray;
                break; 
            
            default: break;
        }
    }
}
