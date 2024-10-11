using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For Button interactions

public class ArrowControl : MonoBehaviour
{
    public GameObject arrow; // Reference to the arrow GameObject
    public Button floorButton; // Reference to the floor button's Button component
    public GameObject targetObject1; // First target object after floor button
    public GameObject targetObject2; // Second target object after reaching first
    public RectTransform wallButton; // Reference to the wall button's RectTransform
    public GameObject wall; // Reference to the wall GameObject

    public float speed = 2f; // Movement speed of the arrow
    private float currentSpeed = 0f; // Current speed of the arrow
    public float acceleration = 1f; // Rate of acceleration
    public bool isMoving = false; // Is the arrow currently moving

    private int movementPhase = 0; // To track movement between different phases

    void Start()
    {
        // Set up button click listener for the floor button
        floorButton.onClick.AddListener(OnFloorButtonClick);

        // Start by moving the arrow to the floor button
        MoveToFloorButton();
    }

    void Update()
    {
        // Keep the arrow moving during the various phases of movement
        if (isMoving)
        {
            switch (movementPhase)
            {
                case 0:
                    Debug.Log("phase 0 ");
                    MoveTowardsPoint(floorButton.transform.position);
                    break;
                case 1:
                    Debug.Log("phase 1 ");
                    MoveTowardsPoint(targetObject1.transform.position);
                    break;
                case 2:
                    Debug.Log("phase 2 ");
                    MoveTowardsPoint(targetObject2.transform.position);
                    break;
                case 3:
                    MoveTowardsPoint(wallButton.position);
                    break;
                case 4:
                    MoveTowardsPoint(wall.transform.position);
                    break;
            }
        }

    }

    private void MoveTowardsPoint(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - arrow.transform.position;
        float distance = direction.magnitude;

        // If the arrow has not yet reached the target
        if (distance > 0.1f)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, speed);
            arrow.transform.position += direction.normalized * currentSpeed * Time.deltaTime;

            // Rotate the arrow to point towards the target
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
            arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, targetRotation, Time.deltaTime * speed);
        }
        else
        {
            // When the arrow reaches the target, trigger the next action

            OnReachedTarget();
        }
    }

    private void OnReachedTarget()
    {
        isMoving = false;
        currentSpeed = 0f;

        switch (movementPhase)
        {
            case 0:
                // Arrow reached the floor button, waiting for the floor button click
                Debug.Log("Reached Floor Button. Waiting for click...");
                break;

            case 1:
                // Arrow reached targetObject1, automatically move to targetObject2
                Debug.Log("Reached TargetObject1.");
                // Move to next phase (targetObject2)
                DragandThanMove();
                //  MoveToTargetObject2();
                break;

            case 2:
                // Arrow reached targetObject2, now move towards the wall button
                Debug.Log("Reached TargetObject2.");
                movementPhase = 3; // Move to next phase (wallButton)
                MoveToWallButton();
                break;

            case 3:
                // Arrow reached the wall button, simulate the click and move to drag the wall
                Debug.Log("Wall button clicked.");
                movementPhase = 4; // Move to next phase (drag the wall)
                MoveToWall();
                break;

            case 4:
                // Arrow reached the wall and will drag it
                Debug.Log("Reached Wall, dragging...");
                // Implement your wall dragging logic here
                break;
        }
    }

    // Method to start moving the arrow to the floor button
    private void MoveToFloorButton()
    {
        movementPhase = 0;
        isMoving = true;
    }

    // Method triggered when the floor button is clicked
    private void OnFloorButtonClick()
    {
        Debug.Log("Floor Button Clicked!");
        movementPhase = 1; // Move to the first target after button click
        MoveToTargetObject1();
    }
    private void DragandThanMove()
    {
#if UNITY_EDITOR
        isMoving = true;
        Debug.Log("Get mouse up");
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Get mouse up 1");
            // Simulate touch phase for editor (mouse input)
            //  Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
            // transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
            // movementPhase = 2;
            MoveToTargetObject2();


        }
        if (Input.GetMouseButtonUp(0))
        {
            // Get the mouse position in screen coordinates
            Vector3 mousePosition = Input.mousePosition;

            // Create a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            // Perform a raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Get the point where the ray hit a collider
                Vector3 hitPoint = hit.point;

                // Assign the hit point to the UI element's position
                if (targetObject2 != null)
                {
                    // Convert the world position to the UI canvas space
                    Vector3 uiPosition = Camera.main.WorldToScreenPoint(hitPoint);

                    // Update the position of the UI element
                    targetObject2.transform.position = uiPosition;

                    // Log the new UI position
                    Debug.Log("Assigned UI element to position: " + uiPosition);
                    movementPhase = 2;
                }
            }
            else
            {
                Debug.Log("No hit detected.");
            }
        }
#else
        // Mobile device input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
                transform.position = new Vector3(touchPosition.x, touchPosition.y, transform.position.z);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                movementPhase = 2;
                MoveToTargetObject2();
            }
        }
#endif
    }

    // Method to move the arrow to targetObject1 after clicking the floor button
    private void MoveToTargetObject1()
    {
        isMoving = true;
    }

    // Method to move the arrow to targetObject2 after reaching targetObject1
    private void MoveToTargetObject2()
    {
        isMoving = true;
    }

    // Method to move the arrow towards the wall button after dragging mouse
    public void MoveToWallButton()
    {
        isMoving = true;
    }

    // Method to move the arrow towards the wall and drag it
    private void MoveToWall()
    {
        isMoving = true;
    }
}
