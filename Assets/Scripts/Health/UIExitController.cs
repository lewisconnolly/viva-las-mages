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

    public void SetHealthText(int health) { healthValueText.text = health.ToString(); }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
