using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PokerUIController : MonoBehaviour
{
    public static PokerUIController instance;    

    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI healthValueText, enemyHealthValueText, exitHealthValueText;

    public TextMeshProUGUI betValueText;
    public GameObject betIcon;
    private Vector3 betIconStart;
    public Transform betIconTarget;
    private bool showBetIcon;
    public BetSlider BetSlider;

    public GameObject placeBetButton, swapCardButton, endTurnButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InitPokerUI();
    }

    void InitPokerUI()
    {
        betIconStart = betIcon.transform.position;
        HideBetIcon();
        
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            SetEnemyHealthText(DealerHealth.instance.GetHealth());
        }

        if (GameObject.FindGameObjectWithTag("ExitCost") != null)
        {
            SetExitHealthText(ExitCost.instance.GetHealth());
        }

        if (GameObject.FindGameObjectWithTag("Player"))
        {
            SetHealthText(PlayerHealth.instance.GetHealth());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneLoader.instance.LoadRoom();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SetHealthText(PlayerHealth.instance.GetHealth());
        }        

        if (showBetIcon)
        {
            betIcon.transform.position = Vector3.Lerp(betIcon.transform.position, betIconTarget.position, 5f * Time.deltaTime);
        }
        else
        {
            betIcon.transform.position = betIconStart;
        }
    }

    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }
    
    public void SetEnemyHealthText(int health) { enemyHealthValueText.text = health.ToString(); }

    public void SetExitHealthText(int health) { exitHealthValueText.text = health.ToString(); }

    public void SetBetText(int bet) { betValueText.text = "-" + bet.ToString(); }

    public void HideBetIcon()
    {
        betIcon.SetActive(false);
        showBetIcon = false;
    }

    public void ShowBetIcon()
    {
        betIcon.SetActive(true);
        showBetIcon = true;
    }
    public void PlaceBet()
    {
        BattleController.instance.PlaceBet(int.Parse(BetSlider.currentBet.text));
    }

    public void EndPlayerTurn()
    {
        BattleController.instance.EndPlayerTurn();
    }

    public void SwapCards()
    {
        Debug.Log("Swapping card(s)");
    }       
}
