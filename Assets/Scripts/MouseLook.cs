using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Stop the cursor leaving the game view
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse input and multiply by sensitivity variable and delta time to be frame rate independent
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;   
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Decrease xRotation variable by mouse y-axis movement
        xRotation -= mouseY;
        // Prevent player loooking behind themselves
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera around the x-axis
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotate the camera around the y-axis
        playerBody.Rotate(Vector3.up, mouseX);
    }
}
