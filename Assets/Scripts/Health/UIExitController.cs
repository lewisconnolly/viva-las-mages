using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIExitController : MonoBehaviour
{
    public static UIExitController instance;

    private void Awake()
    {
        instance = this;
    }

    public TextMeshProUGUI healthValueText;

    // Start is called before the first frame update
    void Start()
    {
        if (ExitCost.instance != null)
        {
            SetHealthText(ExitCost.instance.GetHealth());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }    
}
