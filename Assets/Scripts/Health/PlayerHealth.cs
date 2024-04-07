using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

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
    private bool updateHealthText;

    public bool isGameOver = false;

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

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isGameOver = true;
        }

        if (SceneManager.GetActiveScene().name == "Poker")
        {
            PokerUIController.instance.SetHealthText(currentHealth);
        }
        else
        {
            UIController.instance.SetHealthText(currentHealth);
            if (isGameOver) { EndGame(); }
        }
    }

    private void EndGame()
    {
        StartCoroutine(EndGame(2f));
    }

    IEnumerator EndGame(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        UIController.instance.GameOver();
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
