using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.XR;

public class DeckViewer : MonoBehaviour
{
    public static DeckViewer instance;

    private void Awake()
    {
        instance = this;
    }

    public List<CardScriptableObject> playerDeck;
    public List<Button> selectCardButtons;
    public Card selectedCard;
    //public GameObject deckViewerParent;

    // Start is called before the first frame update
    void Start()
    {
        //playerDeck = PlayerInventory.instance.playerDeck.OrderBy(card => card.value).ThenBy(card => card.suit).ToList();
        foreach (Button button in selectCardButtons)
        {
            button.onClick.AddListener(() => { CardButtonPressed(button); });
        }
    }

    void CardButtonPressed(Button pressedButton)
    {
        selectedCard.cardSO = playerDeck[selectCardButtons.IndexOf(pressedButton)];
        selectedCard.SetUpCard();
    }

    void Initialise()
    {
        playerDeck = PlayerInventory.instance.playerDeck.OrderBy(card => card.value).ThenBy(card => card.suit).ToList();

        for (int i = 0; i < selectCardButtons.Count; i++)
        {
            string buttonText = $"{playerDeck[i].value} of {playerDeck[i].suit}";

            if (playerDeck[i].powerCardType != PowerCardController.PowerCardType.None)
            {
                buttonText += $" ({playerDeck[i].powerCardType})";
            }

            buttonText = buttonText.Replace("11", "Jack");
            buttonText = buttonText.Replace("12", "Queen");
            buttonText = buttonText.Replace("13", "King");
            buttonText = buttonText.Replace("14", "Ace");

            selectCardButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = buttonText;
        }        

        selectCardButtons[0].Select();
        selectedCard.cardSO = playerDeck[0];
        selectedCard.SetUpCard();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (SceneManager.GetActiveScene().name.Contains("Poker") && RewardCardUI.instance.rewardCardParentObject.activeSelf)
            {
                RewardCardUI.instance.rewardCardParentObject.SetActive(false);
            }

            if (WSCController.instance.cheatSheetParent.activeSelf)
            {
                WSCController.instance.cheatSheetParent.SetActive(false);
            }

            if (!SceneManager.GetActiveScene().name.Contains("Poker") && WSCController.instance.merchantShopParent.activeSelf)
            {
                WSCController.instance.merchantShopParent.SetActive(false);
            }

            if (WSCController.instance.deckViewerParent.activeSelf)
            {
                WSCController.instance.deckViewerParent.SetActive(false);

                if (!SceneManager.GetActiveScene().name.Contains("Poker"))
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else
            {
                WSCController.instance.deckViewerParent.SetActive(true);

                Initialise();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
