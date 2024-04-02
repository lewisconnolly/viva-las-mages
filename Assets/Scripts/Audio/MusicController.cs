using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MusicController : MonoBehaviour
{
    public AK.Wwise.Event Music;


    private void Awake()
    {
       // GameObject[] musicObj = GameObject.FindGameObjectsWithTag("MusicObj");

       // if (musicObj.Length > 1)
       // {
       //     Destroy(this.gameObject);
       // }
    }

    void Start()
    {
        Music.Post(gameObject);      
    }

    
   

   void OnDestroy()
    {
      //Music.Stop(gameObject); 
    }
}
