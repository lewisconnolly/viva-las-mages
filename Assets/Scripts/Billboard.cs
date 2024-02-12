using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform mainCam;
    
    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCam.forward);
    }
}
