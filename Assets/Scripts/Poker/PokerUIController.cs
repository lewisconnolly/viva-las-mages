using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PokerUIController : MonoBehaviour
{
    public static PokerUIController instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapCards()
    {
        Debug.Log("Swapping card(s)");
    }
}
