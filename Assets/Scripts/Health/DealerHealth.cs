using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DealerHealth : MonoBehaviour
{
    public int currentHealth;            
       
    // Start is called before the first frame update
    void Start()
    {
        UIDealerController.instance.SetHealthText(currentHealth);
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
        UIDealerController.instance.SetHealthText(currentHealth);
    }
}
