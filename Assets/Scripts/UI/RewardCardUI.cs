using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class RewardCardUI : MonoBehaviour
{
    public static RewardCardUI instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject rewardCardParentObject;
    public Card rewardCard;
    public TextMeshPro rewardCardText;

    private Vector3 targetScale;
    public float growSpeed = 0.01f;

    private void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject activeEnemy = enemies.Where(enemy => enemy.GetComponent<DealerHealth>().activeEnemy == true).ToList().First();
        EnemyReward enemyReward = activeEnemy.GetComponent<EnemyReward>();         

        CardScriptableObject enemyRewardCard = enemyReward.GetRewardCard();        
        rewardCard.cardSO = enemyRewardCard;
        rewardCard.SetUpCard();

        rewardCardText.text = rewardCard.powerCardType.ToString() + " Won";
    }

    void Update()
    {
        //transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growSpeed * Time.deltaTime);
    }

    public void GrowToScale(Vector3 scaleToGrowTo) { targetScale = scaleToGrowTo; }
}
