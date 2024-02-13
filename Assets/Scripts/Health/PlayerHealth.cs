using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public ExitHealth exit;
    private int betCounter;
        
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UIController.instance.SetHealthText(currentHealth);
        betCounter = 0;
        UIController.instance.HideBetIcon();
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
        UIController.instance.SetHealthText(currentHealth);
        // Decrease exit health when player spends hearts
        //exit.TakeDamage(damage);
    }

    public void PlaceBet(int bet)
    {
        betCounter += bet;
        UIController.instance.SetBetText(betCounter);
        UIController.instance.ShowBetIcon();
        exit.TakeDamage(bet);
    }
}
