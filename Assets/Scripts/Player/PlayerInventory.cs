using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    private void Awake()
    {
        instance = this;
        
        ResetDeck();
        LoadDeck();        
    }

    public List<CardScriptableObject> playerDeck;
    public string prevScene;

    public bool hasTalkedToLamp;    

    public void LoadDeck()
    {                
        CardScriptableObject[] baseCards = Resources.LoadAll<CardScriptableObject>("Cards/PlayerDeck");        
        playerDeck.Clear();
        foreach (CardScriptableObject baseCard in baseCards) {
            CardScriptableObject newCard = ScriptableObject.CreateInstance<CardScriptableObject>();
            string name = baseCard.value.ToString() + baseCard.suit + baseCard.ToString();
            newCard.name = name;
            newCard.value = baseCard.value;
            newCard.suit = baseCard.suit;
            newCard.material = baseCard.material;
            newCard.powerCardType = baseCard.powerCardType;
            playerDeck.Add(newCard);
        }
    }

    public void ResetDeck()
    {
        CardScriptableObject[] baseCards = Resources.LoadAll<CardScriptableObject>("Cards");

        foreach (CardScriptableObject baseCard in baseCards)
        {
            baseCard.powerCardType = 0;
        }
    }

    public void AddRewardCardtoDeck(CardScriptableObject cardToAdd)
    {
        for (int i = 0; i < playerDeck.Count; i++)
        {
            if (playerDeck[i].value == cardToAdd.value && playerDeck[i].suit == cardToAdd.suit)
            {
                playerDeck[i].powerCardType = cardToAdd.powerCardType;
            }
        }
    }
}
