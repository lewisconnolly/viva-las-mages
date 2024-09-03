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

        if (enemySwitch.Contains("Rat"))
        {
            MySwitches.Find(x => x.ToString() == "Rat").SetValue(gameObject);
        }

        if (enemySwitch.Contains("DreadPatron"))
        {
            MySwitches.Find(x => x.ToString() == "DreadPatron").SetValue(gameObject);
        }

        if (enemySwitch.Contains("Harlequin"))
        {
            MySwitches.Find(x => x.ToString() == "Harlequin").SetValue(gameObject);
        }

        if (enemySwitch.Contains("SkeletonDealer"))
        {
            MySwitches.Find(x => x.ToString() == "SkeletonDealer").SetValue(gameObject);
        }

        if (enemySwitch.Contains("Manageress"))
        {
            MySwitches.Find(x => x.ToString() == "Manageress").SetValue(gameObject);
        }

        if (enemySwitch.Contains("SlimeChef"))
        {
            MySwitches.Find(x => x.ToString() == "Slime").SetValue(gameObject);
        }

        if (enemySwitch.Contains("Snake"))
        {
            MySwitches.Find(x => x.ToString() == "Snake").SetValue(gameObject);
        }

        if (enemySwitch.Contains("Wizard"))
        {
            MySwitches.Find(x => x.ToString() == "Wizard").SetValue(gameObject);
        }
    }
    
}
