using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Dealer : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;
    public string transitionVideoUrl;
    

    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        GetComponentInParent<DealerHealth>().activeEnemy = true;

        // Play transition video
        //GameObject canvas = GameObject.FindGameObjectWithTag("TVC");
        //canvas.GetComponentInChildren<RawImage>().enabled = true;
        //VideoPlayer vp = canvas.GetComponent<VideoPlayer>();

        //vp.url = transitionVideoUrl;
        //vp.Play();
        //vp.loopPointReached += EndReached;

        SceneLoader.instance.LoadPoker();        

        return true;
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        vp.gameObject.GetComponentInChildren<RawImage>().enabled = false;
    }
}
