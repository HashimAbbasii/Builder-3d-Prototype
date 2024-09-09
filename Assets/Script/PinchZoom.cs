using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Camera mainCamera;  // Assign the camera in the inspector
    public float zoomInSpeed = 0.05f;   // Speed of zooming in
    public float zoomOutSpeed = 0.2f;   // Speed of zooming out
    public float minZoom = 2.0f;    // Minimum zoom level
    public float maxZoom = 15.0f;   // Maximum zoom level

    private Vector3 lastMousePosition;

    void Update()
    {
#if UNITY_EDITOR
        // Simulate touch input using the mouse in the editor
        if (Input.GetMouseButton(0))  // Left mouse button is held down
        {
            // Simulate the movement of two touches
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else
            {
                Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;

                // Use the vertical mouse movement to simulate pinch zoom
                float deltaMagnitudeDiff = deltaMousePosition.y;

                // Determine the zoom speed based on whether we're zooming in or out
                float speed = deltaMagnitudeDiff > 0 ? zoomOutSpeed : zoomInSpeed;

                // Zoom the camera's field of view based on the simulated pinch
                float newFOV = GetComponent<Camera>().fieldOfView + deltaMagnitudeDiff * speed;

                // Clamp the field of view to the min and max values
                GetComponent<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);

                lastMousePosition = Input.mousePosition;
            }
        }
#else
        // Touch input on mobile devices
        if (Input.touchCount == 2)
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

            // Determine the zoom speed based on whether we're zooming in or out
            float speed = deltaMagnitudeDiff > 0 ? zoomOutSpeed : zoomInSpeed;

            // Zoom the camera's field of view based on the difference in distance between the touches
            float newFOV = GetComponent<Camera>().fieldOfView + deltaMagnitudeDiff * speed;

            // Clamp the field of view to the min and max values
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
#endif
    }
}
