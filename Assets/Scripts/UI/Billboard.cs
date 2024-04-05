using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Billboard : MonoBehaviour
{    
    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam.IsDestroyed())
        {
            mainCam = Camera.main;
        }

        transform.LookAt(transform.position + mainCam.transform.forward);        
    }
}
