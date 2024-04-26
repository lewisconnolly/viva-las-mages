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

    private void Update()
    {        
    }

    void Start()
    {               
        //UIExitController.instance.SetHealthText(exitHealthCost);
        ExitController.instance.SetHealthText(exitHealthCost);
    }

    public int GetHealth() { return exitHealthCost; }

    public void SetHealth(int health) { exitHealthCost = health; }

    public void TakeDamage(int damage)
    {               
        exitHealthCost -= damage;

        VFXController.instance.hit.Play();

        ExitController.instance.ShowFloatingText(damage);

        if (exitHealthCost <= 0)
        {
            exitHealthCost = 0;
        }

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetExitHealthText(exitHealthCost);
        }
        else
        {
            //UIExitController.instance.SetHealthText(exitHealthCost);
            ExitController.instance.SetHealthText(exitHealthCost);
        }   
    }
}
