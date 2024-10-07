using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public VariableJoystick joystick;          // Reference to your joystick
    public GameObject mainCameraParent;        // Reference to the camera parent
    public Toggle CameraPosition;              // Toggle for camera position
    public Toggle CameraRotation;              // Toggle for camera rotation
    public SpawningManager spawningManager;

    [SerializeField] private float moveSpeed = 0.05f;   // Adjust this value to change movement speed
    [SerializeField] private float rotationSpeed = 50f; // Speed for camera rotation
    [SerializeField] private float zoomInSpeed = 0.05f;   // Speed of zooming in
    [SerializeField] private float zoomOutSpeed = 0.2f;   // Speed of zooming out
    [SerializeField] private float minZoom = 30.0f;    // Minimum zoom level
    [SerializeField] private float maxZoom = 90.0f;   // Maximum zoom level
    [SerializeField] private float pitchTolerance = 0.1f; // Tolerance for pitching action

    private Vector3 startPos;                  // Initial camera position
    private Vector2 previousTouch0Position = Vector2.zero;
    private Vector2 previousTouch1Position = Vector2.zero;
    private bool isTwoFingerTouching = false;

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
        if (Input.touchCount == 2) // Handle two-finger touch for movement, rotation, and zoom
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Call methods based on toggle states
            if (CameraPosition.isOn)
            {
                MoveCameraWithTwoTouches(touch0, touch1);
            }
            if (CameraRotation.isOn)
            {
                RotateCameraWithTwoTouches(touch0, touch1);
            }

            // Handle zoom functionality
            ZoomCamera(touch0, touch1);
        }
        else
        {
            // Reset touch tracking if not using two fingers
            isTwoFingerTouching = false;
        }
    }

    private void MoveCameraWithTwoTouches(Touch touch0, Touch touch1)
    {
        // Calculate the movement delta (average of the two touches' deltas)
        Vector2 touchDelta0 = touch0.deltaPosition;
        Vector2 touchDelta1 = touch1.deltaPosition;
        Vector2 averageDelta = (touchDelta0 + touchDelta1) / 2f;

        // Calculate new camera position
        Vector3 newPosition = mainCameraParent.transform.position;
        newPosition.x -= averageDelta.x * moveSpeed * Time.deltaTime; // Move along the X-axis
        newPosition.z -= averageDelta.y * moveSpeed * Time.deltaTime; // Move along the Z-axis

        // Update camera position
        mainCameraParent.transform.position = newPosition;
    }

    private void RotateCameraWithTwoTouches(Touch touch0, Touch touch1)
    {
        // Calculate the current distance between the two touches
        Vector2 currentTouch0Position = touch0.position;
        Vector2 currentTouch1Position = touch1.position;

        // Only track the first touch movement if we are already in a two-finger touch
        if (isTwoFingerTouching)
        {
            // Calculate the difference between the previous and current touch positions
            Vector2 touch0Delta = currentTouch0Position - previousTouch0Position;
            Vector2 touch1Delta = currentTouch1Position - previousTouch1Position;

            // Rotate the camera based on the average of both touch deltas
            mainCameraParent.transform.Rotate(0f, (touch0Delta.x + touch1Delta.x) / 2 * rotationSpeed * Time.deltaTime, 0f, Space.World);

            // Get current X rotation in local space and clamp
            float currentXRotation = mainCameraParent.transform.localEulerAngles.x;
            if (currentXRotation > 180f)
            {
                currentXRotation -= 360f;
            }
            float newXRotation = Mathf.Clamp(currentXRotation - ((touch0Delta.y + touch1Delta.y) / 2) * rotationSpeed * Time.deltaTime, -55f, 0f);

            // Apply the clamped rotation
            mainCameraParent.transform.localEulerAngles = new Vector3(newXRotation, mainCameraParent.transform.localEulerAngles.y, 0f);
        }

        // Store the current touch positions for the next frame
        previousTouch0Position = currentTouch0Position;
        previousTouch1Position = currentTouch1Position;
        isTwoFingerTouching = true;
    }

    private void ZoomCamera(Touch touch0, Touch touch1)
    {
        // Find the position in the previous frame of each touch
        Vector2 touch1PrevPos = touch0.position - touch0.deltaPosition;
        Vector2 touch2PrevPos = touch1.position - touch1.deltaPosition;

        // Find the magnitude of the vector (distance) between the touches in each frame
        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        float touchDeltaMag = (touch0.position - touch1.position).magnitude;

        // Find the difference in the distances between each frame
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        // Check if the delta magnitude difference exceeds the tolerance
        if (Mathf.Abs(deltaMagnitudeDiff) > pitchTolerance)
        {
            // Determine the zoom speed based on whether we're zooming in or out
            float speed = deltaMagnitudeDiff > 0 ? zoomOutSpeed : zoomInSpeed;

            // Zoom the camera's field of view based on the difference in distance between the touches
            float newFOV = mainCameraParent.GetComponent<Camera>().fieldOfView + deltaMagnitudeDiff * speed;

            // Clamp the field of view to the min and max values
            mainCameraParent.GetComponent<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
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
