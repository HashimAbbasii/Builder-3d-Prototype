using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceContain : MonoBehaviour
{
    public Transform cameraReference; // Reference to the camera transform
    public Transform parentTransform; // Reference to the parent transform of the camera
    public List <Transform> spawnModel = new List<Transform>(); // List of child transforms>

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
    }

    // Update is called once per frame
    void Update()
    {
        if (parentTransform != null)
        {
            // Continuously monitor and log the parent's position and rotation
            Vector3 parentPosition = parentTransform.position;
            Quaternion parentRotation = parentTransform.rotation;

            //Debug.Log($"Parent Position: {parentPosition}");
            //Debug.Log($"Parent Rotation: {parentRotation}");
        }
        if(spawnModel.Count > 0 && spawnModel != null)
        {

        }

    }
}
