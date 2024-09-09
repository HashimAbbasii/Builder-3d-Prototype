using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFloor : MonoBehaviour
{
    public GameObject[] modelPrefabs;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public Material previewMaterial;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    public Material[] floorMaterials; // Array to hold different materials for the floor

    private Vector3 initialMousePos;
    private Vector3 finalMousePos;
    private GameObject currentFloor;
    private GameObject currentWall;
    private GameObject previewObject;
    private Material originalMaterial; // To store the original material of the model
    private bool isCreatingFloor = false;
    private bool isCreatingWall = false;
    private int selectedObjectIndex = -1;
    public LayerMask layerMask;

    void Update()
    {
        // Floor creation logic
        if (isCreatingFloor && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                initialMousePos = hit.point;
                initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                currentFloor = Instantiate(floorPrefab, initialMousePos, Quaternion.identity);
            }
        }

        if (isCreatingFloor && Input.GetMouseButton(0) && currentFloor != null) // Dragging the mouse
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                finalMousePos = hit.point;
                finalMousePos.y = 0f; // Ensure the y-axis remains at 0

                Vector3 scale = finalMousePos - initialMousePos;
                currentFloor.transform.localScale = new Vector3(scale.x, 0.1f, scale.z); // Assuming floor thickness is 0.1 unit
            }
        }

        if (isCreatingFloor && Input.GetMouseButtonUp(0) && currentFloor != null) // Mouse button released
        {
            isCreatingFloor = false; // Reset the floor creation process
            currentFloor = null; // Reset the current floor object
        }

        // Wall creation logic
        if (isCreatingWall && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                initialMousePos = hit.point;
                initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                currentWall = Instantiate(wallPrefab, initialMousePos, Quaternion.identity);
            }
        }

        if (isCreatingWall && Input.GetMouseButton(0) && currentWall != null) // Dragging the mouse
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                finalMousePos = hit.point;
                finalMousePos.y = 0f; // Ensure the y-axis remains at 0

                Vector3 direction = finalMousePos - initialMousePos;

                // Determine whether the drag is more along the x-axis or the z-axis
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    // Dragging along the x-axis
                    float distanceX = finalMousePos.x - initialMousePos.x;
                    currentWall.transform.localScale = new Vector3(distanceX, currentWall.transform.localScale.y, currentWall.transform.localScale.z);
                    currentWall.transform.rotation = Quaternion.Euler(0, 0, 0); // Set rotation to 0 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the x-axis
                    currentWall.transform.position = new Vector3(initialMousePos.x + distanceX / 2, 0f, initialMousePos.z);
                }
                else
                {
                    // Dragging along the z-axis
                    float distanceZ = finalMousePos.z - initialMousePos.z;
                    currentWall.transform.localScale = new Vector3(distanceZ, currentWall.transform.localScale.y, currentWall.transform.localScale.z);
                    currentWall.transform.rotation = Quaternion.Euler(0, 90, 0); // Set rotation to 90 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the z-axis
                    currentWall.transform.position = new Vector3(initialMousePos.x, 0f, initialMousePos.z + distanceZ / 2);
                }
            }
        }

        if (isCreatingWall && Input.GetMouseButtonUp(0) && currentWall != null) // Mouse button released
        {
            isCreatingWall = false; // Reset the wall creation process
            currentWall = null; // Reset the current wall object
        }

        // Preview object follows the mouse
        if (previewObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask, QueryTriggerInteraction.Collide))
            {
                Vector3 previewPosition = hit.point;
                previewPosition.y = 0f; // Ensure the y-axis remains at 0
                previewObject.transform.position = previewPosition;

                // Check if the object is being placed on a valid surface (i.e., the floor)
                if (hit.collider.CompareTag("Floor"))
                {
                    ApplyMaterial(previewObject, validPlacementMaterial);
                }
                else
                {
                    ApplyMaterial(previewObject, invalidPlacementMaterial);
                }
            }
        }

        // Object placement logic
        if (Input.GetMouseButtonUp(0) && selectedObjectIndex != -1 && previewObject != null) // Place object on mouse button release
        {
            PlaceObjectOnFloor();
        }
    }

    // Called when the floor button is clicked
    public void OnFloorButtonClick()
    {
        isCreatingFloor = true;
        isCreatingWall = false; // Ensure wall creation is not active
    }

    // Called when the wall button is clicked
    public void OnWallButtonClick()
    {
        isCreatingWall = true;
        isCreatingFloor = false; // Ensure floor creation is not active
    }

    // Called when an object button (like Chair, Table) is clicked
    public void SelectObject(int objectIndex)
    {
        if (objectIndex >= 0 && objectIndex < modelPrefabs.Length)
        {
            selectedObjectIndex = objectIndex;

            // Instantiate the preview object
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
            previewObject = Instantiate(modelPrefabs[objectIndex]);
            originalMaterial = previewObject.GetComponent<Renderer>().material; // Store the original material
            ApplyMaterial(previewObject, previewMaterial);
        }
        else
        {
            selectedObjectIndex = -1; // Deselect if index is out of range

            // Destroy the preview object if the selection is cleared
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
        }
    }

    // Apply the given material to the preview object
    private void ApplyMaterial(GameObject obj, Material material)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    // Place the selected object on the floor
    private void PlaceObjectOnFloor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the ray hit an object with the "Floor" tag
            if (hit.collider.CompareTag("Floor"))
            {
                Vector3 placePosition = hit.point;
                placePosition.y = 0f; // Ensure the y-axis remains at 0

                GameObject prefab = modelPrefabs[selectedObjectIndex];
                if (prefab != null)
                {
                    GameObject placedObject = Instantiate(prefab, placePosition, Quaternion.identity);

                    // Reset the material of the placed object to the original material
                    Renderer[] renderers = placedObject.GetComponentsInChildren<Renderer>();
                    foreach (Renderer renderer in renderers)
                    {
                        renderer.material = originalMaterial;
                    }

                    // Destroy the preview object after placing the actual object
                    Destroy(previewObject);
                    previewObject = null; // Reset previewObject to avoid repeated placements
                    selectedObjectIndex = -1; // Reset selection after placement
                }
            }
            else
            {
                Debug.Log("Cannot place object, target is not a floor.");
            }
        }
    }

    // Method to change the floor's texture/material
    public void ChangeFloorTexture(int materialIndex)
    {
        if (currentFloor != null && materialIndex >= 0 && materialIndex < floorMaterials.Length)
        {
            Renderer floorRenderer = currentFloor.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorRenderer.material = floorMaterials[materialIndex];
            }
        }
    }
}
