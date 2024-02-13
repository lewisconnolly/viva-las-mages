using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI healthValueText;
    
    public TextMeshProUGUI betValueText;
    public GameObject betIcon;
    private Vector3 betIconStart;
    public Transform betIconTarget;
    private bool showBetIcon;   

    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }
    
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

    // Start is called before the first frame update
    void Start()
    {
        betIconStart = betIcon.transform.position;
        HideBetIcon();        
    }

    // Update is called once per frame
    void Update()
    {
        if (showBetIcon)
        {
            betIcon.transform.position = Vector3.Lerp(betIcon.transform.position, betIconTarget.position, 5f * Time.deltaTime);
        }
        else
        {
            betIcon.transform.position = betIconStart;
        }
    }
}
