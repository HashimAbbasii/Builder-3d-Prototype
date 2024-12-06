using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector2 lastSingleTouchPosition;
    private float initialFieldOfView;
    private Quaternion targetRotation;

    private Vector2 lastMousePosition;

    void Start()
    {
        if (parentTransform == null)
        {
            parentTransform = transform.parent;
        }

        targetRotation = parentTransform.rotation;
        lastMousePosition = Input.mousePosition;
        initialFieldOfView = mainCamera.fieldOfView;
    }

    void Update()
    {
        HandleTouchInput();
        SmoothRotation();
    }

    void HandleTouchInput()
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

    // Centralized method for rotation to ensure consistency
    void ApplyRotation(Vector2 rotationDelta)
    {
        float horizontalRotation = -rotationDelta.x * rotationSensitivity;
        float verticalRotation = rotationDelta.y * rotationSensitivity;

        Quaternion yawRotation = Quaternion.Euler(0, horizontalRotation, 0);
        Quaternion pitchRotation = Quaternion.Euler(-verticalRotation, 0, 0);

        targetRotation *= yawRotation * pitchRotation;

        // Ensure rotation stays within limits
        Vector3 currentAngles = targetRotation.eulerAngles;

        // Convert 0-360 range to signed angle
        float currentX = currentAngles.x > 180 ? currentAngles.x - 360 : currentAngles.x;
        float currentY = currentAngles.y > 180 ? currentAngles.y - 360 : currentAngles.y;

        // Clamp both X and Y rotations
        currentX = Mathf.Clamp(currentX, minRotationAngle.x, maxRotationAngle.x);
        currentY = Mathf.Clamp(currentY, minRotationAngle.y, maxRotationAngle.y);

        // Reconstruct the rotation with clamped X and Y
        targetRotation = Quaternion.Euler(currentX, currentY, currentAngles.z);
    }

#if UNITY_EDITOR
    void HandleEditorInput()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currentMousePosition = Input.mousePosition;
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            Vector3 moveDirection =
                parentTransform.right * mouseDelta.x * moveSensitivity +
                parentTransform.up * mouseDelta.y * moveSensitivity;

            parentTransform.position += moveDirection * Time.deltaTime;
            lastMousePosition = currentMousePosition;
        }

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            float zoomAmount = -scrollDelta * zoomSensitivity * 100f;
            float newFieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomAmount, minZoom, maxZoom);
            mainCamera.fieldOfView = newFieldOfView;
        }

        if (Input.GetMouseButton(1))
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

    void HandleSingleFingerMovement()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                lastSingleTouchPosition = touch.position;
                break;

            case TouchPhase.Moved:
                Vector2 touchDelta = touch.position - lastSingleTouchPosition;

                Vector3 moveDirection =
                    parentTransform.right * touchDelta.x * moveSensitivity +
                    parentTransform.up * touchDelta.y * moveSensitivity;

                parentTransform.position += moveDirection * Time.deltaTime;
                lastSingleTouchPosition = touch.position;
                break;
        }
    }

    void HandleTwoFingerZoom()
    {
        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);

        if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
        {
            initialFieldOfView = mainCamera.fieldOfView;
        }

        if (touch0.phase == TouchPhase.Moved && touch1.phase == TouchPhase.Moved)
        {
            float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
            float initialPinchDistance = Vector2.Distance(
                touch0.position - touch0.deltaPosition,
                touch1.position - touch1.deltaPosition
            );

            float pinchDelta = initialPinchDistance - currentPinchDistance;
            float zoomAmount = pinchDelta * zoomSensitivity;
            float newFieldOfView = Mathf.Clamp(mainCamera.fieldOfView + zoomAmount, minZoom, maxZoom);

            mainCamera.fieldOfView = newFieldOfView;
        }
    }

    void HandleThreeFingerRotation()
    {
        if (Input.touchCount != 3) return;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        Touch touch2 = Input.GetTouch(2);

        if (touch0.phase != TouchPhase.Moved &&
            touch1.phase != TouchPhase.Moved &&
            touch2.phase != TouchPhase.Moved)
            return;

        Vector2 rotationDelta = new Vector2(
            (touch0.deltaPosition.x + touch1.deltaPosition.x + touch2.deltaPosition.x) / 3f,
            (touch0.deltaPosition.y + touch1.deltaPosition.y + touch2.deltaPosition.y) / 3f
        );

        ApplyRotation(rotationDelta);
    }

    void SmoothRotation()
    {
        parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetRotation, Time.deltaTime * rotationSmoothing);
    }

    public void ResetCamera()
    {
        // Optionally reset position to zero
        parentTransform.position = Vector3.zero;

        // Reset field of view
        mainCamera.fieldOfView = initialFieldOfView;
    }
}