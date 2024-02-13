using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExitHealth : MonoBehaviour
{
    public int currentHealth;    

    // Start is called before the first frame update
    void Start()
    {
        UIExitController.instance.SetHealthText(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth() { return currentHealth; }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth < 0) { currentHealth = 0; }
        UIExitController.instance.SetHealthText(currentHealth);
    }
}
