using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private float rotationSpeed = 5f; // Speed for camera rotation
    [SerializeField] private float zoomInSpeed = 0.05f;   // Speed of zooming in
    [SerializeField] private float zoomOutSpeed = 0.05f;   // Speed of zooming out
    [SerializeField] private float minZoom = 20.0f;    // Minimum zoom level
    [SerializeField] private float maxZoom = 90.0f;   // Maximum zoom level
    [SerializeField] private float pitchTolerance = 1f; // Tolerance for pitching action

    [SerializeField] private float maxZPosition = 10.38793f; // Maximum Z-axis position
    [SerializeField] private float minXPosition = -8f;       // Minimum X-axis position
    [SerializeField] private float maxXPosition = 8f;        // Maximum X-axis position
    [SerializeField] private float minZPosition = -10f;      // Minimum Z-axis position

    public TMP_Text tempText;
    
    private Vector3 startPos;                  // Initial camera position
    private Quaternion startRot;                  // Initial camera position
    private Vector2 previousTouch0Position = Vector2.zero;
    private Vector2 previousTouch1Position = Vector2.zero;

    void Start()
    {
        // Set the initial camera position
        startPos = mainCameraParent.transform.position;
        startRot = mainCameraParent.transform.rotation;

        // Set up listeners for the toggles
        CameraPosition.onValueChanged.AddListener(OnCameraPositionToggleChanged);
        CameraRotation.onValueChanged.AddListener(OnCameraRotationToggleChanged);
    }

    void Update()
    {
        switch (Input.touchCount)
        {
            // Handle two-finger touch for movement, rotation, and zoom
            case 2:
            {
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);

                // Call methods based on toggle states
               
                
                MoveCameraWithTwoTouches(touch0, touch1);

                // Handle zoom functionality
                ZoomCamera(touch0, touch1);
                break;
            }
            case 3:
            {
                var touch0 = Input.GetTouch(0);
                var touch1 = Input.GetTouch(1);
                var touch2 = Input.GetTouch(2);
                RotateCameraWithThreeTouches(touch0, touch1, touch2);
                break;
            }
            default:
                // Reset touch tracking if not using two fingers
                break;
        }

#if UNITY_EDITOR
        ZoomCameraEditor();
#else
ZoomCamera(touch0, touch1);

# endif

    }

 

    private void ZoomCameraEditor()
    {
        // Simulate zoom using mouse scroll wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        // Check if there's any scroll input
        if (Mathf.Abs(scrollInput) > 0f)
        {
            // Determine the zoom speed based on scroll direction
            float speed = scrollInput > 0 ? zoomInSpeed : zoomOutSpeed;

            // Zoom the camera's field of view based on scroll input
            float newFOV = mainCameraParent.GetComponentInChildren<Camera>().fieldOfView - scrollInput * speed;

            // Clamp the field of view to the min and max values
            mainCameraParent.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
    }



    private void MoveCameraWithTwoTouches(Touch touch0, Touch touch1)
    {
        // Calculate the movement delta (average of the two touches' deltas)
        var touchDelta0 = touch0.deltaPosition;
        var touchDelta1 = touch1.deltaPosition;
        var averageDelta = (touchDelta0 + touchDelta1) / 2f;

        // Calculate new camera position
        var newPosition = mainCameraParent.transform.position;
        newPosition += mainCameraParent.transform.right * (-averageDelta.x * moveSpeed * Time.deltaTime); // Move along the camera's right axis
        newPosition += mainCameraParent.transform.forward * (-averageDelta.y * moveSpeed * Time.deltaTime); // Move along the camera's forward axis
        
        newPosition.x = Mathf.Clamp(newPosition.x, minXPosition, maxXPosition);
        newPosition.z = Mathf.Clamp(newPosition.z, minZPosition, maxZPosition);
        
        // Update camera position
        mainCameraParent.transform.position = newPosition;
    }

  
    
    
    
    private void RotateCameraWithThreeTouches(Touch touch0, Touch touch1, Touch touch2)
    {
        // Calculate the current positions of the three touches
        var currentTouch0Position = touch0.position;
        var currentTouch1Position = touch1.position;
        var currentTouch2Position = touch2.position;

        // Calculate the previous positions of the three touches
        var previousTouch0Position = touch0.position - touch0.deltaPosition;
        var previousTouch1Position = touch1.position - touch1.deltaPosition;
        var previousTouch2Position = touch2.position - touch2.deltaPosition;

        // Calculate the difference between the previous and current touch positions
        var touch0Delta = currentTouch0Position - previousTouch0Position;
        var touch1Delta = currentTouch1Position - previousTouch1Position;
        var touch2Delta = currentTouch2Position - previousTouch2Position;

        // Calculate the average touch delta
        var averageDelta = (touch0Delta + touch1Delta + touch2Delta) / 3f;

        // Rotate the camera based on the average delta
        if (Mathf.Abs(averageDelta.x) > Mathf.Abs(averageDelta.y))
        {
            // Horizontal movement, rotate around Y axis
            mainCameraParent.transform.Rotate(Vector3.up, averageDelta.x * rotationSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Vertical movement, rotate around X axis
            var newXRotation = mainCameraParent.transform.eulerAngles.x - averageDelta.y * rotationSpeed * Time.deltaTime;
            newXRotation = Mathf.Clamp(newXRotation, 16, 90);
            mainCameraParent.transform.eulerAngles = new Vector3(newXRotation, mainCameraParent.transform.eulerAngles.y, mainCameraParent.transform.eulerAngles.z);
            tempText.text = mainCameraParent.transform.eulerAngles.ToString();
        }


    }

    private void ZoomCamera(Touch touch0, Touch touch1)
    {
        // Find the position in the previous frame of each touch
        var touch1PrevPos = touch0.position - touch0.deltaPosition;
        var touch2PrevPos = touch1.position - touch1.deltaPosition;

        // Find the magnitude of the vector (distance) between the touches in each frame
        var prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
        var touchDeltaMag = (touch0.position - touch1.position).magnitude;

        // Find the difference in the distances between each frame
        var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        Debug.Log(deltaMagnitudeDiff);

        // Check if the delta magnitude difference exceeds the tolerance
        if (!(Mathf.Abs(deltaMagnitudeDiff) > pitchTolerance)) return;
        
        // Determine the zoom speed based on whether we're zooming in or out
        var speed = deltaMagnitudeDiff > 0 ? zoomOutSpeed : zoomInSpeed;

        // Zoom the camera's field of view based on the difference in distance between the touches
        var newFOV = mainCameraParent.GetComponentInChildren<Camera>().fieldOfView + deltaMagnitudeDiff * speed;

        // Clamp the field of view to the min and max values
        mainCameraParent.GetComponentInChildren<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
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

    public void ResetCamera()
    {
        mainCameraParent.transform.position = startPos;
        mainCameraParent.transform.rotation = startRot;
    }
    
}
