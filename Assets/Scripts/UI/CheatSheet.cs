using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatSheet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {        
        if (Input.GetKeyDown(KeyCode.H) && PlayerInventory.instance.hasTalkedToLamp)
        {
            ShowCheatSheet();   
        }
    }

    public void ShowCheatSheet()
    {
        bool inPoker = false;

        if (SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            inPoker = true;
        }

        if (inPoker && RewardCardUI.instance.rewardCardParentObject.activeSelf)
        {
            RewardCardUI.instance.rewardCardParentObject.SetActive(false);
        }

        if (WSCController.instance.deckViewerParent.activeSelf)
        {
            WSCController.instance.deckViewerParent.SetActive(false);
        }

        if (!SceneManager.GetActiveScene().name.Contains("Poker") && WSCController.instance.merchantShopParent.activeSelf)
        {
            WSCController.instance.merchantShopParent.SetActive(false);
        }

        if (WSCController.instance.cheatSheetParent.activeSelf)
        {
            WSCController.instance.cheatSheetParent.SetActive(false);

            if (!inPoker)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            WSCController.instance.cheatSheetParent.SetActive(true);

            if (inPoker)
            {
                FindAnyObjectByType<PokerUIController>().GetComponent<Canvas>().enabled = false;
            }
        }
    }
}
