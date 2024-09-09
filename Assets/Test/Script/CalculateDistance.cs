using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalculateDistance : MonoBehaviour
{
    public LayerMask wallLayer;
    public LayerMask floorLayer;
    public LayerMask grassGroundLayer;
    public TextMeshProUGUI distanceText; // UI element to show distance (optional)
    private List<GameObject> placedObjects = new List<GameObject>();

    void Start()
    {
        // No need to set up the line renderer here since each object will have its own
    }

    // Call this method after placing the object
    public void CalculateDistances(GameObject placedObject)
    {
        if (placedObject == null)
        {
            Debug.LogError("Placed object is null.");
            return;
        }

        LineRenderer lineRenderer = placedObject.GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = placedObject.AddComponent<LineRenderer>();
        }

        // Set up the line renderer's appearance
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;

        Vector3 objectPosition = placedObject.transform.position;
        Vector3 closestPoint = Vector3.zero;
        float shortestDistance = float.MaxValue;

        // Check distance from wall first
        if (CheckDistanceToLayer(objectPosition, wallLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the wall.");
        }
        else if (CheckDistanceToLayer(objectPosition, floorLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the floor.");
        }
        else if (CheckDistanceToLayer(objectPosition, grassGroundLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the grass ground.");
        }

        // Display the distance in Unity
        DisplayDistance(shortestDistance);

        // Draw a line between the placed object and the closest point (wall, floor, or ground)
        DrawLine(lineRenderer, objectPosition, closestPoint);

        // Add the object to the list of placed objects
        placedObjects.Add(placedObject);
    }
    private bool CheckDistanceToLayer(Vector3 objectPosition, LayerMask layerMask, out Vector3 closestPoint, out float distance)
    {
        Collider[] colliders = Physics.OverlapSphere(objectPosition, 100f, layerMask); // Adjust the radius as needed
        closestPoint = Vector3.zero;
        distance = float.MaxValue;

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                Vector3 point = collider.ClosestPoint(objectPosition);
                float currentDistance = Vector3.Distance(objectPosition, point);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestPoint = point;
                }
            }
            return true;
        }
        return false;
    }

    private void DisplayDistance(float distance)
    {
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + distance.ToString("F2") + " units";
        }
        Debug.Log("Distance: " + distance.ToString("F2") + " units");
    }

    private void DrawLine(LineRenderer lineRenderer, Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.positionCount = 2;
        startPosition.y = 1.0f;
        endPosition.y = 1.0f;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    // Method to recalculate the distance and update the LineRenderer when selecting an object
    public void RecalculateDistanceForSelectedObject(GameObject selectedObject)
    {
        Vector3 objectPosition = selectedObject.transform.position;
        Vector3 closestPoint = Vector3.zero;
        float shortestDistance = float.MaxValue;

        LineRenderer lineRenderer = selectedObject.GetComponent<LineRenderer>();

        // Check distance from wall first
        if (CheckDistanceToLayer(objectPosition, wallLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the wall.");
        }
        // If no wall, check distance from floor
        else if (CheckDistanceToLayer(objectPosition, floorLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the floor.");
        }
        // If no wall or floor, check distance from grass ground
        else if (CheckDistanceToLayer(objectPosition, grassGroundLayer, out closestPoint, out shortestDistance))
        {
            Debug.Log("Nearest object is the grass ground.");
        }

        // Display the distance in Unity
        DisplayDistance(shortestDistance);

        // Update the line renderer with the new closest point
        DrawLine(lineRenderer, objectPosition, closestPoint);
    }
}
