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
       
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Poker")
        {
            UIDealerController.instance.SetHealthText(currentHealth);
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
            PokerUIController.instance.SetEnemyHealthText(currentHealth);
        }
        else
        {
            UIDealerController.instance.SetHealthText(currentHealth);
        }
        
    }
}
