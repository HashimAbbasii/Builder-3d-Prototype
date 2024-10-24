using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpawningManagerTutorial : MonoBehaviour
{

    public List<GameObject> modelPrefabs;
    public List<GameObject> furniturePrefabs;
    public List<GameObject> evidencePrefabs;
    
    public Material[] floorMaterials; // Array to hold different materials for the floor
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public Material previewMaterial;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;
    public ArrowControl arrowControl;


    private Vector3 _initialMousePos;
    private Vector3 _finalMousePos;
    private GameObject _currentFloor;
    private GameObject _currentWall;
    private GameObject _previewObject;
    private Material _originalMaterial; // To store the original material of the model
    private bool _isCreatingFloor = false;
    private bool _isCreatingWall = false;
    private int _selectedObjectIndex = -1;

    public GameObject floorMaterialChanged;
    public LayerMask surfaceLayerMask;

    public List<GameObject> floorsSpawned = new();
    public List<GameObject> wallsSpawned = new();
    public List<GameObject> modelsSpawned = new();

   // public ArrowControl arrowControl;

    private void OnValidate()
    {
        for (var i = 0; i < modelPrefabs.Count(); i++)
        {
            var model = modelPrefabs[i];

            model.GetComponent<SelectableObject>().objectID = i + 2;
        }
    }

    private void Start()
    {
        StartCoroutine(ArrowHandle());
        for (var i = 0; i < modelPrefabs.Count(); i++)
        {
            var model = modelPrefabs[i];

            model.GetComponent<SelectableObject>().objectID = i + 2;
            
            switch (model.GetComponent<SelectableObject>().modelType)
            {
                case ModelType.Furniture:
                    furniturePrefabs.Add(model);
                    break;
                case ModelType.Evidence:
                    evidencePrefabs.Add(model);
                    break;
            }
        }

        var fch = ManagerHandler.Instance.uiManager.canvasHandler.furnitureButtonsContentHolder;
        var ech = ManagerHandler.Instance.uiManager.canvasHandler.evidenceButtonsContentHolder;
        var buttonPrefab = ManagerHandler.Instance.uiManager.modelButtonPrefab;

        for (var i = 0; i < furniturePrefabs.Count; i++)
        {
            var fp = furniturePrefabs[i];
            var fb = Instantiate(buttonPrefab, fch.transform);
            fb.modelText.text = fp.name;

            fb.button.onClick.AddListener(() =>
                SelectObject(modelPrefabs.FirstOrDefault(x => x == fp)!.GetComponent<SelectableObject>().objectID - 2));
        }

        for (var i = 0; i < evidencePrefabs.Count; i++)
        {
            var ep = evidencePrefabs[i];
            var eb = Instantiate(buttonPrefab, ech.transform);
            eb.modelText.text = ep.name;
            eb.button.onClick.AddListener(() =>
                SelectObject(modelPrefabs.FirstOrDefault(x => x == ep)!.GetComponent<SelectableObject>().objectID - 2));
        }
    }

    private void Update()
    {
        // Floor creation logic
        if (_isCreatingFloor && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _initialMousePos = hit.point;
                _initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                _currentFloor = Instantiate(floorPrefab, _initialMousePos, Quaternion.identity);
                floorsSpawned.Add(_currentFloor);
                floorMaterialChanged = _currentFloor;
            }
        }
        

        if (_isCreatingFloor && Input.GetMouseButton(0) && _currentFloor != null) // Dragging the mouse
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                _finalMousePos = hit.point;
                _finalMousePos.y = 0f; // Ensure the y-axis remains at 0

                Vector3 scale = _finalMousePos - _initialMousePos;
                _currentFloor.transform.localScale = new Vector3(scale.x, 0.1f, scale.z); // Assuming floor thickness is 0.1 unit
            }
        }

        if (_isCreatingFloor && Input.GetMouseButtonUp(0) && _currentFloor != null) // Mouse button released
        {
            _isCreatingFloor = false; // Reset the floor creation process
            _currentFloor = null; // Reset the current floor object

           // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
            Debug.Log("Floor Up");
        }

        // Wall creation logic
        if (_isCreatingWall && Input.GetMouseButtonDown(0)) // Left mouse button pressed
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _initialMousePos = hit.point;
                _initialMousePos.y = 0f; // Ensure the y-axis is set to 0
                _currentWall = Instantiate(wallPrefab, _initialMousePos, Quaternion.identity);
                wallsSpawned.Add(_currentWall);
            }
        }

        if (_isCreatingWall && Input.GetMouseButton(0) && _currentWall != null) // Dragging the mouse
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _finalMousePos = hit.point;
                _finalMousePos.y = 0f; // Ensure the y-axis remains at 0

                var direction = _finalMousePos - _initialMousePos;

                // Determine whether the drag is more along the x-axis or the z-axis
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    // Dragging along the x-axis
                    var distanceX = _finalMousePos.x - _initialMousePos.x;
                    _currentWall.transform.localScale = new Vector3(distanceX, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 0, 0); // Set rotation to 0 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the x-axis
                    _currentWall.transform.position = new Vector3(_initialMousePos.x + distanceX / 2, 0f, _initialMousePos.z);
                }
                else
                {
                    // Dragging along the z-axis
                    var distanceZ = _finalMousePos.z - _initialMousePos.z;
                    _currentWall.transform.localScale = new Vector3(distanceZ, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 90, 0); // Set rotation to 90 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the z-axis
                    _currentWall.transform.position = new Vector3(_initialMousePos.x, 0f, _initialMousePos.z + distanceZ / 2);
                }
            }
        }

        if (_isCreatingWall && Input.GetMouseButtonUp(0) && _currentWall != null) // Mouse button released
        {
            _isCreatingWall = false; // Reset the wall creation process
            _currentWall = null; // Reset the current wall object
            arrowControl.arrowForWallIndicate.gameObject.SetActive(false);
            arrowControl.MovetotheModelInterior();

           // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
            Debug.Log("Wall Up");
        }

        // Preview object follows the mouse
        if (_previewObject != null)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, surfaceLayerMask, QueryTriggerInteraction.Collide))
            {
                var previewPosition = hit.point;
                previewPosition.y = 0f; // Ensure the y-axis remains at 0
                _previewObject.transform.position = previewPosition;

                // Check if the object is being placed on a valid surface
                var surface = hit.collider.GetComponent<ObjectType>();
                Debug.Log("Place");
                ApplyMaterial(_previewObject, surface != null ? validPlacementMaterial : invalidPlacementMaterial); // Invalid surface
                // Valid surface
            }
        }

        // Object placement logic
        if (Input.GetMouseButtonUp(0) && _selectedObjectIndex != -1 && _previewObject != null) // Place object on mouse button release
        {
            PlaceObjectOnSurface();
            Debug.Log("Model prefab");
            //arrowControl.EvidenceButtonMoves();
        }
    }
    public bool ArrowIndicate = false;
    public GameObject Arrow;
    // Called when the floor button is clicked
    public void OnFloorButtonClick()
    {
      //  if (arrowControl.FloorButton == true)
        {
            //Arrow.gameObject.SetActive(false);

            _isCreatingFloor = true;
            _isCreatingWall = false; // Ensure wall creation is not active
            if (_previewObject != null)
            {
                Destroy(_previewObject);
            }
            _previewObject = null;
            _selectedObjectIndex = -1;
        }
        
    }



    IEnumerator ArrowHandle()
    {
        ArrowIndicate = true;
        yield return new WaitForSecondsRealtime(2f);
        ArrowIndicate = false;

    }


    // Called when the wall button is clicked
    public void OnWallButtonClick()
    {
      //  ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        _isCreatingWall = true;
        _isCreatingFloor = false; // Ensure floor creation is not active
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        _previewObject = null;
        _selectedObjectIndex = -1;
    }

    // Called when an object button (like Chair, Table) is clicked
    public void SelectObject(int objectIndex)
    {
        _isCreatingWall = false;
        _isCreatingFloor = false;

       // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        Debug.Log("SelectObject");
        // Ensure floor creation is not active
        if (objectIndex >= 0 && objectIndex < modelPrefabs.Count)
        {
            _selectedObjectIndex = objectIndex;

            // Instantiate the preview object
            if (_previewObject != null)
            {
                Destroy(_previewObject);
            }
            _previewObject = Instantiate(modelPrefabs[objectIndex]);
            arrowControl.EvidenceButtonMoves();
            _originalMaterial = _previewObject.GetComponentInChildren<SelectableObject>().objectRenderer.material; // Store the original material
            ApplyMaterial(_previewObject, previewMaterial);
        }
        else
        {
            _selectedObjectIndex = -1; // Deselect if index is out of range

            // Destroy the preview object if the selection is cleared
            if (_previewObject != null)
            {
                Destroy(_previewObject);
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
            var prefab = modelPrefabs[_selectedObjectIndex];

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
                       // arrowControl.EvidenceButtonMoves();
                        Debug.Log("EvidenceButtonMoves");

                        // Reset the material of the placed object to the original material
                        var renderers = placedObject.GetComponentsInChildren<Renderer>();
                        foreach (var renderer in renderers)
                        {
                            renderer.material = _originalMaterial;
                        }
                        
                        // Destroy the preview object after placing the actual object
                        Destroy(_previewObject);
                        _previewObject = null; // Reset previewObject to avoid repeated placements
                        _selectedObjectIndex = -1; // Reset selection after placement
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
                            renderer.material = _originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(_previewObject);
                        _previewObject = null; // Reset previewObject to avoid repeated placements
                        _selectedObjectIndex = -1; // Reset selection after placement
                    }

                    break;

                case SurfaceType.Wall:
                    var wall = (WallObject)surface;

                    var placePositionWall = hit.point;

                    // Instantiate and place the selected object at the hit position
                    var prefabWall = modelPrefabs[_selectedObjectIndex];
                    if (prefabWall != null)
                    {
                        var placedWallObject = Instantiate(prefabWall, placePositionWall, Quaternion.identity);

                        // Reset the material of the placed object to the original material
                        var wallRenderers = placedWallObject.GetComponentsInChildren<Renderer>();
                        foreach (var wallRenderer in wallRenderers)
                        {
                            wallRenderer.material = _originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(_previewObject);
                        _previewObject = null; // Reset previewObject to avoid repeated placements
                        _selectedObjectIndex = -1; // Reset selection after placement
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
                            renderer.material = _originalMaterial;
                        }

                        // Destroy the preview object after placing the actual object
                        Destroy(_previewObject);
                        _previewObject = null; // Reset previewObject to avoid repeated placements
                        _selectedObjectIndex = -1; // Reset selection after placement
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