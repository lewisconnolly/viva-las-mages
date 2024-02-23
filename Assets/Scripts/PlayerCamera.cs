using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    
    private void Awake()
    {
        instance = this;
    }    
    
    public void EnablePlayerCamera()
    {
        gameObject.SetActive(true);
    }

    public void DisablePlayerCamera()
    {
        gameObject.SetActive(false);
    }
}
