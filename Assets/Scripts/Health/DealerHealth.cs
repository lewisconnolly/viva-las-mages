using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerHealth : MonoBehaviour
{
    public int currentHealth;
    public Health health;

    // Start is called before the first frame update
    void Start()
    {
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
    }

    public int GetHealth() { return currentHealth; }
}
