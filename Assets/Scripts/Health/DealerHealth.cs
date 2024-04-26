using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DealerHealth : MonoBehaviour
{
    private void Awake()
    {
    }    

    public int currentHealth;
    public bool activeEnemy;
    public int pcntChanceOfRandomHand;
    public Sprite uiIcon;

    // Start is called before the first frame update
    void Start()
    {                      
    }

    private void Update()
    {
    }

    public int GetHealth() { return currentHealth; }    

    public void SetHealthText() { this.gameObject.GetComponentInChildren<UIDealerController>().SetHealthText(currentHealth); }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dealer dealerInteractable = GetComponentInChildren<Dealer>();
            dealerInteractable.prompt = ""; 
        }

        if (SceneManager.GetActiveScene().name == "Poker") { PokerUIController.instance.SetEnemyHealthText(currentHealth); }
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;        

        if (SceneManager.GetActiveScene().name == "Poker") { PokerUIController.instance.SetEnemyHealthText(currentHealth); }
    }
}
