using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public VariableJoystick joystick;           // Reference to your joystick
    public GameObject mainCameraParent;         // Reference to the camera parent
    public Toggle CameraPosition;               // Toggle for camera position
    public Toggle CameraRotation;               // Toggle for camera rotation
    public SpawningManager spawningManager;

    [SerializeField] private float moveSpeed = 0.05f;     // Movement speed
    [SerializeField] private float rotationSpeed = 0.5f;  // Speed for camera rotation
    [SerializeField] private float zoomInSpeed = 0.05f;   // Zooming in speed
    [SerializeField] private float zoomOutSpeed = 0.05f;  // Zooming out speed
    [SerializeField] private float minZoom = 20.0f;       // Minimum zoom level
    [SerializeField] private float maxZoom = 90.0f;       // Maximum zoom level
    [SerializeField] private float zoomTolerance = 10f;   // Tolerance for zoom gestures
    [SerializeField] private float moveTolerance = 0.8f;  // Tolerance for movement similarity
    [SerializeField] private float pitchTolerance = 0.05f;// Tolerance for distinguishing gestures

    [SerializeField] private float maxZPosition = 10.38793f; // Maximum Z-axis position
    [SerializeField] private float minXPosition = -8f;       // Minimum X-axis position
    [SerializeField] private float maxXPosition = 8f;        // Maximum X-axis position
    [SerializeField] private float minZPosition = -10f;      // Minimum Z-axis position
    [SerializeField] private float minPitch = 10f;          // Minimum pitch angle (downward limit)
    [SerializeField] private float maxPitch = 90f;           // Maximum pitch angle (upward limit)

    public TMP_Text tempText;

    private Vector3 startPos;                // Initial camera position
    private Quaternion startRot;             // Initial camera rotation
    private float startFOV;
    private float currentPitch = 0f;         // Current pitch of the camera

    // Gesture state management
    private enum GestureState { None, Zooming, Moving, Rotating }
    private GestureState currentGesture = GestureState.None;

    void Start()
    {
        // Set the initial camera position and rotation
        startPos = mainCameraParent.transform.position;
        startRot = mainCameraParent.transform.rotation;
        startFOV = mainCameraParent.GetComponentInChildren<Camera>().fieldOfView;
        currentPitch = mainCameraParent.transform.eulerAngles.x;
    }

    void Update()
    {
        // Handle touch input based on the number of touches
        if (Input.touchCount == 2)
        {
            // Handle two-finger gestures for zooming and moving
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            // Handle zoom and move with two touches
            ZoomCamera(touch0, touch1);
            MoveCameraWithTwoTouches(touch0, touch1);
        }
        else if (Input.touchCount == 3)
        {
            // Handle rotation with three fingers
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);
            var touch2 = Input.GetTouch(2);

            RotateCameraWithThreeTouches(touch0, touch1, touch2);
        }
        else
        {
            // Reset the gesture state when not enough touches are active
            currentGesture = GestureState.None;
        }
    }

    // Move the camera with two touches
    private void MoveCameraWithTwoTouches(Touch touch0, Touch touch1)
    {
        Vector2 averageDelta = (touch0.deltaPosition + touch1.deltaPosition) / 2f;

        Vector3 newPosition = mainCameraParent.transform.position;
        newPosition += mainCameraParent.transform.right * (-averageDelta.x * moveSpeed * Time.deltaTime);
        newPosition += mainCameraParent.transform.forward * (-averageDelta.y * moveSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, minXPosition, maxXPosition);
        newPosition.z = Mathf.Clamp(newPosition.z, minZPosition, maxZPosition);

        mainCameraParent.transform.position = newPosition;
    }

    // Rotate the camera with three touches
    private void RotateCameraWithThreeTouches(Touch touch0, Touch touch1, Touch touch2)
    {
        // Average the delta positions of all three touches
        Vector2 averageDelta = (touch0.deltaPosition + touch1.deltaPosition + touch2.deltaPosition) / 3f;

        // Rotate the camera horizontally around the Y-axis based on the average delta
        float rotationAmount = averageDelta.x * rotationSpeed * Time.deltaTime;
        mainCameraParent.transform.Rotate(Vector3.up, rotationAmount, Space.World);

        // Rotate vertically (pitch) based on Y-axis movement (tilt), with clamping
        float pitchAmount = -averageDelta.y * rotationSpeed * Time.deltaTime;
        currentPitch += pitchAmount;

        // Clamp the pitch angle to avoid extreme tilting
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        mainCameraParent.transform.localEulerAngles = new Vector3(currentPitch, mainCameraParent.transform.localEulerAngles.y, mainCameraParent.transform.localEulerAngles.z);

        tempText.text = $"Camera Rotation: {mainCameraParent.transform.rotation.eulerAngles}";
    }

    // Zoom the camera
    private void ZoomCamera(Touch touch0, Touch touch1)
    {
        var currentDistance = Vector2.Distance(touch0.position, touch1.position);
        var previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

        var deltaMagnitude = previousDistance - currentDistance;

        var newFOV = mainCameraParent.GetComponentInChildren<Camera>().fieldOfView + deltaMagnitude * (deltaMagnitude > 0 ? zoomOutSpeed : zoomInSpeed);
        mainCameraParent.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
    }

    public void ResetCamera()
    {
        mainCameraParent.transform.position = startPos;
        mainCameraParent.transform.rotation = startRot;
        mainCameraParent.GetComponentInChildren<Camera>().fieldOfView = startFOV;
        currentPitch = startRot.eulerAngles.x; // Reset the pitch angle
    }
}
