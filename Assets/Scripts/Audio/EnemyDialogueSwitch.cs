using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDialogueSwitch : MonoBehaviour
{
    public List<AK.Wwise.Switch> MySwitches = new List<AK.Wwise.Switch>();
    public string enemySwitch;

    void Start()
    {
        //enemySwitch = PokerUIController.instance.enemyIcon.sprite.ToString();
        enemySwitch = BattleController.instance.activeEnemy.ToString();

        if (enemySwitch.Contains("RatIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Rat").SetValue(gameObject);
        }

        if (enemySwitch.Contains("DreadPatronIcon"))
        {
            MySwitches.Find(x => x.ToString() == "DreadPatron").SetValue(gameObject);
        }

        if (enemySwitch.Contains("HarlequinIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Harlequin").SetValue(gameObject);
        }

        if (enemySwitch.Contains("SkeletonDealerIcon"))
        {
            MySwitches.Find(x => x.ToString() == "SkeletonDealer").SetValue(gameObject);
        }

        if (enemySwitch.Contains("ManageressIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Manageress").SetValue(gameObject);
        }

        if (enemySwitch.Contains("SlimeChefIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Slime").SetValue(gameObject);
        }

        if (enemySwitch.Contains("SnakeIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Snake").SetValue(gameObject);
        }

        if (enemySwitch.Contains("WizardIcon"))
        {
            MySwitches.Find(x => x.ToString() == "Wizard").SetValue(gameObject);
        }
    }
    
}
