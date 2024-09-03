using UnityEngine;
using UnityEngine.SceneManagement;

public class WSCController : MonoBehaviour
{
    public static WSCController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject deckViewerParent;
    public GameObject cheatSheetParent;
    public GameObject cheatSheetButton;
    public GameObject merchantShopParent;

    private void Update()
    {
        Canvas pokerUI;

        if (SceneManager.GetActiveScene().name.Contains("Poker"))
        {          
            pokerUI = FindAnyObjectByType<PokerUIController>(FindObjectsInactive.Include).GetComponent<Canvas>();

            if (!cheatSheetParent.activeSelf && !pokerUI.enabled)
            {
                pokerUI.enabled = true;
            }

            if (PlayerInventory.instance.hasTalkedToLamp)
            {
                cheatSheetButton.SetActive(true);
            }
        }             
    }
    
}
