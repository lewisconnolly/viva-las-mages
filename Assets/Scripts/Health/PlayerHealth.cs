using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

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

    private float vignetteIntensity = 0;
    UnityEngine.Rendering.VolumeProfile volumeProfile;
    UnityEngine.Rendering.Universal.Vignette vignette;

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

        // Display damage post process vignette effect        
        volumeProfile = FindAnyObjectByType<UnityEngine.Rendering.Volume>()?.profile;
        
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));

        if (!volumeProfile.TryGet(out vignette)) throw new System.NullReferenceException(nameof(vignette));

        if (!vignette)
        {
            Debug.Log("error, vignette empty");
        }

        StartCoroutine(TakeDamageEffect());

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

    private IEnumerator TakeDamageEffect()
    {
        vignette.active = true;

        vignetteIntensity = 0.5f;

        vignette.intensity.Override(vignetteIntensity);

        yield return new WaitForSeconds(0.4f);

        while (vignetteIntensity > 0)
        {
            vignetteIntensity -= 0.01f;

            if (vignetteIntensity < 0) vignetteIntensity = 0;

            vignette.intensity.Override(vignetteIntensity);

            yield return new WaitForSeconds(0.1f);
        }

        vignette.active = false;
        
        yield break;
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
