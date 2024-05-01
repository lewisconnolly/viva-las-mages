using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyReward : MonoBehaviour
{
    public CardScriptableObject baseSO;
    public PowerCardController.PowerCardType powerCardType;
    public CardScriptableObject rewardCard;

    void Start()
    {     
    }

    public void SetUpCard()
    {
        //rewardCard = ScriptableObject.CreateInstance<CardScriptableObject>();
        if (baseSO != null)
        {
            rewardCard = Instantiate(baseSO);
            string name = baseSO.value.ToString() + baseSO.suit + powerCardType.ToString();
            rewardCard.name = name;
            rewardCard.value = baseSO.value;
            rewardCard.suit = baseSO.suit;
            rewardCard.material = baseSO.material;
            rewardCard.powerCardType = powerCardType;
        }
    }

    public CardScriptableObject GetRewardCard() { return rewardCard; }

}
