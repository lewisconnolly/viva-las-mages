using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public string InteractionPrompt => prompt;

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
        if(ExitCost.instance.GetHealth() == 0)
        {         
            SceneLoader.instance.LoadRoom2();
        }        

        return true;
    }
}
