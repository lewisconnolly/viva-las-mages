using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerVfx : MonoBehaviour
{
    public static PlayerVfx instance;

    private void Awake()
    {
        instance = this;
    }

    public VisualEffect sparkles;
    private Camera mainCam;
    private float distance = 0.35f;
    private float offsetY = -0.125f;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        // If sparkle effect is active, make it follow camera
        if (sparkles.HasAnySystemAwake())
        {
            if (mainCam.IsDestroyed())
            {
                mainCam = Camera.main;
            }

            var rotation = mainCam.transform.rotation;
            // Follow the player camera and tilt to face towards camera
            transform.position = mainCam.transform.TransformPoint(new Vector3(0, offsetY, distance));
            transform.LookAt(transform.position + Vector3.forward, rotation * Vector3.up);
        }
    }
}
