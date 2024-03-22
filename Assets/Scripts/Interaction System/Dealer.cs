using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dealer : MonoBehaviour, IInteractable
{
    [SerializeField] public string prompt;

    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        GetComponentInParent<DealerHealth>().activeEnemy = true;
        SceneLoader.instance.LoadPoker();
        
        return true;
    }
}
