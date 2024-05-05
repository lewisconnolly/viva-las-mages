using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public string InteractionPrompt => prompt;
    public string sceneName;

    void Start()
    {
        if(ExitCost.instance != null && ExitCost.instance.GetHealth() == 0)
        {
            prompt = "Open Door";
        }
        else
        {
            prompt = "Locked";
        }
    }

    public bool Interact(Interactor interactor)
    {        
        // If the exit health for this door is 0, allow player to open
        if (ExitCost.instance.GetHealth() == 0)
        {
            //Play transition video
            if (sceneName == "FinalBossPokerRoom")
            {
                GameObject canvas = GameObject.FindGameObjectWithTag("TVC");
                canvas.GetComponentInChildren<RawImage>().enabled = true;
                VideoPlayer vp = canvas.GetComponentInChildren<VideoPlayer>();

                vp.url = "Assets/Resources/TransitionVideos/Wizard_Transition.mp4";
                vp.Play();
                vp.loopPointReached += EndReached;
            }

            SceneLoader.instance.LoadRoom(sceneName);
        }        

        return true;
    }

    public bool ResetInteractable()
    {
        return true;
    }

    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        vp.gameObject.GetComponentInChildren<RawImage>().enabled = false;
    }
}
