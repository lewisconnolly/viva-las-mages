using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] public InteractionPromptUI interactionPromptUI;

    private readonly Collider[] colliders = new Collider[3];
    [SerializeField] private int numFound;   

    private IInteractable interactable;

    // Update is called once per frame
    void Update()
    {
        // Count number of colliders overlapping with interaction point sphere
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);

        if(numFound > 0)
        {
            // Get component implementing IInteractable on the collider
            interactable = colliders[0].GetComponent<IInteractable>();                     

            if (interactable != null )
            {
                // Change the UI text on the interaction prompt panel based on the prompt text of the interactable and display it
                if (!interactionPromptUI.isDisplayed) interactionPromptUI.SetUp(interactable.InteractionPrompt);

                // Interact with the interactable if e was pressed this frame
                if (Keyboard.current.eKey.wasPressedThisFrame && !UIController.isPaused && !PlayerHealth.instance.isGameOver) interactable.Interact(this);
            }            
        }
        else
        {
            // Null out interactable
            if (interactable != null) interactable = null;
            // Stop displaying interaction prompt panel
            if (interactionPromptUI.isDisplayed) interactionPromptUI.Close();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Show wire sphere in editor for interaction point
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }
}
