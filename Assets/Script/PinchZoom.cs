using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Camera mainCamera;  // Assign the camera in the inspector
    public float zoomInSpeed = 0.05f;   // Speed of zooming in
    public float zoomOutSpeed = 0.2f;   // Speed of zooming out
    public float minZoom = 30.0f;    // Minimum zoom level
    public float maxZoom = 90.0f;   // Maximum zoom level
    public float pitchTolerance = 0.1f; // Tolerance for pitching action

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.touchCount == 2) // Only process if there are two fingers touching the screen
        {
            // Get the first and second touches
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            // Find the magnitude of the vector (distance) between the touches in each frame
            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            // Find the difference in the distances between each frame
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // Check if the delta magnitude difference exceeds the tolerance
            if (Mathf.Abs(deltaMagnitudeDiff) > pitchTolerance)
            {
                // Determine the zoom speed based on whether we're zooming in or out
                float speed = deltaMagnitudeDiff > 0 ? zoomOutSpeed : zoomInSpeed;

                // Zoom the camera's field of view based on the difference in distance between the touches
                float newFOV = mainCamera.fieldOfView + deltaMagnitudeDiff * speed;

                // Clamp the field of view to the min and max values
                mainCamera.fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
            }
        }
    }
}