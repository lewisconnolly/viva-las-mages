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

    private void Start()
    {     
    }

    private void Update()
    {        
    }

    public void LoadDeck()
    {
        CardScriptableObject[] baseCards = Resources.LoadAll<CardScriptableObject>("Cards/PlayerDeck");

        foreach (CardScriptableObject baseCard in baseCards) { playerDeck.Add(baseCard); }
    }

    public void ResetDeck()
    {
        DeleteDeck();
        
        CardScriptableObject[] baseCards = Resources.LoadAll<CardScriptableObject>("Cards");
        
        foreach (CardScriptableObject baseCard in baseCards)
        {                        
            CardScriptableObject newCard = ScriptableObject.CreateInstance<CardScriptableObject>();
            newCard.value = baseCard.value;
            newCard.suit = baseCard.suit;
            newCard.material = baseCard.material;
            newCard.powerCardType = baseCard.powerCardType;

            AssetDatabase.CreateAsset(newCard, "Assets/Resources/Cards/PlayerDeck/" + newCard.value.ToString() + newCard.suit + newCard.powerCardType.ToString() + ".asset");
        }
    }

    public void DeleteDeck()
    {
        string[] playerDeckFolder = { "Assets/Resources/Cards/PlayerDeck" };
        foreach (var asset in AssetDatabase.FindAssets("", playerDeckFolder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }
    }

    public void DeleteCard(string cardName)
    {

    }

    public void AddRewardCardtoDeck()
    {
        // Get reward card on enemy
        EnemyReward reward = FindObjectOfType<EnemyReward>();
        CardScriptableObject rewardCard = reward.GetRewardCard();

        // Find asset file for card of same suit and value of reward card then delete it
        string[] playerDeckFolder = { "Assets/Resources/Cards/PlayerDeck" };
        foreach (var asset in AssetDatabase.FindAssets("", playerDeckFolder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            string[] splitData = path.Split('/');
            string name = splitData[splitData.Length - 1].Replace("None.asset", string.Empty);

            if(name == (rewardCard.value.ToString() + rewardCard.suit))
            {
                Debug.Log(name);
                AssetDatabase.DeleteAsset(path);                
            }
        }

        // Create a new card from the reward card
        CardScriptableObject newCard = ScriptableObject.CreateInstance<CardScriptableObject>();
        newCard.value = rewardCard.value;
        newCard.suit = rewardCard.suit;
        newCard.material = rewardCard.material;
        newCard.powerCardType = rewardCard.powerCardType;
        string newCardName = newCard.value.ToString() + newCard.suit + newCard.powerCardType.ToString();

        // Save the new card as an asset file and then load it
        AssetDatabase.CreateAsset(newCard, "Assets/Resources/Cards/PlayerDeck/" + newCardName + ".asset");
        CardScriptableObject baseCard = Resources.Load<CardScriptableObject>("Cards/PlayerDeck/" + newCardName);                

        // Replace the card in the deck with the same suit and value as the new card (created from the reward card)
        int indexOfCardToReplace = 0;
        for (int i = 0; i < playerDeck.Count; i++)
        {
            if (playerDeck[i].value == rewardCard.value && playerDeck[i].suit == rewardCard.suit) { indexOfCardToReplace = i; }
        }

        playerDeck[indexOfCardToReplace] = baseCard;
    }
}
