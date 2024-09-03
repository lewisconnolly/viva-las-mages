using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

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

    public float growSpeed = 0.01f;

    private void Start()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<GameObject> activeEnemies = enemies.Where(enemy => enemy.GetComponent<DealerHealth>().activeEnemy == true).ToList();

        if (activeEnemies.Count > 0 && SceneManager.GetActiveScene().name != "FinalBossPokerRoom")
        {
            GameObject activeEnemy = activeEnemies.First();
            EnemyReward enemyReward = activeEnemy.GetComponent<EnemyReward>();

            CardScriptableObject enemyRewardCard = enemyReward.GetRewardCard();
            rewardCard.cardSO = enemyRewardCard;
            rewardCard.SetUpCard();

            rewardCardText.text = rewardCard.powerCardType.ToString() + " Won";
        }
    }

    private void Update()
    {
        bool isPaused;
        if (!SceneManager.GetActiveScene().name.Contains("Poker")) { isPaused = UIController.isPaused; } else { isPaused = PokerUIController.isPaused; }

        if (Input.GetMouseButtonDown(0) && !isPaused)
        {
            rewardCardParentObject.SetActive(false);

            if (SceneManager.GetActiveScene().name.Contains("Poker"))
            {
                VFXController.instance.sparkles.Stop();
            }
            else
            {
                if (PlayerVfx.instance.sparkles != null)
                {
                    PlayerVfx.instance.sparkles.Stop();
                }
            }
        }
    }

    public void SlotMachineReward(CardScriptableObject smRewardCard)
    {
        //GameObject[] slotMachines = GameObject.FindGameObjectsWithTag("SlotMachine");
        //SlotMachine activeSlotMachine = slotMachines.Where(sm => sm.GetComponentInChildren<SlotMachine>().activeSlotMachine == true).ToList().First().GetComponentInChildren<SlotMachine>();

        //CardScriptableObject smRewardCard = activeSlotMachine.GetRewardCard();
        rewardCard.cardSO = smRewardCard;
        rewardCard.SetUpCard();

        rewardCardText.text = rewardCard.powerCardType.ToString() + " Won";

        if (PlayerVfx.instance.sparkles != null)
        {
            PlayerVfx.instance.sparkles.Play();
        }

        rewardCardParentObject.SetActive(true);       
        PlayerInventory.instance.AddRewardCardtoDeck(rewardCard.cardSO);
    }
}
