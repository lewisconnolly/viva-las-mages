using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;
    //public ExitCost exit;

    public bool Interact(Interactor interactor)
    {        
        // If the exit health for this door is 0, allow player to open
        if(ExitCost.instance.GetHealth() == 0 )
        {
            Debug.Log("Opening door");
        }
        else
        {
            Debug.Log("Door locked. Bet more health");
        }
                
        return true;
    }
}
