using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinchZoom : MonoBehaviour
{
    public Camera mainCamera;  // Assign the camera in the inspector
    public float zoomSpeed = 0.1f;  // Speed of zoom
    public float minZoom = 2.0f;    // Minimum zoom level
    public float maxZoom = 15.0f;   // Maximum zoom level

    void Update()
    {
        // Check if there are two touches on the screen
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

            // Zoom the camera's field of view based on the difference in distance between the touches
            float newFOV = GetComponent<Camera>().fieldOfView + deltaMagnitudeDiff * zoomSpeed;

            // Clamp the field of view to the min and max values
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(newFOV, minZoom, maxZoom);
        }
    }
}



