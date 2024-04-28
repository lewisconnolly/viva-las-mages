using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void LoadDeck()
    {                
        CardScriptableObject[] baseCards = Resources.LoadAll<CardScriptableObject>("Cards/PlayerDeck");        
        playerDeck.Clear();
        foreach (CardScriptableObject baseCard in baseCards) { playerDeck.Add(baseCard); }
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
