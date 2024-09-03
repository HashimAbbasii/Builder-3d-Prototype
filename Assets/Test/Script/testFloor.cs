using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testFloor : MonoBehaviour
{
    public GameObject[] modelPrefabs; // Array to hold your object prefabs
    public GameObject floorPrefab;    // Prefab for the floor
    public Material previewMaterial;  // Material for the preview object (semi-transparent)
    public Material validPlacementMaterial;  // Material when placement is valid (e.g., yellow)
    public Material invalidPlacementMaterial; // Material when placement is invalid (e.g., red)

    private Vector3 initialMousePos;
    private Vector3 finalMousePos;
    private GameObject currentFloor;
    private GameObject previewObject; // The object that follows the mouse cursor
    private bool isCreatingFloor = false;
    private int selectedObjectIndex = -1; // Index for the selected object (chair, table, etc.)
    public LayerMask layerMask;

    void Update()
    {
        // Floor creation logic
        if (isCreatingFloor && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                initialMousePos = hit.point;
                currentFloor = Instantiate(floorPrefab, initialMousePos, Quaternion.identity);
            }
        }

        if (isCreatingFloor && Input.GetMouseButton(0) && currentFloor != null) // Dragging the mouse
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                finalMousePos = hit.point;

                Vector3 scale = finalMousePos - initialMousePos;
                currentFloor.transform.localScale = new Vector3(scale.x, 0.1f, scale.z); // Assuming floor thickness is 1 unit
            }
        }

        if (isCreatingFloor && Input.GetMouseButtonUp(0) && currentFloor != null) // Mouse button released
        {
            isCreatingFloor = false; // Reset the floor creation process
            currentFloor = null; // Reset the current floor object
        }

        // Preview object follows the mouse
        if (previewObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Collide))
            {
                Vector3 previewPosition = hit.point;
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
        if (Input.GetMouseButtonDown(0) && selectedObjectIndex != -1 && previewObject != null) // Place object on left click
        {
            PlaceObjectOnFloor();
        }
    }

    // Called when the floor button is clicked
    public void OnFloorButtonClick()
    {
        isCreatingFloor = true;
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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Check if the ray hit an object with the "Floor" tag
            if (hit.collider.CompareTag("Floor"))
            {
                Debug.Log("Placed Object on Floor");
                Vector3 placePosition = hit.point;

                // Ensure the y-axis remains at 0
                placePosition.y = 0f;

                GameObject prefab = modelPrefabs[selectedObjectIndex];

                if (prefab != null)
                {
                    Instantiate(prefab, placePosition, Quaternion.identity);

                    // Destroy the preview object after placing the actual object
                    Destroy(previewObject);
                    previewObject = null; // Reset previewObject to avoid repeated placements
                }
            }
            else
            {
                Debug.Log("Cannot place object, target is not a floor.");
            }
        }
    }
}
