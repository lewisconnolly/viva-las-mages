using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    public Health health;
    public ExitHealth exit;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        health.SetHealth(currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) { currentHealth = 0; }                
        health.SetHealth(currentHealth);
        // Decrease exit health when player spends hearts
        exit.TakeDamage(damage);
    }
}
