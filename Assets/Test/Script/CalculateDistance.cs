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
    public LineRenderer lineRenderer;
    public TextMeshProUGUI distanceText; // UI element to show distance (optional)

    private GameObject placedObject;

    void Start()
    {
        // Initialize the LineRenderer component
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set up the line renderer's appearance
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
    }

    // Call this method after placing the object
    public void CalculateDistances(GameObject placedObject)
    {
        this.placedObject = placedObject;
        Debug.Log("Object Place" + placedObject.gameObject.name);

        Vector3 objectPosition = placedObject.transform.position;
        Vector3 closestPoint = Vector3.zero;
        float shortestDistance = float.MaxValue;

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

        // Draw a line between the placed object and the closest point (wall, floor, or ground)
        DrawLine(objectPosition, closestPoint);
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

    private void DrawLine(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}
