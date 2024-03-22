using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    private void Awake()
    {
        instance = this;
    }

    public float speed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 0.25f;
        
    public CharacterController controller;    
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private Vector3 velocity;
    private bool isGrounded;
    private bool isFrozen = false;

    public bool moveToStartingPosition = false;

    public void FreezePlayer()
    {
        if (!isFrozen) { isFrozen = true; }
    }

    public void UnfreezePlayer()
    {        
        if (isFrozen) { isFrozen = false; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFrozen)
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
        }
    }      
}
