using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    float unfreezeWait = 3.0f;

    //Audio queues and footstep timers
    public AK.Wwise.Event FootStep;
    private float timer = 0.0f;
    private float footstepSpeed = 0.4f;
    Vector3 lastposition;
    float moveMin = 0.01f;
    private bool isMoving = false;

    private void Awake()
    {
        instance = this;        
        UnfreezePlayer();
        unfreezeWait = 0.5f;
    }

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 0.25f;
        
    public CharacterController controller;    
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private Vector3 velocity;
    private bool isGrounded;
    public bool isFrozen = true;

    public bool moveToStartingPosition = false;

    public void FreezePlayer()
    {
        if (!isFrozen) { isFrozen = true; }
    }

    public void UnfreezePlayer()
    {
        if (isFrozen) { StartCoroutine(UnfreezeCo()); }
    }

    IEnumerator UnfreezeCo()
    {
        yield return new WaitForSeconds(unfreezeWait);

        isFrozen = false;
    }

    private void FixedUpdate()
    {
        lastposition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFrozen && !(WSCController.instance.deckViewerParent.activeSelf || WSCController.instance.cheatSheetParent.activeSelf || WSCController.instance.merchantShopParent.activeSelf))
        {
            // Test if sphere with radius groundDistance around Ground Check object is in contact with the Ground layer
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            // If in contact with the ground and y velocity is still decreasing, set to -2f (not 0 in case still falling)
            if (isGrounded && velocity.y < 0) { velocity.y = -2f; }

            // Get input for moving left, right, forward and backward
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Modify x and z positions of character controller
            Vector3 move = transform.right * x + transform.forward * z;
            // Multiply move vector by player speed variable and delta time for movement (to be framerate independent)
            controller.Move(move * speed * Time.deltaTime);

            // If grounded and jump button (space) is pressed then increase y of velocity vector depending on jumpHeight and gravity variables
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity to y velocity to make player fall
            velocity.y += gravity * Time.deltaTime;

            // Apply velocity vector to controller movement (gravity equation requires time squared so multiply by deltaTime again)
            controller.Move(velocity * Time.deltaTime);

            // Footstep sound on timer
            CheckMoving();

            if (isGrounded && isMoving)
            {
                if (timer > footstepSpeed)
                {
                    FootStep.Post(gameObject);
                    timer = 0.0f;                   
                }

                timer += Time.deltaTime;
            }
            
            
        }
        void CheckMoving()
        {
            float distance = Vector3.Distance(transform.position, lastposition);
            if(distance > moveMin)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
    }          
}
