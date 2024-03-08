using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DealerHealth : MonoBehaviour
{
    public static DealerHealth instance;

    private void Awake()
    {
        instance = this;
    }

    public int currentHealth;
    private bool updateHealthText;

    // Start is called before the first frame update
    void Start()
    {
        //SceneManager.activeSceneChanged += ChangedActiveScene;

        UIDealerController.instance.SetHealthText(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        
    }

    public int GetHealth() { return currentHealth; }    

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Dealer dealerInteractable = GetComponentInChildren<Dealer>();
            dealerInteractable.prompt = ""; 
        }

        UIDealerController.instance.SetHealthText(currentHealth);

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetEnemyHealthText(currentHealth);
        }        
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;
        
        UIDealerController.instance.SetHealthText(currentHealth);

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetEnemyHealthText(currentHealth);
        }
    }

    //private void ChangedActiveScene(Scene current, Scene next)
    //{
    //    string currentName = current.name;
    //    string nextName = next.name;

    //    if (nextName == "Poker")
    //    {
    //    }
    //    else
    //    {
    //    }
    //}
}
