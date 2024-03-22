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
    }

    public int maxHealth = 10;
    public int currentHealth;
    //private int currentBet;
    private bool updateHealthText;
        
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.activeSceneChanged += ChangedActiveScene;

        if (SceneManager.GetActiveScene().name != "Poker")
        {
            UIController.instance.SetHealthText(currentHealth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (updateHealthText)
        {
            UIController.instance.SetHealthText(currentHealth);
            updateHealthText = false;
        }        
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

    public void IncreaseHealth(int health)
    {
        currentHealth += health;
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
        //currentBet = bet;
        PokerUIController.instance.SetBetText(bet);
        PokerUIController.instance.ShowBetIcons();
        ExitCost.instance.TakeDamage(bet);
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        string currentName = current.name;
        string nextName = next.name;

        if (nextName == "Poker")
        {
        }
        else
        {
            updateHealthText = true;
        }
    }
}
