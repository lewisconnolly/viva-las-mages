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
    public TextMeshProUGUI enemyBetValueText;
    public TextMeshProUGUI playerHandText;
    public TextMeshProUGUI enemyHandText;
    public TextMeshProUGUI winnerText;
    public GameObject betIcon;
    public GameObject enemyBetIcon;
    private Vector3 betIconOrigin;
    private Vector3 enemyBetIconOrigin;
    public Transform betIconTarget;
    public Transform enemyBetIconTarget;
    private bool showBetIcons;
    public BetSlider betSlider;

    public GameObject placeBetButton, swapCardButton, playHandButton, playAgainButton, leaveButton;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InitPokerUI();
    }

    void InitPokerUI()
    {
        betIconOrigin = betIcon.transform.position;
        enemyBetIconOrigin = enemyBetIcon.transform.position;
        leaveButton.SetActive(false);
        HideBetIcons();
        
        if (GameObject.FindGameObjectWithTag("Enemy") != null)
        {
            SetEnemyHealthText(DealerHealth.instance.GetHealth());
        }

        if (GameObject.FindGameObjectWithTag("ExitCost") != null)
        {
            SetExitHealthText(ExitCost.instance.GetHealth());
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
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

        if (showBetIcons)
        {
            betIcon.transform.position = Vector3.Lerp(betIcon.transform.position, betIconTarget.position, 5f * Time.deltaTime);
            enemyBetIcon.transform.position = Vector3.Lerp(enemyBetIcon.transform.position, enemyBetIconTarget.position, 5f * Time.deltaTime);
        }
        else
        {
            betIcon.transform.position = Vector3.Lerp(betIcon.transform.position, betIconOrigin, 5f * Time.deltaTime);
            enemyBetIcon.transform.position = Vector3.Lerp(enemyBetIcon.transform.position, enemyBetIconOrigin, 5f * Time.deltaTime);
        }
    }

    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }
    
    public void SetEnemyHealthText(int health) { enemyHealthValueText.text = health.ToString(); }

    public void SetExitHealthText(int health) { exitHealthValueText.text = health.ToString(); }

    public void SetHandText(string playerHand, string enemyHand)
    {
        playerHandText.text = playerHand;
        enemyHandText.text = enemyHand;
    }

    public void SetWinnerText(string text)
    {
        winnerText.text = text;
    }

    public void SetBetText(int bet)
    {
        betValueText.text = "-" + bet.ToString();
        enemyBetValueText.text = "-" + bet.ToString();
    }

    public void HideBetIcons()
    {
        betIcon.SetActive(false);
        enemyBetIcon.SetActive(false);
        showBetIcons = false;
    }

    public void ShowBetIcons()
    {
        betIcon.SetActive(true);
        enemyBetIcon.SetActive(true);
        showBetIcons = true;
    }
    public void PlayHand()
    {
        BattleController.instance.PlayHand();
    }

    public void PlaceBet()
    {
        BattleController.instance.PlaceBet(int.Parse(betSlider.currentBet.text));
    }

    public void PlayAgain()
    {
        BattleController.instance.PlayAgain();
    }

    public void Leave()
    {
        SceneLoader.instance.LoadRoom();
    }

    public void SwapCards()
    {
        Debug.Log("Swapping card(s)");
    }       
}
