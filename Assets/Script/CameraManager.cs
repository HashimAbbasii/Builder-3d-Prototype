using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    [Header("Camera References")]
    public Camera mainCamera;
    public Transform parentTransform;

    [Header("Touch Sensitivity")]
    [SerializeField] private float moveSensitivity = 0.5f;
    [SerializeField] private float rotationSensitivity = 0.5f;
    [SerializeField] private float zoomSensitivity = 0.1f;

    [Header("Camera Constraints")]
    [SerializeField] private float minZoom = 20f;
    [SerializeField] private float maxZoom = 100f;

    [Header("Rotation Limits")]
    [SerializeField] private Vector2 minRotationAngle = new Vector2(-90f, -90f);
    [SerializeField] private Vector2 maxRotationAngle = new Vector2(90f, 90f);

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothing = 5f;

    private Vector2 lastTouchPosition;
    private Quaternion targetRotation;

    private Vector2 lastMousePosition;

    // Reference to ObjectManipulator for checking if an object is selected
    private ObjectManipulator objectManipulator;

    void Start()
    {
        if (parentTransform == null)
        {
            parentTransform = transform.parent;
        }

        targetRotation = parentTransform.rotation;
        lastMousePosition = Input.mousePosition;

        objectManipulator = FindObjectOfType<ObjectManipulator>();
        if (objectManipulator == null)
        {
            Debug.LogError("ObjectManipulator script not found in the scene.");
        }
    }

    void Update()
    {
        // Check if pointer is over UI element
        if (IsPointerOverUIElement())
        {
            return;
        }

        // If an object is selected, allow only zooming and rotation
        if (objectManipulator != null && objectManipulator._isObjectSelected)
        {
            HandleZoomAndRotation();
        }
        else
        {
            HandleFullCameraControl();
        }

        SmoothRotation();
    }

    bool IsPointerOverUIElement()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the first touch is over a UI element
            return IsTouchOverUIElement(touch);
        }

        // Fallback to mouse check for editor/standalone
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Specific method to check if a touch is over a UI element
    bool IsTouchOverUIElement(Touch touch)
    {
        // Create a pointer event data for the touch position
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = touch.position;

        // Create a list to store raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the event system
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Return true if any UI element was hit
        return results.Count > 0;
    }



    void HandleFullCameraControl()
    {
#if UNITY_EDITOR
        HandleEditorInput();
#else
        if (Input.touchCount == 1)
        {
            HandleSingleFingerMovement();
        }
        else if (Input.touchCount == 2)
        {
            HandleTwoFingerZoom();
        }
        else if (Input.touchCount == 3)
        {
            HandleThreeFingerRotation();
        }
#endif
    }

    void HandleZoomAndRotation()
    {
#if UNITY_EDITOR
        HandleEditorZoomAndRotation();
#else
        if (Input.touchCount == 2)
        {
            HandleTwoFingerZoom();
        }
        else if (Input.touchCount == 3)
        {
            HandleThreeFingerRotation();
        }
#endif
    }

    // Smoothly interpolate camera rotation
    void SmoothRotation()
    {
        parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetRotation, Time.deltaTime * rotationSmoothing);
    }

    // Handles camera movement for mouse/keyboard input in the editor
#if UNITY_EDITOR
    void HandleEditorInput()
    {
        if (Input.GetMouseButton(0)) // Move camera
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            Vector3 moveDirection =
                parentTransform.right * mouseDelta.x * moveSensitivity +
                parentTransform.up * mouseDelta.y * moveSensitivity;

            parentTransform.position += moveDirection * Time.deltaTime;
            lastMousePosition = currentMousePosition;
        }

        HandleEditorZoomAndRotation();
    }

    void HandleEditorZoomAndRotation()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            float zoomAmount = -scrollDelta * zoomSensitivity * 100f;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomAmount, minZoom, maxZoom);
        }

        if (Input.GetMouseButton(1)) // Rotate camera
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            ApplyRotation(mouseDelta);

            lastMousePosition = currentMousePosition;
        }
        else
        {
            lastMousePosition = Input.mousePosition;
        }
    }
#endif

    // Handles single finger movement for touch input
    void HandleSingleFingerMovement()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                lastTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:
                Vector2 touchDelta = touch.position - lastTouchPosition;

                Vector3 moveDirection =
                    parentTransform.right * touchDelta.x * moveSensitivity +
                    parentTransform.up * touchDelta.y * moveSensitivity;

                parentTransform.position += moveDirection * Time.deltaTime;
                lastTouchPosition = touch.position;
                break;
        }
    }

    // Handles two-finger pinch zoom
    void HandleTwoFingerZoom()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
        {
            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            float previousPinchDistance = Vector2.Distance(
                touch0.position - touch0.deltaPosition,
                touch1.position - touch1.deltaPosition
            );

            float pinchDelta = previousPinchDistance - currentPinchDistance;
            float zoomAmount = pinchDelta * zoomSensitivity;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomAmount, minZoom, maxZoom);
        }
    }

    // Handles three-finger rotation
    void HandleThreeFingerRotation()
    {
        if (Input.touchCount != 3) return;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        Touch touch2 = Input.GetTouch(2);

        Vector2 rotationDelta = new Vector2(
            (touch0.deltaPosition.x + touch1.deltaPosition.x + touch2.deltaPosition.x) / 3f,
            (touch0.deltaPosition.y + touch1.deltaPosition.y + touch2.deltaPosition.y) / 3f
        );

        ApplyRotation(rotationDelta);
    }

    // Applies rotation to the camera
    void ApplyRotation(Vector2 rotationDelta)
    {
        float horizontalRotation = -rotationDelta.x * rotationSensitivity;
        float verticalRotation = rotationDelta.y * rotationSensitivity;

        Quaternion yawRotation = Quaternion.Euler(0, horizontalRotation, 0);
        Quaternion pitchRotation = Quaternion.Euler(-verticalRotation, 0, 0);

        targetRotation *= yawRotation * pitchRotation;

        Vector3 currentAngles = targetRotation.eulerAngles;

        float currentX = currentAngles.x > 180 ? currentAngles.x - 360 : currentAngles.x;
        float currentY = currentAngles.y > 180 ? currentAngles.y - 360 : currentAngles.y;

        currentX = Mathf.Clamp(currentX, minRotationAngle.x, maxRotationAngle.x);
        currentY = Mathf.Clamp(currentY, minRotationAngle.y, maxRotationAngle.y);

        targetRotation = Quaternion.Euler(currentX, currentY, currentAngles.z);
    }
}
