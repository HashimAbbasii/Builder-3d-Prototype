using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CalculateDistance : MonoBehaviour
{
    public LineHandler distanceLinePrefab;
    public LayerMask wallLayer;
    public LayerMask floorLayer;
    public LayerMask grassGroundLayer;
    public TextMeshProUGUI distanceText; // UI element to show distance (optional)
    public List<LineHandler> lines = new();
    
    private List<GameObject> _placedObjects = new();

    
    // Call this method after placing the object
    // public void CalculateDistances(GameObject placedObject)
    // {
    //     if (placedObject == null)
    //     {
    //         Debug.LogError("Placed object is null.");
    //         return;
    //     }
    //
    //     var lineRenderer = placedObject.GetComponent<LineRenderer>();
    //
    //     if (lineRenderer == null)
    //     {
    //         lineRenderer = placedObject.AddComponent<LineRenderer>();
    //     }
    //
    //     // Set up the line renderer's appearance
    //     lineRenderer.startWidth = 0.05f;
    //     lineRenderer.endWidth = 0.05f;
    //     lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    //     lineRenderer.startColor = Color.green;
    //     lineRenderer.endColor = Color.red;
    //
    //     var objectPosition = placedObject.transform.position;
    //     var closestPoint = Vector3.zero;
    //     var shortestDistance = float.MaxValue;
    //
    //     // Check distance from wall first
    //     if (CheckDistanceToLayer(objectPosition, wallLayer, out closestPoint, out shortestDistance))
    //     {
    //         //Debug.Log("Nearest object is the wall.");
    //     }
    //     else if (CheckDistanceToLayer(objectPosition, floorLayer, out closestPoint, out shortestDistance))
    //     {
    //         //Debug.Log("Nearest object is the floor.");
    //     }
    //     else if (CheckDistanceToLayer(objectPosition, grassGroundLayer, out closestPoint, out shortestDistance))
    //     {
    //        // Debug.Log("Nearest object is the grass ground.");
    //     }
    //
    //     // Display the distance in Unity
    //     DisplayDistance(shortestDistance);
    //
    //     // Draw a line between the placed object and the closest point (wall, floor, or ground)
    //     DrawLine(lineRenderer, objectPosition, closestPoint);
    //
    //     // Add the object to the list of placed objects
    //     _placedObjects.Add(placedObject);
    // }

    public void CalculateDistances(GameObject placedObject)
    {
        if (placedObject == null)
        {
            Debug.LogError("Placed object is null.");
            return;
        }
        
        var objectPosition = placedObject.transform.position;
        var closestPoint = Vector3.zero;
        var shortestDistance = float.MaxValue;

        foreach (var line in lines)
        {
            DrawLine(line.lineRenderer, placedObject.transform.position, line.relatedObject.transform.position);
            DisplayDistanceOnLine(line, placedObject.transform, line.relatedObject.transform);
        }
        
    }
    
    private bool CheckDistanceToLayer(Vector3 objectPosition, LayerMask layerMask, out Vector3 closestPoint, out float distance)
    {
        var colliders = Physics.OverlapSphere(objectPosition, 100f, layerMask); // Adjust the radius as needed
        closestPoint = Vector3.zero;
        distance = float.MaxValue;

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                var point = collider.ClosestPoint(objectPosition);
                var currentDistance = Vector3.Distance(objectPosition, point);
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
            distanceText.text = "Distance: " + distance.ToString("F2") + " feet";
        }
        //Debug.Log("Distance: " + distance.ToString("F2") + " units");
    }

    private void DrawLine(LineRenderer lineRenderer, Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.positionCount = 2;
        startPosition.y = 1.0f;
        endPosition.y = 1.0f;
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private void DisplayDistanceOnLine(LineHandler line, Transform startObject, Transform endObject)
    {
        var distance = Vector3.Distance(startObject.position, endObject.position);
        
        // var disInt = (int)(distance * 100);
        //
        // distance = distance * 0.01f;
        
        line.distanceText.text = distance.ToString("F2") + " feet";
    }

    // Method to recalculate the distance and update the LineRenderer when selecting an object
    public void RecalculateDistanceForSelectedObject(GameObject selectedObject)
    {
        var objectPosition = selectedObject.transform.position;
        var closestPoint = Vector3.zero;
        var shortestDistance = float.MaxValue;

        foreach (var model in ManagerHandler.Instance.spawningManager.modelsSpawned)
        {
            if (selectedObject.transform.parent.gameObject == model) continue;

            var line = Instantiate(distanceLinePrefab, Vector3.zero, Quaternion.identity);
            line.relatedObject = model;
            lines.Add(line);
        }
        
        
        
        
        // var lineRenderer = selectedObject.GetComponent<LineRenderer>();
        //
        // if (lineRenderer == null)
        // {
        //     lineRenderer = selectedObject.AddComponent<LineRenderer>();
        // }
        
        
        
        // // Check distance from wall first
        // if (CheckDistanceToLayer(objectPosition, wallLayer, out closestPoint, out shortestDistance))
        // {
        //     //Debug.Log("Nearest object is the wall.");
        // }
        // else if (CheckDistanceToLayer(objectPosition, floorLayer, out closestPoint, out shortestDistance))
        // {
        //    // Debug.Log("Nearest object is the floor.");
        // }
        // else if (CheckDistanceToLayer(objectPosition, grassGroundLayer, out closestPoint, out shortestDistance))
        // {
        //    // Debug.Log("Nearest object is the grass ground.");
        // }

        
        
        
        // Display the distance in Unity
        // DisplayDistance(shortestDistance);

        // Draw a line between the placed object and the closest point (wall, floor, or ground)
        // DrawLine(lineRenderer, objectPosition, closestPoint);
    }
}
