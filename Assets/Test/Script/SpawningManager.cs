using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class SpawningManager : MonoBehaviour
{
    public Text measurementText;
    public List<GameObject> modelPrefabs;
    public List<GameObject> furniturePrefabs;
    public List<GameObject> evidencePrefabs;

    public Shader lineShader;
    public Material[] floorMaterials; // Array to hold different materials for the floor
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public Material previewMaterial;
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    private Vector3 _initialMousePos;
    private Vector3 _finalMousePos;
    public GameObject _currentFloor;
    private GameObject _currentWall;
    private GameObject _previewObject;
    private Material _originalMaterial; // To store the original material of the model
    private bool _isCreatingFloor = false;
    private bool _isCreatingWall = false;
    public bool _wallAlongZAxis = false;
    private int _selectedObjectIndex = -1;

    public GameObject floorMaterialChanged;
    public LayerMask surfaceLayerMask;
    public LayerMask floorPlacementMask;

    public List<GameObject> floorsSpawned = new();
    public List<GameObject> wallsSpawned = new();
    public List<GameObject> modelsSpawned = new();
    public TextMeshProUGUI _floorDimensionsText; // Reference to your UI Text element

    private LineRenderer xLineRenderer;
    private LineRenderer zLineRenderer;
    private LineRenderer wallLineRenderer;


    public TextMeshProUGUI xDimensionText; // Text for X-axis dimensions
    public TextMeshProUGUI zDimensionText; // Text for Z-axis dimensions
    public TextMeshProUGUI wallDimensionText;

    public Canvas uiCanvas; // Reference to the Canvas
    public Canvas uiCanvasZaxis;
    public Canvas uiCanvasWall;



    private void SetupLineRenderers()
    {


        //if (uiCanvas == null)
        //{
        //    // Find or create a Canvas in the scene
        //    uiCanvas = FindObjectOfType<Canvas>();
        //    if (uiCanvas == null)
        //    {
        //        uiCanvas = new GameObject("UI Canvas").AddComponent<Canvas>();
        //        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //        uiCanvas.gameObject.AddComponent<CanvasScaler>();
        //        uiCanvas.gameObject.AddComponent<GraphicRaycaster>();
        //    }
        //}

        // Create and configure the material for LineRenderers
        Material transparentMaterial = new Material(lineShader);
        transparentMaterial.color = new Color(1, 0, 0, 0.2f); // Red color with 50% transparency for XLine
                                                              // X-axis line (representing X-scale)
        xLineRenderer = new GameObject("XLine").AddComponent<LineRenderer>();
        xLineRenderer.startWidth = 0.025f;
        xLineRenderer.endWidth = 0.025f;
        xLineRenderer.material = transparentMaterial;

        // Z-axis line (representing Z-scale)
        zLineRenderer = new GameObject("ZLine").AddComponent<LineRenderer>();
        zLineRenderer.startWidth = 0.025f;
        zLineRenderer.endWidth = 0.025f;
        zLineRenderer.material = new Material(transparentMaterial) { color = new Color(0, 0, 1, 0.2f) }; // Blue color with 50% transparency


        wallLineRenderer = new GameObject("WallLine").AddComponent<LineRenderer>();
        wallLineRenderer.startWidth = 0.025f;
        wallLineRenderer.endWidth = 0.025f;
        wallLineRenderer.material = new Material(transparentMaterial) { color = new Color(0, 0, 1, 0.2f) }; // Blue color with 50% transparency

        xLineRenderer.positionCount = 2;
        zLineRenderer.positionCount = 2;
        wallLineRenderer.positionCount = 0;

        // Initially disable the lines
        xLineRenderer.enabled = false;
        zLineRenderer.enabled = false;
        wallLineRenderer.enabled = false;




}


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
        SetupLineRenderers();  // Initialize the LineRenderers

        xDimensionText = new GameObject("XDimensionText").AddComponent<TextMeshProUGUI>();
        zDimensionText = new GameObject("ZDimensionText").AddComponent<TextMeshProUGUI>();
        wallDimensionText = new GameObject("WallDimensionText").AddComponent<TextMeshProUGUI>();

        //......Set the Value By Default.........................................//


        xDimensionText.transform.SetParent(uiCanvas.transform, false);
        uiCanvas.transform.SetParent(xLineRenderer.transform, false);


        Vector2 currentPosition = xDimensionText.rectTransform.anchoredPosition;
        RectTransform rectTransform = xDimensionText.rectTransform;

        // Access width and height
        float width =  10f;
        float height = 10f;
        rectTransform.sizeDelta = new Vector2(width, height);


        currentPosition.x = 0; // Set x to 0
        currentPosition.y = 1.06f; // Set y to 0



        xDimensionText.rectTransform.anchoredPosition = currentPosition;


        RectTransform rectTransformX = xDimensionText.rectTransform;
        rectTransformX.rotation = Quaternion.Euler(90,0,0);

        //..........................;;;;............................//



        zDimensionText.transform.SetParent(uiCanvasZaxis.transform, false);
        uiCanvasZaxis.transform.SetParent(zLineRenderer.transform, false);

        //.....................Set Z Dimension...........................//

        Vector2 currentPositionZ = zDimensionText.rectTransform.anchoredPosition;
        RectTransform rectTransformZ = zDimensionText.rectTransform;


        float widthZ = 10f;
        float heightZ = 10f;
        rectTransformZ.sizeDelta = new Vector2(widthZ, heightZ);


        currentPositionZ.x = 5; // Set x to 0
        currentPositionZ.y = 1.06f; // Set y to 0

        zDimensionText.rectTransform.anchoredPosition = currentPositionZ;
        RectTransform rectTransformZe = zDimensionText.rectTransform;
        rectTransformZe.rotation = Quaternion.Euler(90, -90, 0);




        zDimensionText.transform.SetParent(uiCanvasZaxis.transform, false);

        //.........................,,,....................................//
        wallDimensionText.transform.SetParent(uiCanvasWall.transform, false);
        uiCanvasWall.transform.SetParent(wallLineRenderer.transform, false);


        Vector2 currentPositionwall = wallDimensionText.rectTransform.anchoredPosition;
        RectTransform rectTransformwall = wallDimensionText.rectTransform;

        // Access width and height
        float widthwall = 10f;
        float heightwall = 10f;
        wallDimensionText.rectTransform.sizeDelta = new Vector2(widthwall, heightwall);


        currentPositionwall.x = 0; // Set x to 0
        currentPositionwall.y = 1.06f; // Set y to 0



        wallDimensionText.rectTransform.anchoredPosition = currentPositionwall;


        RectTransform rectTransformWall = wallDimensionText.rectTransform;
        rectTransformWall.rotation = Quaternion.Euler(90, 0, 0);



        // Set text properties (position, font size, color, etc.)
        xDimensionText.fontSize = 1;
        xDimensionText.color = Color.red;
        zDimensionText.fontSize = 1;
        zDimensionText.color = Color.blue;
        wallDimensionText.fontSize = 1;
        wallDimensionText.color = Color.gray;

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
       // UpdateFloorDimensionsText(Vector3.zero, Vector3.zero); // Initialize with zero dimensions
    }

    private void Update()
    {
        // Floor creation logic
        if (_isCreatingFloor && Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorPlacementMask))
            {
                _initialMousePos = hit.point;
               
                _initialMousePos.y = 0f;
                _currentFloor = Instantiate(floorPrefab, _initialMousePos, Quaternion.identity);
                floorsSpawned.Add(_currentFloor);
                floorMaterialChanged = _currentFloor;
                _floorDimensionsText = _currentFloor.GetComponentInChildren<TextMeshProUGUI>();

                // Initialize text when floor creation starts
                UpdateFloorDimensionsText(_initialMousePos, _initialMousePos);
                xLineRenderer.enabled = true;
                zLineRenderer.enabled = true;
                xLineRenderer.startWidth = 0.3f;
                xLineRenderer.endWidth = 0.3f;
                zLineRenderer.startWidth = 0.3f;
                zLineRenderer.endWidth = 0.3f;
            }
        }

        if (_isCreatingFloor && Input.GetMouseButton(0) && _currentFloor != null)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, floorPlacementMask))
            {
                _finalMousePos = hit.point;
                _finalMousePos.y = 0f;

                Vector3 scale = _finalMousePos - _initialMousePos;
                _currentFloor.transform.localScale = new Vector3(scale.x, 0.1f, scale.z);

                // Update the UI text with the current dimensions dynamically
                UpdateLinePositions(_initialMousePos, _finalMousePos);
                xLineRenderer.SetPosition(0, _initialMousePos);
                xLineRenderer.SetPosition(1, new Vector3(_finalMousePos.x, 0, _initialMousePos.z));

                zLineRenderer.SetPosition(0, _initialMousePos);
                zLineRenderer.SetPosition(1, new Vector3(_initialMousePos.x, 0, _finalMousePos.z));

                // Get the actual scale of the current floor
                Vector3 actualScale = _currentFloor.transform.localScale;
                float actualScaleX = Mathf.Abs(scale.x);  // Adjust scale calculation
                float actualScaleZ = Mathf.Abs(scale.z);

                Debug.Log("Xactual Size: " + actualScaleX);
                Debug.Log("Zactual Size: " + actualScaleZ);

                // Continuously update the dimensions in the UI as the object is dragged
                UpdateFloorDimensionsText(_initialMousePos, _finalMousePos);
            }
        }


        if (_isCreatingFloor && Input.GetMouseButtonUp(0) && _currentFloor != null)
        {
            xLineRenderer.enabled = false;
            zLineRenderer.enabled = false;
            _isCreatingFloor = false;
            _currentFloor = null;

            xLineRenderer.startWidth = 0f;
            xLineRenderer.endWidth = 0f;
            zLineRenderer.startWidth = 0f;
            zLineRenderer.endWidth = 0f;

            // Hide the lines after the floor is placed
            xLineRenderer.enabled = false;
            zLineRenderer.enabled = false;
            //xDimensionText.text = "";
            //zDimensionText.text = "";

          //  StartCoroutine(HideFloorDimensionsTextAfterDelay(2f));    // Hide the Text After Sometime
           
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
                UpdateFloorDimensionsTextForWall(_initialMousePos, _initialMousePos);
                wallLineRenderer.enabled = true;
              //  zLineRenderer.enabled = true;
                wallLineRenderer.startWidth = 0.3f;
                wallLineRenderer.endWidth = 0.3f;
               

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
                    _wallAlongZAxis = false;
                    var distanceX = _finalMousePos.x - _initialMousePos.x;
                    _currentWall.transform.localScale = new Vector3(distanceX, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 0, 0); // Set rotation to 0 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the x-axis
                    _currentWall.transform.position = new Vector3(_initialMousePos.x + distanceX / 2, 0f, _initialMousePos.z);
                    UpdateFloorDimensionsTextForWall(_initialMousePos, _finalMousePos);
                   // Debug.Log("X-axis ");
                }
                else
                {
                    // Dragging along the z-axis
                    var distanceZ = _finalMousePos.z - _initialMousePos.z;
                    _currentWall.transform.localScale = new Vector3(distanceZ, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 90, 0); // Set rotation to 90 degrees on the y-axis

                    // Keep the initial position the same and update only the position along the z-axis
                    _currentWall.transform.position = new Vector3(_initialMousePos.x, 0f, _initialMousePos.z + distanceZ / 2);
                    _wallAlongZAxis=true;
                    UpdateFloorDimensionsTextForWall(_initialMousePos, _finalMousePos);
                   // Debug.Log("Z-axis ");
                }

                //UpdateLineForWall(_initialMousePos, _finalMousePos);
              //  wallLineRenderer.SetPosition(0, _initialMousePos);
            //    wallLineRenderer.SetPosition(1, new Vector3(_finalMousePos.x, 0, _initialMousePos.z));
                Vector3 actualScale = _currentWall.transform.localScale;
                float actualScaleX = Mathf.Abs(actualScale.x); // Actual scale on X-axis
             
            }
        }

        if (_isCreatingWall && Input.GetMouseButtonUp(0) && _currentWall != null) // Mouse button released
        {
            _isCreatingWall = false; // Reset the wall creation process
            _currentWall = null; // Reset the current wall object

           // ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Floor", "Selectable", "Selected");
            Debug.Log("Wall Up");
          //  wallDimensionText.text = "";
            //StartCoroutine(HideFloorDimensionsTextAfterDelay(2f));    // Hide the Text After Sometime


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
                ApplyMaterial(_previewObject, surface != null ? validPlacementMaterial : invalidPlacementMaterial); // Invalid surface
                // Valid surface
            }
        }

        // Object placement logic
        if (Input.GetMouseButtonUp(0) && _selectedObjectIndex != -1 && _previewObject != null) // Place object on mouse button release
        {
            PlaceObjectOnSurface();
        }
    }

    // Called when the floor button is clicked


    private void UpdateLinePositions(Vector3 start, Vector3 end)
    {
        // X-axis line
        xLineRenderer.SetPosition(0, new Vector3(start.x, 0.01f, start.z));
        xLineRenderer.SetPosition(1, new Vector3(end.x, 0.01f, start.z)); // Draw along X-axis

        // Z-axis line
        zLineRenderer.SetPosition(0, new Vector3(start.x, 0.01f, start.z));
        zLineRenderer.SetPosition(1, new Vector3(start.x, 0.01f, end.z)); // Draw along Z-axis
    }
    private void UpdateLineForWall(Vector3 start, Vector3 end)
    {
        wallLineRenderer.SetPosition(0, new Vector3(start.x, 0.01f, start.z));
        wallLineRenderer.SetPosition(1, new Vector3(end.x, 0.01f, start.z)); // Draw along X-axis
    }

    public void OnFloorButtonClick()
    {
        //ManagerHandler.Instance.objectManipulator.selectableLayer = LayerMask.GetMask("Selectable", "Selected");
        _isCreatingFloor = true;
        _isCreatingWall = false; // Ensure wall creation is not active
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        _previewObject = null;
        _selectedObjectIndex = -1;
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

    private void UpdateFloorDimensionsText(Vector3 start, Vector3 end)
    {
        if (start != end)
        {
            // Get the current scale of the floor based on drag
            Vector3 actualScale = _currentFloor.transform.localScale;
            
            

            // Calculate dynamic scale based on drag for x and z axes
            float actualScaleX = Mathf.Abs(end.x - start.x); // Dynamic calculation based on drag
            float actualScaleZ = Mathf.Abs(end.z - start.z); // Dynamic calculation based on drag

            // Ensure the x-axis length starts at 1
            float displayScaleX = Mathf.Max(1f, actualScaleX); // Set the minimum text value to 1
            float displayScaleZ = actualScaleZ;
          

            // Convert to feet (if needed)
            float lengthInFeetX = displayScaleX;
            float lengthInFeetZ = displayScaleZ;

            // Update the UI text, ensuring the x-axis starts at 1
            xDimensionText.text = $"X Length: {displayScaleX:F2} m / {lengthInFeetX:F2} ft";
            zDimensionText.text = $"Z Length: {displayScaleZ:F2} m / {lengthInFeetZ:F2} ft";
           
            // Update the positions for the dimension text labels
            Vector3 midPointX = new Vector3((start.x + end.x) / 2, 0f, start.z);
            Vector3 midPointZ = new Vector3(start.x+1f , 0.1f, (start.z + end.z) / 2);
            xDimensionText.transform.position = midPointX;
          
            zDimensionText.transform.position = midPointZ;
        }
        else
        {
            // Clear the text when dragging is finished or not happening
            //xDimensionText.text = "";
            //zDimensionText.text = "";
        }
    }




    private void UpdateFloorDimensionsTextForWall(Vector3 start, Vector3 end)
    {
        if (start != end)
        {
            if (_wallAlongZAxis == false)
            {

                Debug.Log("Wall Text Update");
                // Get actual scale of the floor
                Vector3 actualScale = _currentWall.transform.localScale;

                float actualScaleX = Mathf.Abs(actualScale.x); // Actual scale on X-axis


                // Convert to feet (optional)
                float lengthInFeetX = actualScaleX;

               // float lengthInFeetX = actualScaleX;
                RectTransform rectTransform = wallDimensionText.GetComponent<RectTransform>();
                rectTransform.transform.rotation = Quaternion.Euler(90, 0, 0);


                // Update UI with actual dimensions
                wallDimensionText.text = $"X Length: {actualScaleX:F2} m / {lengthInFeetX:F2} ft";


                // Update positions for the text
                Vector3 midPointX = new Vector3((start.x + end.x) / 2, 2f, start.z - 0f);

                wallDimensionText.transform.position = midPointX;
            }
            else
            {
                Vector3 actualScale = _currentWall.transform.localScale;

                float actualScaleX = Mathf.Abs(actualScale.x); // Actual scale on X-axis


                // Convert to feet (optional)
                float lengthInFeetX = actualScaleX;
                RectTransform rectTransform = wallDimensionText.GetComponent<RectTransform>();
                rectTransform.transform.rotation= Quaternion.Euler(90,0,90);
                // Update UI with actual dimensions
                wallDimensionText.text = $"X Length: {actualScaleX:F2} m / {lengthInFeetX:F2} ft";


                // Update positions for the text
                Vector3 midPointX = new Vector3((start.x + end.x)/2.2f , 1f, start.z - 1.24f);

                wallDimensionText.transform.position = midPointX;

            }
           
        }
        else
        {
            // Clear the text when not dragging
           // wallDimensionText.text = "";
            
        }
    }



    private IEnumerator HideFloorDimensionsTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_floorDimensionsText != null)
        {
            _floorDimensionsText.text = ""; // Clear text after delay
        }
    }


}