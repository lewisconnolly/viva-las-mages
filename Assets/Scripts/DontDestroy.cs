using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Awake()
    {

        //GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");

        //if (objs.Length > 1)
        //{
        //    Destroy(this.gameObject);
        //}

        DontDestroyOnLoad(this.gameObject);
    }
}