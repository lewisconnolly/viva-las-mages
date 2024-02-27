using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    private void Awake()
    {
        instance = this;
        currentHealth = maxHealth;        
        //betCounter = 0;
    }

    public int maxHealth = 10;
    private int currentHealth;
    //private int betCounter;
        
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Poker")
        {
            UIController.instance.SetHealthText(currentHealth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth() { return currentHealth; }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) { currentHealth = 0; }
        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetHealthText(currentHealth);
        }
        else
        {
            UIController.instance.SetHealthText(currentHealth);
        }
    }

    public void PlaceBet(int bet)
    {
        //betCounter += bet;
        PokerUIController.instance.SetBetText(bet);
        PokerUIController.instance.ShowBetIcons();
        ExitCost.instance.TakeDamage(bet);
    }
}
