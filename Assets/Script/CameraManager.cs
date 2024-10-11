using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static uMouseLook;

public class CameraManager : MonoBehaviour
{
    public VariableJoystick joystick;           // Reference to your joystick
    public GameObject mainCameraParent;         // Reference to the camera parent
    public Toggle CameraPosition;               // Toggle for camera position
    public Toggle CameraRotation;               // Toggle for camera rotation
    public SpawningManager spawningManager;

    [SerializeField] private float moveSpeed = 0.05f;     // Movement speed
    [SerializeField] private float rotationSpeed = 0.5f;    // Speed for camera rotation
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

    public TMP_Text tempText;

    private Vector3 startPos;                // Initial camera position
    private Quaternion startRot;             // Initial camera rotation

    // Gesture state management
    private enum GestureState { None, Zooming, Moving, Rotating }
    private GestureState currentGesture = GestureState.None;

    void Start()
    {
        // Set the initial camera position and rotation
        startPos = mainCameraParent.transform.position;
        startRot = mainCameraParent.transform.rotation;

        // Set up listeners for the toggles
        CameraPosition.onValueChanged.AddListener(OnCameraPositionToggleChanged);
        CameraRotation.onValueChanged.AddListener(OnCameraRotationToggleChanged);
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            if (currentGesture == GestureState.None)
            {
                // Determine the type of gesture and lock in the state
                if (IsZoomGesture(touch0, touch1))
                {
                    currentGesture = GestureState.Zooming;
                }
                else if (IsMovementGesture(touch0, touch1))
                {
                    currentGesture = GestureState.Moving;
                }
                else if (IsRotationGesture(touch0, touch1))
                {
                    currentGesture = GestureState.Rotating;
                }
            }

            // Execute the appropriate gesture based on the current state
            if (currentGesture == GestureState.Zooming)
            {
                ZoomCamera(touch0, touch1);
            }
            else if (currentGesture == GestureState.Moving)
            {
                MoveCameraWithTwoTouches(touch0, touch1);
            }
            else if (currentGesture == GestureState.Rotating)
            {
                RotateCameraWithTwoTouches(touch0, touch1);
            }
        }
        else
        {
            // Reset the gesture state when all touches are released
            currentGesture = GestureState.None;
        }
    }

    // Check if the gesture is a zoom gesture
    private bool IsZoomGesture(Touch touch0, Touch touch1)
    {
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        float previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

        float distanceDelta = Mathf.Abs(currentDistance - previousDistance);

        // Require a larger difference in the distance to trigger zoom
        return distanceDelta > zoomTolerance;
    }

    // Check if the gesture is a movement gesture
    private bool IsMovementGesture(Touch touch0, Touch touch1)
    {
        // Ensure both fingers move in almost the same direction for it to be considered movement
        return Vector2.Dot(touch0.deltaPosition.normalized, touch1.deltaPosition.normalized) > moveTolerance;
    }

    // Check if the gesture is a rotation gesture
    private bool IsRotationGesture(Touch touch0, Touch touch1)
    {
        float angleDelta = Vector2.SignedAngle(touch0.deltaPosition, touch1.deltaPosition);

        // If the fingers move in opposite directions with significant angular difference, it's a rotation gesture
        return Mathf.Abs(angleDelta) > pitchTolerance * 100;
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

    private Vector2 prevTouchDelta;

    // Rotate the camera with two touches
    private void RotateCameraWithTwoTouches(Touch touch0, Touch touch1)
    {
        var touch0PrevPos = touch0.position - touch0.deltaPosition;
        var touch1PrevPos = touch1.position - touch1.deltaPosition;

        prevTouchDelta = touch0PrevPos - touch1PrevPos;
        var touchDelta = touch0.position - touch1.position;

        var angleDelta = Vector2.SignedAngle(prevTouchDelta, touchDelta);

        //float angleDelta = Vector2.SignedAngle(touch0.deltaPosition, touch1.deltaPosition);

        // Rotate the camera horizontally around the Y-axis based on the angle delta

        mainCameraParent.transform.Rotate(Vector3.up, angleDelta * rotationSpeed * Time.deltaTime, Space.World);

        tempText.text = $"Camera Rotation: {mainCameraParent.transform.rotation.eulerAngles}";
    }

    // Zoom the camera
    private void ZoomCamera(Touch touch0, Touch touch1)
    {
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);
        float previousDistance = Vector2.Distance(touch0.position - touch0.deltaPosition, touch1.position - touch1.deltaPosition);

        float deltaMagnitude = previousDistance - currentDistance;

        float newFOV = mainCameraParent.GetComponentInChildren<Camera>().fieldOfView + deltaMagnitude * (deltaMagnitude > 0 ? zoomOutSpeed : zoomInSpeed);
        mainCameraParent.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
    }

    // Called when the CameraPosition toggle is changed
    private void OnCameraPositionToggleChanged(bool isOn)
    {
        if (isOn)
        {
            CameraRotation.isOn = false;
        }
    }

    // Called when the CameraRotation toggle is changed
    private void OnCameraRotationToggleChanged(bool isOn)
    {
        if (isOn)
        {
            CameraPosition.isOn = false;
        }
    }

    private void OnDestroy()
    {
        CameraPosition.onValueChanged.RemoveListener(OnCameraPositionToggleChanged);
        CameraRotation.onValueChanged.RemoveListener(OnCameraRotationToggleChanged);
    }

    public void ResetCamera()
    {
        mainCameraParent.transform.position = startPos;
        mainCameraParent.transform.rotation = startRot;
    }
}
