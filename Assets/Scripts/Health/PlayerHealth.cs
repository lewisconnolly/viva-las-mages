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
    }

    public int maxHealth = 10;
    private int currentHealth;
    private int betCounter;
        
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        if (SceneManager.GetActiveScene().name != "Poker")
        {
            UIController.instance.SetHealthText(currentHealth);
        }
        betCounter = 0;
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
        betCounter += bet;
        PokerUIController.instance.SetBetText(betCounter);
        PokerUIController.instance.ShowBetIcon();
        ExitCost.instance.TakeDamage(bet);
    }
}
