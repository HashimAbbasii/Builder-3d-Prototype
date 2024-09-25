using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpawningManager : MonoBehaviour
{

    public GameObject[] modelPrefabs;
    public Material[] floorMaterials; // Array to hold different materials for the floor
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public Material previewMaterial;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    public Material originalMatrial;
  
    

    private Vector3 initialMousePos;
    private Vector3 finalMousePos;
    public GameObject currentFloor;
    private GameObject currentWall;
    private GameObject previewObject;
    private Material originalMaterial; // To store the original material of the model
    private bool isCreatingFloor = false;
    private bool isCreatingWall = false;
    private int selectedObjectIndex = -1;

    public GameObject FloorMaterialChanged;
    public LayerMask surfaceLayerMask;

    public List<GameObject> floorsSpawned = new();
    public List<GameObject> wallsSpawned = new();
    public List<GameObject> modelsSpawned = new();


 

 



    private void Update()
    {
        // Floor creation logic
        if (isCreatingFloor && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                initialMousePos = hit.point;
                initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                currentFloor = Instantiate(floorPrefab, initialMousePos, Quaternion.identity);
                floorsSpawned.Add(currentFloor);
                FloorMaterialChanged = currentFloor;
            }
        }
        

        if (isCreatingFloor && Input.GetMouseButton(0) && currentFloor != null) // Dragging the mouse
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
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

           // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
            Debug.Log("Floor Up");
        }

        // Wall creation logic
        if (isCreatingWall && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                initialMousePos = hit.point;
                initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                currentWall = Instantiate(wallPrefab, initialMousePos, Quaternion.identity);
                wallsSpawned.Add(currentWall);
            }
        }

        if (isCreatingWall && Input.GetMouseButton(0) && currentWall != null) // Dragging the mouse
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                finalMousePos = hit.point;
                finalMousePos.y = 0f; // Ensure the y-axis remains at 0

                var direction = finalMousePos - initialMousePos;

                // Determine whether the drag is more along the x-axis or the z-axis
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    // Dragging along the x-axis
                    var distanceX = finalMousePos.x - initialMousePos.x;
                    currentWall.transform.localScale = new Vector3(distanceX, currentWall.transform.localScale.y, currentWall.transform.localScale.z);
                    currentWall.transform.rotation = Quaternion.Euler(0, 0, 0); // Set rotation to 0 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the x-axis
                    currentWall.transform.position = new Vector3(initialMousePos.x + distanceX / 2, 0f, initialMousePos.z);
                }
                else
                {
                    // Dragging along the z-axis
                    var distanceZ = finalMousePos.z - initialMousePos.z;
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

           // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
            Debug.Log("Wall Up");
        }

        // Preview object follows the mouse
        if (previewObject != null)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, surfaceLayerMask, QueryTriggerInteraction.Collide))
            {
                var previewPosition = hit.point;
                previewPosition.y = 0f; // Ensure the y-axis remains at 0
                previewObject.transform.position = previewPosition;

                // Check if the object is being placed on a valid surface
                var surface = hit.collider.GetComponent<ObjectType>();
                ApplyMaterial(previewObject, surface != null ? validPlacementMaterial : invalidPlacementMaterial); // Invalid surface
                // Valid surface
            }
        }

        // Object placement logic
        if (Input.GetMouseButtonUp(0) && selectedObjectIndex != -1 && previewObject != null) // Place object on mouse button release
        {
            PlaceObjectOnSurface();
        }
    }

    // Called when the floor button is clicked
    public void OnFloorButtonClick()
    {
        ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        isCreatingFloor = true;
        isCreatingWall = false; // Ensure wall creation is not active
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = null;
        selectedObjectIndex = -1;
    }


    // Called when the wall button is clicked
    public void OnWallButtonClick()
    {
        ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        isCreatingWall = true;
        isCreatingFloor = false; // Ensure floor creation is not active
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
        previewObject = null;
        selectedObjectIndex = -1;
    }

    // Called when an object button (like Chair, Table) is clicked
    public void SelectObject(int objectIndex)
    {
        isCreatingWall = false;
        isCreatingFloor = false;

        ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        Debug.Log("SelectObject");
        // Ensure floor creation is not active
        if (objectIndex >= 0 && objectIndex < modelPrefabs.Length)
        {
            selectedObjectIndex = objectIndex;

            // Instantiate the preview object
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
            previewObject = Instantiate(modelPrefabs[objectIndex]);
            originalMaterial = previewObject.GetComponentInChildren<SelectableObject>().objectRenderer.material; // Store the original material
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
        var renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }
    }

    // Place the selected object on a valid surface
    private void PlaceObjectOnSurface()
    {

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.SphereCast(ray, 0.1f, out var hit)) return;

        // Check if the ray hit a valid surface with the Surface component
        var surface = hit.collider.GetComponent<ObjectType>();


        if (surface != null)
        {
            var prefab = modelPrefabs[selectedObjectIndex];

            switch (surface.surfaceType)
            {
                case SurfaceType.Grass:
                    var grass = (GrassObject)surface;

                    var placePosition1 = hit.point;

                    if (prefab != null)
                    {
                        // Instantiate the object (e.g., knife) at the calculated position
                        var placedObject = Instantiate(prefab, placePosition1, Quaternion.identity);

                        //ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");

                        modelsSpawned.Add(placedObject);

                        // Reset the material of the placed object to the original material
                        var renderers = placedObject.GetComponentsInChildren<Renderer>();
                        foreach (var renderer in renderers)
                        {
                            renderer.material = originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(previewObject);
                        previewObject = null; // Reset previewObject to avoid repeated placements
                        selectedObjectIndex = -1; // Reset selection after placement
                    }

                    break;

                case SurfaceType.Floor:
                    var floor = (FloorObject)surface;
                    var placePosition2 = hit.point;

                    if (prefab != null)
                    {
                        // Instantiate the object (e.g., knife) at the calculated position
                        var placedObject = Instantiate(prefab, placePosition2, Quaternion.identity);

                        //ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
                        modelsSpawned.Add(placedObject);

                        // Reset the material of the placed object to the original material
                        var renderers = placedObject.GetComponentsInChildren<Renderer>();
                        foreach (var renderer in renderers)
                        {
                            renderer.material = originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(previewObject);
                        previewObject = null; // Reset previewObject to avoid repeated placements
                        selectedObjectIndex = -1; // Reset selection after placement
                    }

                    break;

                case SurfaceType.Wall:
                    var wall = (WallObject)surface;

                    var placePositionWall = hit.point;

                    // Instantiate and place the selected object at the hit position
                    var prefabWall = modelPrefabs[selectedObjectIndex];
                    if (prefabWall != null)
                    {
                        var placedWallObject = Instantiate(prefabWall, placePositionWall, Quaternion.identity);

                        // Reset the material of the placed object to the original material
                        var wallRenderers = placedWallObject.GetComponentsInChildren<Renderer>();
                        foreach (var wallRenderer in wallRenderers)
                        {
                            wallRenderer.material = originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(previewObject);
                        previewObject = null; // Reset previewObject to avoid repeated placements
                        selectedObjectIndex = -1; // Reset selection after placement
                    }




                    break;

                case SurfaceType.Models:
                    var model = (SelectableObject)surface;

                    // if (!model.canPlaceObjectsOnIt) return;

                    var placePosition3 = hit.point;

                    // Adjust the y-position based on the surface's height offset
                    placePosition3.y += model.heightOffset;

                    Debug.Log("placePosition" + placePosition3.y);

                    // Instantiate the object (e.g., knife) at the calculated position
                    if (prefab != null)
                    {
                        var placedObject = Instantiate(prefab, placePosition3, Quaternion.identity);

                        //ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
                        modelsSpawned.Add(placedObject);

                        // Reset the material of the placed object to the original material
                        var renderers = placedObject.GetComponentsInChildren<Renderer>();
                        foreach (var renderer in renderers)
                        {
                            renderer.material = originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(previewObject);
                        previewObject = null; // Reset previewObject to avoid repeated placements
                        selectedObjectIndex = -1; // Reset selection after placement
                    }

                    break;
            }
        }
        else
        {
            Debug.Log("Cannot place object, no valid surface detected.");
        }
    }

    // Method to change the floor's texture/material
    public void ChangeFloorTexture(int materialIndex)
    {
        if (materialIndex >= 0 && materialIndex < floorMaterials.Length)
        {
            // Find all objects in the scene that have the same prefab or a specific tag (e.g., "Floor")
            GameObject[] allFloors = GameObject.FindGameObjectsWithTag("Floor");

            foreach (var floor in allFloors)
            {
                // Check if the floor object has children
                if (floor.transform.childCount > 0)
                {
                    // Get the first child of the floor object
                    var firstChild = floor.transform.GetChild(0);

                    // Access the Renderer component of the first child
                    var floorRenderer = firstChild.GetComponent<Renderer>();

                    if (floorRenderer != null)
                    {
                        // Apply the selected material
                        floorRenderer.material = floorMaterials[materialIndex];
                    }
                    else
                    {
                        Debug.LogWarning("Renderer component not found on the first child of the floor object.");
                    }
                }
                else
                {
                    Debug.LogWarning("No children found on the floor object.");
                }
            }
        }
        else
        {
            Debug.LogWarning("Invalid materialIndex or floor materials not set.");
        }
    }
}