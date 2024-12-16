using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceContain : MonoBehaviour
{
    public Transform cameraReference; // Reference to the camera transform
    public Transform parentTransform; // Reference to the parent transform of the camera
    public List<Transform> spawnModel = new List<Transform>(); // List of child transforms

    private Dictionary<Transform, Vector3> modelPositions = new Dictionary<Transform, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        if (cameraReference != null)
        {
            // Get the parent of the camera reference
            parentTransform = cameraReference.parent;

            if (parentTransform != null)
            {
                Debug.Log($"Camera's parent found: {parentTransform.name}");
            }
            else
            {
                Debug.LogWarning("The camera has no parent.");
            }
        }
        else
        {
            Debug.LogError("Camera reference is not assigned in the Inspector.");
        }

        // Initialize the position dictionary
        foreach (var model in spawnModel)
        {
            if (model != null)
            {
                modelPositions[model] = model.position;
            }
        }

        // Start monitoring positions in a coroutine
        StartCoroutine(MonitorModelPositions());
    }

    private IEnumerator MonitorModelPositions()
    {
        while (true)
        {
            foreach (var model in spawnModel)
            {
                if (model != null)
                {
                    Vector3 currentPosition = model.position;

                    // Check if the position has changed
                    if (modelPositions.ContainsKey(model) && modelPositions[model] != currentPosition)
                    {
                        Debug.Log($"Model {model.name} moved to {currentPosition}");
                        modelPositions[model] = currentPosition; // Update the last known position
                    }
                }
            }

            // Yield to spread processing over multiple frames
            yield return null;
        }
    }
}
