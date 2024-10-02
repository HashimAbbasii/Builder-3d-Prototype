using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public VariableJoystick joystick;          // Reference to your joystick
    public GameObject mainCameraParent;        // Reference to the camera parent
    public Toggle CameraPosition;               // Toggle for camera position
    public Toggle CameraRotation;               // Toggle for camera rotation
    private Vector3 startPos;                  // Initial camera position

    [SerializeField] private float moveSpeed = 0.05f; // Adjust this value to change movement speed

    void Start()
    {
        // Set the initial camera position
        startPos = mainCameraParent.transform.position;

        // Set up listeners for the toggles
        CameraPosition.onValueChanged.AddListener(OnCameraPositionToggleChanged);
        CameraRotation.onValueChanged.AddListener(OnCameraRotationToggleChanged);
    }

    void Update()
    {
        // Check if the CameraPosition toggle is on
        if (CameraPosition.isOn)
        {
            // Move the camera based on joystick input
            MoveCamera();
        }
    }

    private void MoveCamera()
    {
        // Get the joystick input
        float horizontalInput = joystick.Horizontal; // X-axis movement
        float verticalInput = joystick.Vertical;     // Z-axis movement

        // Calculate the new camera position
        Vector3 newPosition = mainCameraParent.transform.position;

        // Move the camera on the Z-axis when joystick is moved up
        newPosition.z += verticalInput * moveSpeed; // Increase Z position based on joystick's vertical input

        // Move the camera on the X-axis when joystick is moved sideways
        newPosition.x += horizontalInput * moveSpeed; // Increase X position based on joystick's horizontal input

        // Update the camera position
        mainCameraParent.transform.position = newPosition;
    }

    // Called when the CameraPosition toggle is changed
    private void OnCameraPositionToggleChanged(bool isOn)
    {
        if (isOn)
        {
            CameraRotation.isOn = false; // Ensure the other toggle is off
        }
    }

    // Called when the CameraRotation toggle is changed
    private void OnCameraRotationToggleChanged(bool isOn)
    {
        if (isOn)
        {
            CameraPosition.isOn = false; // Ensure the other toggle is off
        }
    }

    private void OnDestroy()
    {
        // Clean up the listeners when the object is destroyed
        CameraPosition.onValueChanged.RemoveListener(OnCameraPositionToggleChanged);
        CameraRotation.onValueChanged.RemoveListener(OnCameraRotationToggleChanged);
    }
}
