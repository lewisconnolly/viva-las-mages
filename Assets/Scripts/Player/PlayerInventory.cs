using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory instance;

    private void Awake()
    {
        instance = this;
        SaveDeck();
    }

    public List<CardScriptableObject> playerDeck;

    public void SaveDeck()
    {
        //"Assets/Unused" folder should exist before running this Method
        string[] playerDeckFolder = { "Assets/Cards/PlayerDeck" };
        foreach (var asset in AssetDatabase.FindAssets("", playerDeckFolder))
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }

        foreach (CardScriptableObject card in playerDeck) { AssetDatabase.CreateAsset(card, "Assets/Cards/PlayerDeck" + card.value.ToString() + card.suit + card.powerCardType.ToString() + ".asset"); }
    }

    public void AddRewardCardtoDeck()
    {
        EnemyReward reward = FindObjectOfType<EnemyReward>();
        CardScriptableObject rewardCard = reward.GetRewardCard();

        int indexOfCardToReplace = 0;

        for (int i = 0; i < playerDeck.Count; i++)
        {
            if (playerDeck[i].value == rewardCard.value && playerDeck[i].suit == rewardCard.suit)
            {
                indexOfCardToReplace = i;
            }
        }

        playerDeck[indexOfCardToReplace] = rewardCard;
    }
}
