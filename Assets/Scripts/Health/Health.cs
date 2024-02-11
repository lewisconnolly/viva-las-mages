using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public TextMeshProUGUI count;    

    public int GetHealth()
    {
        return int.Parse(count.text);        
    }

    public void SetHealth(int health)
    {
        count.text = health.ToString();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
