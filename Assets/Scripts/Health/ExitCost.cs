using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitCost : MonoBehaviour
{
    public static ExitCost instance;
    private void Awake()
    {
        instance = this;
    }

    public int exitHealthCost = 2;

    void Start()
    {               
         UIExitController.instance.SetHealthText(exitHealthCost);
    }

    public int GetHealth() { return exitHealthCost; }

    public void SetHealth(int health) { exitHealthCost = health; }

    public void TakeDamage(int damage)
    {
        exitHealthCost -= damage;
        if (exitHealthCost < 0) { exitHealthCost = 0; }

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetExitHealthText(exitHealthCost);
        }
        else
        {
            UIExitController.instance.SetHealthText(exitHealthCost);
        }   
    }
}
