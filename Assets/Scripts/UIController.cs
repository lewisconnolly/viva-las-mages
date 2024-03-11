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
