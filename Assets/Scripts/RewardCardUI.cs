using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardCardUI : MonoBehaviour
{
    public static RewardCardUI instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject rewardCardObject;

    private Vector3 targetScale;
    public float growSpeed = 0.01f;


    private void Update()
    {
        //transform.localScale = Vector3.Lerp(transform.localScale, targetScale, growSpeed * Time.deltaTime);
    }

    public void GrowToScale(Vector3 scaleToGrowTo) { targetScale = scaleToGrowTo; }
}
