using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }
    
    public static bool isPaused = false;

    public GameObject pauseScreen;
    public GameObject settingsScreen;
    public GameObject gameOverScreen;

    public Slider sensitivitySlider;
    public Slider volumeSlider;
    public TextMeshProUGUI currentSensitivity;
    public TextMeshProUGUI currentVolume;
    
    public AudioMixer audioMixer;

    public Animator gameOverTransition;

    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    private void Start()
    {
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

        currentSensitivity.text = Mathf.Round((sensitivitySlider.value - 10f) / 10f).ToString();
        currentVolume.text = Mathf.Round((volumeSlider.value + 80f) * 1.25f).ToString();
    }

    public TextMeshProUGUI healthValueText;
    
    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }  

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

            if (!WSCController.instance.deckViewerParent.activeSelf)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    public void GoToSettings()
    {
        pauseScreen.SetActive(false);
        settingsScreen.SetActive(true);

    }

    public void QuitToMainMenu()
    {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        isPaused = false;
        SceneLoader.instance.LoadMainMenu();
        Time.timeScale = 1f;
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

    public void RestartGame()
    {
        gameOverScreen.SetActive(false);

        // Delete DontDestroyOnLoad objects
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject exit = GameObject.FindGameObjectWithTag("ExitCost");
        GameObject tvc = GameObject.FindGameObjectWithTag("TVC");
        GameObject[] slotMachines = GameObject.FindGameObjectsWithTag("SlotMachine");
        GameObject merchant = GameObject.FindGameObjectWithTag("Merchant");      

        foreach (GameObject enemy in enemies) { if (enemy != null) { Destroy(enemy); } }
        foreach (GameObject sm in slotMachines) { if (sm != null) { Destroy(sm); } }
        if (player != null) { Destroy(player); }
        if (exit != null) { Destroy(exit); }
        if (tvc != null) { Destroy(tvc); }
        if (merchant != null) { Destroy(merchant); }

        SceneManager.LoadScene("T1BasementRoom1");
    }

    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        gameOverTransition.Play(0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;        
    }
}
