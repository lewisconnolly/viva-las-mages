using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PointerHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject parent;
    
    //Detect if a click occurs
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        parent.SetActive(false);

        if (SceneManager.GetActiveScene().name != "Poker")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}