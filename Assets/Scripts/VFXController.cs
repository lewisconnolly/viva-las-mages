using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXController : MonoBehaviour
{
    public static VFXController instance;

    private void Awake()
    {
        instance = this;
    }

    public VisualEffect hit;
    public VisualEffect smokePuff;
    public VisualEffect sparkles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
