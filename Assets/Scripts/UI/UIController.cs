using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static System.TimeZoneInfo;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }

    public GameObject PauseScreen;
    public static bool isPaused = false;
    public Animator transition;
    public float transitionTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { PauseUnpause(); }
    }

    public TextMeshProUGUI healthValueText;
    
    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }  

    public void PauseUnpause()
    {
        if (PauseScreen.activeSelf == false)
        {
            PauseScreen.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            PauseScreen.SetActive(false);
            Time.timeScale = 1f;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QuitToMainMenu()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneLoader.instance.LoadMainMenu();
    }
}
