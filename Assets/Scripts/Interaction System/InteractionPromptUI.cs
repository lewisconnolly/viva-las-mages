using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera mainCam;
    private float distance = 0.35f;
    private float offsetY = -0.125f;
    public TextMeshProUGUI prompText;
    [SerializeField] private GameObject uiPanel;

    private void Start()
    {
        mainCam = Camera.main;
        uiPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        if (mainCam.IsDestroyed())
        {
            mainCam = Camera.main;
        }

        var rotation = mainCam.transform.rotation;
        // Follow the player camera and tilt to face towards camera
        transform.position = mainCam.transform.TransformPoint(new Vector3(0, offsetY, distance));        
        transform.LookAt(transform.position + rotation * Vector3.forward,
            rotation * Vector3.up);
    }

    public bool isDisplayed = false;

    // Set text and display
    public void SetUp(string promptText)
    {
        if (promptText == "")
        {
            prompText.text = promptText;
            uiPanel.SetActive(false);
            isDisplayed = false;
        }
        else
        {
            prompText.text = promptText;
            uiPanel.SetActive(true);
            isDisplayed = true;
        }
    }

    // Stop displaying
    public void Close()
    {
        uiPanel.SetActive(false);
        isDisplayed = false;
    }
}
