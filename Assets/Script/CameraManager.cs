using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public VariableJoystick joystick;          // Reference to your joystick
    public GameObject mainCameraParent;        // Reference to the camera parent
    public Toggle CameraPosition;              // Toggle for camera position
    public Toggle CameraRotation;
    public SpawningManager spawningManager;
    private Vector3 startPos;                  // Initial camera position

    [SerializeField] private float moveSpeed = 0.05f;   // Adjust this value to change movement speed
    [SerializeField] private float rotationSpeed = 50f; // Speed for camera rotation
    [SerializeField] private float maxZPosition = 10.38793f; // Maximum Z-axis position
    [SerializeField] private float minXPosition = -8f;       // Minimum X-axis position
    [SerializeField] private float maxXPosition = 8f;        // Maximum X-axis position
    [SerializeField] private float minZPosition = -10f;      // Minimum Z-axis position


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
        if (CameraRotation.isOn)
        {
            // Rotate the camera based on joystick input
            RotateCamera();
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

        // Restrict Z position to the specified maximum value
        if (newPosition.z > maxZPosition)
        {
            newPosition.z = maxZPosition; // Clamp Z position
        }

        if (newPosition.z < minZPosition)
        {
            newPosition.z = minZPosition; // Clamp Z position
        }

        // Restrict X position between the minimum and maximum values
        if (newPosition.x < minXPosition)
        {
            newPosition.x = minXPosition; // Clamp X position
        }
        else if (newPosition.x > maxXPosition)
        {
            newPosition.x = maxXPosition; // Clamp X position
        }

        // Update the camera position
        mainCameraParent.transform.position = newPosition;
    }

    private void RotateCamera()
    {
        // Get joystick input for rotation
        float horizontalInput = joystick.Horizontal; // Joystick movement along the X-axis
        float verticalInput = joystick.Vertical;     // Joystick movement along the Y-axis

        // Rotate the camera on the Y-axis (left/right) when joystick is moved horizontally (X-axis)
        mainCameraParent.transform.Rotate(0f, horizontalInput * rotationSpeed * Time.deltaTime, 0f, Space.World);

        // Get current X rotation in local space
        float currentXRotation = mainCameraParent.transform.localEulerAngles.x;

        // Adjust X rotation for clamping (Unity represents rotations in 0-360, so we convert to -180 to 180 range)
        if (currentXRotation > 180f)
        {
            currentXRotation -= 360f;
            Debug.Log("Current Rotation"+currentXRotation);
        }

        // Calculate the new X rotation after applying the joystick input
        float newXRotation = currentXRotation + verticalInput * rotationSpeed * Time.deltaTime;

        // Clamp the X rotation between -60 and 0 degrees
        newXRotation = Mathf.Clamp(newXRotation, -55f, 0f);

        // Apply the clamped X rotation
        mainCameraParent.transform.localEulerAngles = new Vector3(newXRotation, mainCameraParent.transform.localEulerAngles.y, 0f);
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
