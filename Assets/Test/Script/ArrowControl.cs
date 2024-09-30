using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowControl : MonoBehaviour
{
    public GameObject arrow; // Reference to the arrow GameObject
    public RectTransform button; // Reference to the button's RectTransform
    public RectTransform FloorDrag;
    public float speed = 2f; // Maximum speed of the arrow
    private float currentSpeed; // Current speed of the arrow
    public float acceleration = 1f; // Rate of acceleration
    private bool hasReachedTarget = false; // Flag to check if the arrow has reached the target position

    // Tilting parameters
    public float tiltAngle = 15f; // Maximum tilt angle
    public float tiltDuration = 0.5f; // Duration of one tilt cycle
    private Coroutine tiltCoroutine; // Reference to the tilt coroutine
    public bool FloorButton=false;

    void Update()
    {
        if (FloorButton == false)
        {
            // Calculate the direction to the button
            Vector3 direction = button.position - arrow.transform.position;

            // Normalize the direction to get a unit vector
            Vector3 normalizedDirection = direction.normalized;

            // If the arrow is not close enough to the button and has not yet reached the target, move towards it
            if (direction.magnitude > 0.1f && !hasReachedTarget)
            {
                // Accelerate towards the maximum speed
                currentSpeed += acceleration * Time.deltaTime;
                currentSpeed = Mathf.Min(currentSpeed, speed); // Cap the speed

                // Move the arrow
                arrow.transform.position += normalizedDirection * currentSpeed * Time.deltaTime;

                // Rotate the arrow to point towards the button
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, normalizedDirection);
                arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, targetRotation, Time.deltaTime * speed);
            }
            else if (!hasReachedTarget) // If the arrow has reached the target position
            {
                // Set the arrow's new Y position (2 units lower than the button's Y)
                Vector3 targetPosition = button.position;
                targetPosition.y -= 12; // Lower the Y position by 2 units

                // Set the arrow's position to the new target position
                arrow.transform.position = targetPosition;

                // Set the flag to true to prevent further adjustments
                hasReachedTarget = true;

                // Start the continuous tilting coroutine
                if (tiltCoroutine != null) StopCoroutine(tiltCoroutine); // Stop any existing tilt coroutine
                tiltCoroutine = StartCoroutine(TiltArrow()); // Start the tilting coroutine
                FloorButton = true;
            }
        }
        else if(FloorButton == true)
        {
            Vector3 direction = FloorDrag.position - arrow.transform.position;

            // Normalize the direction to get a unit vector
            Vector3 normalizedDirection = direction.normalized;
            Debug.Log("Direction " + direction);
        }
    }

    private IEnumerator TiltArrow()
    {
        while (true) // Keep tilting indefinitely
        {
            // Tilt left
            for (float time = 0; time < tiltDuration; time += Time.deltaTime)
            {
                float angle = Mathf.Lerp(0, -tiltAngle, time / tiltDuration); // Tilt left
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate the arrow
                yield return null; // Wait for the next frame
            }

            // Tilt right
            for (float time = 0; time < tiltDuration; time += Time.deltaTime)
            {
                float angle = Mathf.Lerp(-tiltAngle, 0, time / tiltDuration); // Tilt back to upright
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate the arrow
                yield return null; // Wait for the next frame
            }

            // Repeat tilting to the right
            for (float time = 0; time < tiltDuration; time += Time.deltaTime)
            {
                float angle = Mathf.Lerp(0, tiltAngle, time / tiltDuration); // Tilt right
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate the arrow
                yield return null; // Wait for the next frame
            }

            // Tilt back to upright
            for (float time = 0; time < tiltDuration; time += Time.deltaTime)
            {
                float angle = Mathf.Lerp(tiltAngle, 0, time / tiltDuration); // Tilt back to upright
                arrow.transform.rotation = Quaternion.Euler(0, 0, angle); // Rotate the arrow
                yield return null; // Wait for the next frame
            }
        }
    }
}
