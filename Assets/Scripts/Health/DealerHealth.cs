using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class DealerHealth : MonoBehaviour
{
    private void Awake()
    {
    }

    public enum EnemyType { DreadPatron, Harlequin, Manageress, Rat, SkeletonDealer, SlimeChef, Snake, Wizard }

    public int currentHealth;
    public bool activeEnemy;
    public int pcntChanceOfRandomHand;
    public Sprite uiIcon;
    public VisualEffect smokePuff;
    public GameObject model;
    public GameObject ui;
    public EnemyType enemyType;
    public Collider _collider;

    public AK.Wwise.Event poofSound;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (enemyType == EnemyType.Wizard)
        {
            transform.position = transform.position + Vector3.up * Mathf.Sin(Time.frameCount / 110) / 5f * Time.deltaTime;
        }
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
            if (dealerInteractable != null) dealerInteractable.prompt = "";
        }

        if (SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            PokerUIController.instance.SetEnemyHealthText(currentHealth);
            PokerUIController.instance.ShowHitSprite();
            PokerUIController.instance.ShowHealthChangeText(-damage, true);
        }

        if (SceneManager.GetActiveScene().name == "FinalBossPokerRoom")
        {
            for (int i = 0; i < damage; i++)
            {
                GameObject.Find("WizardHit").GetComponent<VisualEffect>().Play();
            }
        }
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;

        if (SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            PokerUIController.instance.SetEnemyHealthText(currentHealth);
            PokerUIController.instance.ShowHealthChangeText(health, true);
        }
    }

    public void DestroySelf()
    {
        StartCoroutine(DestroySelfEffect());
    }

    IEnumerator DestroySelfEffect()
    {
        yield return new WaitForSeconds(2);

        model.SetActive(false);
        ui.SetActive(false);
        _collider.enabled = false;
        poofSound.Post(gameObject);
        smokePuff.Play();

        yield return new WaitForSeconds(3);

        Destroy(this.gameObject);
    }
}
