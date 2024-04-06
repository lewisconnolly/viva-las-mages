using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

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

    public static bool isPaused = false;

    public GameObject pauseScreen;
    public GameObject settingsScreen;

    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public TextMeshProUGUI currentSensitivity;
    public TextMeshProUGUI currentVolume;

    public AudioMixer audioMixer;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    public Animator transition;
    public float transitionTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused && !pauseScreen.activeSelf)
            {
                settingsScreen.SetActive(false);
                pauseScreen.SetActive(true);
            }
            else
            {
                PauseUnpause();
            }
        }

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

        currentSensitivity.text = Mathf.Round((sensitivitySlider.value - 10f) / 10f).ToString();
        currentVolume.text = Mathf.Round((volumeSlider.value + 80f) * 1.25f).ToString();
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
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
        }
    }
    public void GoToSettings()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);

    }

    public void QuitToMainMenu()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneLoader.instance.LoadMainMenu();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetGraphicsQuality(int qualityIndex)
    {
        int level = 2;

        if (qualityIndex == 1) { level = 1; }

        QualitySettings.SetQualityLevel(level);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetLookSensitivity(float sensitivity)
    {
        MouseLook playerMouseLook = FindObjectOfType<MouseLook>();

        if (playerMouseLook != null)
        {
            playerMouseLook.mouseSensitivity = sensitivity;
        }
    }
}
