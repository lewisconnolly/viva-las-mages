using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        // Get health component of interacting player        
        //var playerHealth = interactor.GetComponent<PlayerHealth>();

        //if (playerHealth == null) return false;

        // Remove two hearts from player
        //playerHealth.TakeDamage(2);
        //playerHealth.PlaceBet(2);
        SceneLoader.instance.LoadPoker();
        
        return true;
    }
}
