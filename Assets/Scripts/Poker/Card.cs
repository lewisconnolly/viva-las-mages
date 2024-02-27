using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public bool isPlayer;

    public int value;
    public string suit;
    public GameObject model;

    private Vector3 targetPoint;
    private Quaternion targetRot;
    public float moveSpeed = 5f;
    public float rotateSpeed = 540f;

    public bool inHand;
    public int handPosition;

    private HandController hc;
    private Camera mainCam;

    private bool isSelected;
    private bool isInSelectedPosition;
    private Collider col;

    public LayerMask whatIsDesktop;
    public LayerMask whatIsPlacement;
    //private bool justPressed;

    public CardPlacePoint assignedPlace;

    // Start is called before the first frame update
    void Start()
    {        
        if(targetPoint == Vector3.zero)
        {
            targetPoint = transform.position;
            targetRot = transform.rotation;
        }
        
        SetUpCard();
        hc = FindObjectOfType<HandController>();
        col = GetComponent<Collider>();
        mainCam = Camera.main;
    }

    // Set the value, suit and mesh to display based on scriptable object
    public void SetUpCard()
    {
        value = cardSO.value;
        suit = cardSO.suit;
        model.GetComponent<MeshFilter>().mesh = cardSO.mesh;
    }

    // Update is called once per frame
    void Update()
    {
        // Linear interpolation to target point in moveSpeed increments
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        // Match target rotation in rotateSpeed increments
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        
        if (isSelected)
        {
            //// Cast a ray from the camera to the mouse position
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;


            //// Detect where the ray hits the desktop layer
            //if(Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            //{
            //    // Move the card to the point of intersection but 0.5 units higher in the y-axis (to float above desktop)
            //    MoveToPoint(hit.point + new Vector3(0f, 0.3f, 0f), hc.minPos.rotation);
            //}                        

            if (Input.GetMouseButtonDown(0) && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == col)
                    {
                        if (!isInSelectedPosition)
                        {
                            MoveToPoint(hc.cardPositions[handPosition] + new Vector3(-.3f, .1f, .1f), hc.minPos.rotation);
                            isInSelectedPosition = true;
                            hc.SelectCard(this);
                            hc.SortSelectedCards();
                        }
                        else
                        {
                            ReturnToHand();
                        }
                    }
                }
            }

            // Right click to return card to hand
            if (Input.GetMouseButtonDown(1))
            {
                ReturnToHand();
            }
            
            // Stop card immediately being returned to hand if mouse was just clicked by checking justPressed
            //if (Input.GetMouseButtonDown(0) && !justPressed && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive)
            //{
            //    if (Physics.Raycast(ray, out hit, 100f, whatIsPlacement))
            //    {
            //        // Get placement point that was clicked on
            //        CardPlacePoint selectedPoint = hit.collider.GetComponent<CardPlacePoint>();

            //        // If the selected point has an active card attribute and it is a player placement slot
            //        // then move this card to the placement slot and remove from hand
            //        if (selectedPoint.activeCard == null && selectedPoint.isPlayerPoint)
            //        {
            //            selectedPoint.activeCard = this;
            //            assignedPlace = selectedPoint;

            //            MoveToPoint(selectedPoint.transform.position, hc.minPos.rotation);

            //            inHand = false;
            //            isSelected = false;

            //            hc.RemoveCardFromHand(this);
            //        }
            //        else
            //        {
            //            ReturnToHand();
            //        }
            //    }
            //    else
            //    {
            //        ReturnToHand();
            //    }
            //}
        }

        // Reset justPressed
        //justPressed = false;
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
        if (inHand && isPlayer && !isSelected)
        {
            MoveToPoint(hc.cardPositions[handPosition] + new Vector3(0f, .1f, .1f), hc.minPos.rotation);
        }
    }

    // Move card back down if mouse is no longer hovering over
    private void OnMouseExit()
    {
        if (inHand && isPlayer && !isSelected)
        {
            MoveToPoint(hc.cardPositions[handPosition], hc.minPos.rotation);
        }
    }

    // Prevent card being selected again on click
    private void OnMouseDown()
    {
        if (inHand && BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && isPlayer
            && hc.selectedCards.Count < 5)
        {
            isSelected = true;
            //col.enabled = false;
            //justPressed = true;                                       
        }
    }
   
    public void ReturnToHand()
    {
        isSelected = false;
        isInSelectedPosition = false;
        col.enabled = true;
        hc.selectedCards.Remove(this);
        hc.SortSelectedCards();
        MoveToPoint(hc.cardPositions[handPosition], hc.minPos.rotation);
    }
}
