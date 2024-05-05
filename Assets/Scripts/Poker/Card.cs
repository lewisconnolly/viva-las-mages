using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public bool isPlayer;

    public int value;
    public string suit;
    public GameObject model;
    public PowerCardController.PowerCardType powerCardType;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 7.5f;
    public float rotateSpeed = 540f;

    public bool inHand;
    public int handPosition;

    private HandController hc;
    private Camera mainCam;

    public bool isSelected;
    private bool isInSelectedPosition;
    private Collider col;

    public LayerMask whatIsDesktop;
    public LayerMask whatIsPlacement;  

    private MeshRenderer cardRenderer;
    private Color originalColour;
    private Color targetColour;
    private float transparentAlpha = 0.5f;
    private Color lerpedColour;
    private float alphaChangeSpeed = 6.75f;
    private bool isMouseOverAndHasFadedOut = false;

    public bool isDuplicate;

    // Start is called before the first frame update
    void Start()
    {
        if (targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }

        SetUpCard();
        hc = FindObjectOfType<HandController>();
        col = GetComponent<Collider>();
        mainCam = Camera.main;
        cardRenderer = model.GetComponent<MeshRenderer>();
        originalColour = cardRenderer.material.color;
        targetColour = originalColour;
    }

    // Set the value, suit and mesh to display based on scriptable object
    public void SetUpCard()
    {
        value = cardSO.value;
        suit = cardSO.suit;        
        powerCardType = cardSO.powerCardType;

        if (!isPlayer)
        {
            // Add reward card to enemy's hand
            if (value == RewardCardUI.instance.rewardCard.cardSO.value && suit == RewardCardUI.instance.rewardCard.cardSO.suit)
            {
                powerCardType = RewardCardUI.instance.rewardCard.cardSO.powerCardType;
            }

            // Add player's card to wizard's hand
            if (SceneManager.GetActiveScene().name == "FinalBossPokerRoom")
            {
                for (int i = 0; i < PlayerInventory.instance.playerDeck.Count; i++)
                {
                    if (value == PlayerInventory.instance.playerDeck[i].value && suit == PlayerInventory.instance.playerDeck[i].suit)
                    {
                        powerCardType = PlayerInventory.instance.playerDeck[i].powerCardType;
                    }
                }
            }
        }        

        SetPowerCardMaterial();
    }

    void SetPowerCardMaterial()
    {
        Material[] mats = model.GetComponent<MeshRenderer>().materials;
        mats[0] = cardSO.material;

        switch (powerCardType)
        {
            case PowerCardController.PowerCardType.None:
                mats[1] = PowerCardController.instance.noneMaterial;
                break;

            case PowerCardController.PowerCardType.Wildcard:
                mats[1] = PowerCardController.instance.wildcardMaterial;
                break;

            case PowerCardController.PowerCardType.FreeSwap:
                mats[1] = PowerCardController.instance.freeSwapMaterial;
                break;

            case PowerCardController.PowerCardType.HandSwap:
                mats[1] = PowerCardController.instance.handSwapMaterial;
                break;

            case PowerCardController.PowerCardType.TwoInOneClubs:
                mats[1] = PowerCardController.instance.halfClubsMaterial;
                break;

            case PowerCardController.PowerCardType.TwoInOneSpades:
                mats[1] = PowerCardController.instance.halfSpadesMaterial;
                break;

            case PowerCardController.PowerCardType.TwoInOneHearts:
                mats[1] = PowerCardController.instance.halfHeartsMaterial;
                break;

            case PowerCardController.PowerCardType.TwoInOneDiamonds:
                mats[1] = PowerCardController.instance.halfDiamondsMaterial;
                break;

            case PowerCardController.PowerCardType.Duplicard:
                mats[1] = PowerCardController.instance.autoPairMaterial;
                break;

            case PowerCardController.PowerCardType.RankUpgrade:
                mats[1] = PowerCardController.instance.upgradeRankMaterial;
                break;

            case PowerCardController.PowerCardType.Health:
                mats[1] = PowerCardController.instance.gainHeartMaterial;
                break;
        }

        model.GetComponent<MeshRenderer>().materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        // Linear interpolation to target point in moveSpeed increments
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        // Match target rotation in rotateSpeed increments
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        // Match target colour in alphaChangeSpeed increments
        lerpedColour = Color.Lerp(cardRenderer.material.color, targetColour, alphaChangeSpeed * Time.deltaTime);
        cardRenderer.material.color = lerpedColour;       

        // If card has been made transparent, set target colour to opaque again
        if (Mathf.Round(cardRenderer.material.color.a * 10) * 0.1 <= transparentAlpha) { MakeOpaque(); }

        if (isSelected)
        {
            // Cast a ray from the camera to the mouse position
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;              

            if (Input.GetMouseButtonDown(0) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive &&
                !PokerUIController.isPaused && !PlayerHealth.instance.isGameOver &&
                !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == col)
                    {
                        if (!isInSelectedPosition) { AddToSelection(); } else { ReturnToHand(); }
                    }
                }
            }

            // Right click to return card to hand
            if (Input.GetMouseButtonDown(1) && !PokerUIController.isPaused &&
                !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf))
            {
                ReturnToHand();
            }                        
        }

        if (SceneManager.GetActiveScene().name.Contains("Poker"))
        {
            double zPos = Mathf.Round(transform.position.z);
            if (zPos == Mathf.Round(BattleController.instance.playerDiscardPosition.position.z) ||
                zPos == Mathf.Round(BattleController.instance.enemyDiscardPosition.position.z))
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Set point and rotation for card
    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotToMatch)
    {
        targetPoint = pointToMoveTo;
        targetRot = rotToMatch;        
    }

    // Pop up card towards camera on mouse hover
    private void OnMouseOver()
    {
        if (inHand && isPlayer && !isSelected && !PokerUIController.isPaused &&
            !PlayerHealth.instance.isGameOver &&
            !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf))
        {
            MoveToPoint(hc.cardHandPositions[handPosition] + new Vector3(0.1f, 0.1f, 0), hc.cardHandRotations[handPosition]);
            
            // Make cards in hand next to this card transparent
            if (!isMouseOverAndHasFadedOut)
            {
                //hc.SetTransparency(this, "mouse over");
                // Stop card cycling between transparent and opaque while mouse over
                isMouseOverAndHasFadedOut = true;
            }
        }
    }

    // Move card back down if mouse is no longer hovering over
    private void OnMouseExit()
    {
        if (inHand && isPlayer && !isSelected && !PokerUIController.isPaused &&
            !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf))
        {
            MoveToPoint(hc.cardHandPositions[handPosition], hc.cardHandRotations[handPosition]);

            isMouseOverAndHasFadedOut = false;

            // Make cards in hand next to this card transparent
            //hc.SetTransparency(this, "mouse exit");
        }       
    }

    // Prevent card being selected again on click
    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer
            && hc.selectedCards.Count < (HandController.instance.numCardsRequiredToPlay - BattleController.instance.numAutoPairs)
            && !isSelected && !PokerUIController.isPaused && !PlayerHealth.instance.isGameOver
            && !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf))
        {
            //hc.SetTransparency(this, "select");

            if (!(HandController.instance.numCardsRequiredToPlay - BattleController.instance.numAutoPairs - hc.selectedCards.Count == 1 &&
                this.powerCardType == PowerCardController.PowerCardType.Duplicard))
            {
                isSelected = true;
            }
        }
    }

    public void AddToSelection()
    {
        hc.SelectCard(this);
        hc.SortSelectedCards();
               
        MoveToPoint(hc.cardHandPositions[handPosition] + new Vector3(-0.2f, 0.2f, 0), hc.cardHandRotations[handPosition]);
        
        isInSelectedPosition = true;
    }

    public void ReturnToHand()
    {        
        //hc.SetTransparency(this, "return");
        
        hc.selectedCards.Remove(this);
        hc.SortSelectedCards();
        isSelected = false;

        MoveToPoint(hc.cardHandPositions[handPosition], hc.cardHandRotations[handPosition]);

        isInSelectedPosition = false;
    }

    public void MakeTransparent()
    {        
        var r = cardRenderer.material.color.r;
        var g = cardRenderer.material.color.g;
        var b = cardRenderer.material.color.b;

        targetColour = new Color(r, g, b, transparentAlpha);
    }

    public void MakeOpaque()
    {        
        targetColour = originalColour;
    }
}
