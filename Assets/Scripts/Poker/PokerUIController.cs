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

    public TextMeshProUGUI healthValueText, enemyHealthValueText;
    public TextMeshPro exitHealthValueText;
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

    public GameObject pauseScreen;
    public static bool isPaused = false;
    public Animator transition;
    public float transitionTime = 1f;

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

        if (GameObject.FindGameObjectWithTag("Enemy") != null) { SetEnemyHealthText(BattleController.instance.activeEnemy.GetHealth()); }

        if (GameObject.FindGameObjectWithTag("ExitCost") != null) { SetExitHealthText(ExitCost.instance.GetHealth()); }

        if (GameObject.FindGameObjectWithTag("Player") != null) { SetHealthText(PlayerHealth.instance.GetHealth()); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { PauseUnpause(); }

        if (Input.GetKeyDown(KeyCode.L))
        {
            BattleController.instance.activeEnemy.activeEnemy = false;
            SceneLoader.instance.LoadRoom(PlayerInventory.instance.prevScene);            
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

    public void SetWinnerText(string text) { winnerText.text = text; }

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
        if (!isPaused) { BattleController.instance.PlayHand(); }
    }

    public void PlaceBet()
    {
        if (!isPaused) { BattleController.instance.PlaceBet(int.Parse(betSlider.currentBet.text)); }
    }

    public void PlayAgain()
    {     
        if (!isPaused) { BattleController.instance.PlayAgain(); }
    }

    public void Leave()
    {
        if (!isPaused)
        {
            BattleController.instance.activeEnemy.activeEnemy = false;
            SceneLoader.instance.LoadRoom(PlayerInventory.instance.prevScene);
        }
    }

    public void SwapCards()
    {
        if (!isPaused) { HandController.instance.SwapCards(); }
    }

    public void PauseUnpause()
    {
        if (pauseScreen.activeSelf == false)
        {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }

    public void QuitToMainMenu()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneLoader.instance.LoadMainMenu();
    }
}
