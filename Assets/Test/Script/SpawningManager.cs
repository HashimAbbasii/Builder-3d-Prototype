using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using System.Xml.Serialization;
using Random = System.Random;

public class SpawningManager : MonoBehaviour
{
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
    public  bool _isCreatingFloor = false;
    public bool _isCreatingWall = false;
    public bool _wallAlongZAxis = false;
    public bool pauseCondition = false;
    private int _selectedObjectIndex = -1;

    public GameObject floorMaterialChanged;
    public LayerMask surfaceLayerMask;
    public LayerMask floorPlacementMask;

    public List<GameObject> floorsSpawned = new();
    public List<GameObject> wallsSpawned = new();
    public List<GameObject> modelsSpawned = new();

    public GameObject categoryButtonPrefab;   // Prefab for the category button
    public GameObject EvidenceButtonPrefab;   // UI panel for category selection


    public Transform categoryListParent;      // Parent for category buttons (Scroll View's Content)
    public Transform EvidenceListParent;      // Parent for category buttons (Scroll View's Content)

    public GameObject subPanel;               // UI panel for model selection
    public GameObject EvidenceSubPanel;       // UI panel for evidence selection


    public GameObject modelButtonPrefab;      // Prefab for each model button
    public GameObject EvidenceModelButtonPrefab;

    public Transform modelListParent;         // Parent for model buttons (Scroll View's Content)
    public Transform EvidenceModelListParent; // Parent for model buttons (Scroll View's Content)



    public List<GameObject> chairModelPrefabs = new();
    public List<GameObject> tableModelPrefabs = new();
    public List<GameObject> bedModelPrefabs = new();
    public List<GameObject> carpetModelPrefabs = new();

    [Header("Evidence")]

    public List <GameObject> deadBodyPrefabs = new();
    public List <GameObject> bloodPrefabs = new();
    public List <GameObject> knifePrefabs = new();


    public List<GameObject> currentCategoryModels;  // Currently selected category models

    public List<GameObject> currentEvidenceModels;  // Currently selected evidence models
    private int currentPage = 0;
    private int modelsPerPage = 20;
   




    [Space(5)]
    private SelectableObject selectableObject;

    
    public bool IsCreatingWall => _isCreatingWall;
    public bool IsCreatingFloor => _isCreatingFloor;

    private void SetupLineRenderers()
    {
        // Create and configure the material for LineRenderers
        Material transparentMaterial = new Material(lineShader);
        transparentMaterial.color = new Color(1, 0, 0, 0.2f); // Red color with 50% transparency for XLine
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
        //canvasEssential.gameObject.SetActive(true);
        pauseCondition = false;
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.gameObject.SetActive(false);
        SetupLineRenderers(); // Initialize the LineRenderers



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

        foreach (var model in modelPrefabs)
        {
            if (model.GetComponent<SelectableObject>().modelType != ModelType.Furniture || model.GetComponent<SelectableObject>().surfaceType != SurfaceType.Models) continue;
            
            switch (model.GetComponent<SelectableObject>().furnitureType)
            {
                case FurnitureType.Chair:
                    chairModelPrefabs.Add(model);
                    break;

                case FurnitureType.Table:
                    tableModelPrefabs.Add(model);
                    break;

                case FurnitureType.Bed:
                    bedModelPrefabs.Add(model);
                    break;

                case FurnitureType.Carpet:
                    carpetModelPrefabs.Add(model);
                    break;

            }
        }

        foreach (var model in modelPrefabs)
        {
            if (model.GetComponent<SelectableObject>().modelType != ModelType.Evidence || model.GetComponent<SelectableObject>().surfaceType != SurfaceType.Models) continue;

            switch (model.GetComponent<SelectableObject>().evidenceType)
            {
                case EvidenceType.Blood:
                    bloodPrefabs.Add(model);
                    break;
                case EvidenceType.DeadBody:
                    deadBodyPrefabs.Add(model);
                    break;
                    
                case EvidenceType.Knife:
                    knifePrefabs.Add(model);
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

        subPanel.SetActive(false);  // Hide sub-panel initially

        // Create category buttons dynamically
        CreateCategoryButton("Chair");
        CreateCategoryButton("Table");
        CreateCategoryButton("Bed");
        CreateCategoryButton("Carpet");
        //EvidencPanel.SetActive(true);

        // Set initial category (for example, Chairs)
       // SetCurrentCategory("Chair");

        CreateEvidence();



        // UpdateFloorDimensionsText(Vector3.zero, Vector3.zero); // Initialize with zero dimensions
    }


    private void CreateCategoryButton(string categoryName)
    {
        GameObject button = Instantiate(categoryButtonPrefab, categoryListParent);
        button.name = categoryName;
        button.GetComponentInChildren<TextMeshProUGUI>().text = categoryName;
        button.GetComponent<Button>().onClick.AddListener(() => OnCategorySelected(categoryName));
    }

   // public GameObject categoryPanelScrollView;
   // public GameObject canvasEssential;
    private void OnCategorySelected(string category)
    {
        canvasFurniture.SetActive(false);
        SetCurrentCategory(category);
        //categoryPanelScrollView.SetActive(false);
        subPanel.SetActive(true);

    }
    private void OnEvidenceSelected(string category)
    {
        Debug.Log("Evidence Selected: " + category);
        EvidenceScrollView.SetActive(false);
        SetEvidenceCategory(category);
        //categoryPanelScrollView.SetActive(false);
        EvidenceSubPanel.SetActive(true);
    }

    //public GameObject EvidenceSubPanel;
    //private void OnEvidenceSelected(string category)
    //{
    //    //EvidenceSubPanel.SetActive(true);
    //    currentPage = 0;
    //    switch (category)
    //    {
    //        case "Dead Body":
    //            currentEvidenceModels= deadBodyPrefabs;
    //            break;
    //        case "Blood":
    //            currentEvidenceModels = bloodPrefabs;
    //            break;

    //    }
    //    LoadModelEvidence(currentPage);
    //}

    private void SetCurrentCategory(string category)
    {
        currentPage = 0;

        switch (category)
        {
            case "Chair":
                currentCategoryModels = chairModelPrefabs;
                break;
            case "Table":
                currentCategoryModels = tableModelPrefabs;
                break;
            case "Bed":
                currentCategoryModels = bedModelPrefabs;
                break;
            case "Carpet":
                currentCategoryModels = carpetModelPrefabs;
                break;
        }

        LoadModels(currentPage);
    }

    private void SetEvidenceCategory(string category)
    {
        currentPage = 0;

        switch (category)
        {
            case "Dead Body":
                Debug.Log("Dead Body");
                currentEvidenceModels = deadBodyPrefabs;
                break;
            case "Blood":
                currentEvidenceModels = bloodPrefabs;
                break;
                case "Knife":
                currentEvidenceModels = knifePrefabs;
                break;
            
        }

        LoadModelEvidence(currentPage);
    }




    public void NextPage()
    {
        if ((currentPage + 1) * modelsPerPage < currentCategoryModels.Count)
        {
            currentPage++;
            LoadModels(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            LoadModels(currentPage);
        }
    }




    private void LoadModels(int page)
    {
      //  subPanel.SetActive(true);
        // Clear previous buttons
        foreach (Transform child in modelListParent)
        {
            Destroy(child.gameObject);
        }

        // Calculate the range of models to display on this page
        int startIndex = page * modelsPerPage;
        int endIndex = Mathf.Min(startIndex + modelsPerPage, currentCategoryModels.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject button = Instantiate(modelButtonPrefab, modelListParent);
            int index = i;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Model " + (i + 1);
            button.GetComponent<Button>().onClick.AddListener(() => SelectModel(index));
        }
    }


    public void LoadModelEvidence(int page)
    {
        foreach (Transform child in EvidenceModelListParent)
        {
            Destroy(child.gameObject);
        }

        // Calculate the range of models to display on this page
        int startIndex = page * modelsPerPage;
        int endIndex = Mathf.Min(startIndex + modelsPerPage, currentEvidenceModels.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject button = Instantiate(EvidenceModelButtonPrefab, EvidenceModelListParent);
            int index = i;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "Model " + (i + 1);
            button.GetComponent<Button>().onClick.AddListener(() => SelectModelEvidence(index));
        }
    }




    private void Update()
    {
#if UNITY_EDITOR
        // Simulate touch input using the mouse for testing in the Editor
        SimulateTouchInput();
#else
    // Actual touch input for mobile devices
    if (Input.touchCount == 1) 
    {
        Touch touch = Input.GetTouch(0);
        ProcessTouch(touch);
    }
#endif
    }

    // Called when the floor button is clicked

    //.........Procrss Touch ....//

    private void ProcessTouch(Touch touch)
    {
        if(pauseCondition==true) return;
        var ray = Camera.main.ScreenPointToRay(touch.position);

        // Floor creation logic
        if (_isCreatingFloor && touch.phase == TouchPhase.Began)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorPlacementMask))
            {
                _initialMousePos = hit.point;
                _initialMousePos.y = 0f;
                _currentFloor = Instantiate(floorPrefab, _initialMousePos, Quaternion.identity);
                floorsSpawned.Add(_currentFloor);
                floorMaterialChanged = _currentFloor;
            }
        }

        if (_isCreatingFloor && touch.phase == TouchPhase.Moved && _currentFloor != null)
        {
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, floorPlacementMask))
            {
                _finalMousePos = hit.point;
                _finalMousePos.y = 0f;

                Vector3 scale = _finalMousePos - _initialMousePos;
                _currentFloor.transform.localScale = new Vector3(scale.x, 0.1f, scale.z);

                float actualScaleX = Mathf.Abs(scale.x);  // Adjust scale calculation
                float actualScaleZ = Mathf.Abs(scale.z);

               
            }
        }

        if (_isCreatingFloor && touch.phase == TouchPhase.Ended && _currentFloor != null)
        {
            _isCreatingFloor = false;
            _currentFloor = null;


             Invoke(nameof(DeleteLinesForAll), 0.5f);
        }

        // Wall creation logic
        if (_isCreatingWall && touch.phase == TouchPhase.Began)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _initialMousePos = hit.point;
                _initialMousePos.y = 0f;
                _currentWall = Instantiate(wallPrefab, _initialMousePos, Quaternion.identity);
                wallsSpawned.Add(_currentWall);
            }
        }

        if (_isCreatingWall && touch.phase == TouchPhase.Moved && _currentWall != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _finalMousePos = hit.point;
                _finalMousePos.y = 0f;

                var direction = _finalMousePos - _initialMousePos;

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                {
                    _wallAlongZAxis = false;
                    var distanceX = _finalMousePos.x - _initialMousePos.x;
                    _currentWall.transform.localScale = new Vector3(distanceX, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _currentWall.transform.position = new Vector3(_initialMousePos.x + distanceX / 2, 0f, _initialMousePos.z);
                }
                else
                {
                    var distanceZ = _finalMousePos.z - _initialMousePos.z;
                    _currentWall.transform.localScale = new Vector3(distanceZ, _currentWall.transform.localScale.y, _currentWall.transform.localScale.z);
                    _currentWall.transform.rotation = Quaternion.Euler(0, 90, 0);
                    _currentWall.transform.position = new Vector3(_initialMousePos.x, 0f, _initialMousePos.z + distanceZ / 2);
                    _wallAlongZAxis = true;
                }
            }
        }

        if (_isCreatingWall && touch.phase == TouchPhase.Ended && _currentWall != null)
        {
            _isCreatingWall = false;
            _currentWall = null;

            Invoke(nameof(DeleteLinesForAll), 0.5f);
        }

        // Preview object logic
        if (_previewObject != null)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, surfaceLayerMask, QueryTriggerInteraction.Collide))
            {
                var previewPosition = hit.point;
                previewPosition.y = 0f;
                _previewObject.transform.position = previewPosition;

                var surface = hit.collider.GetComponent<ObjectType>();
                ApplyMaterial(_previewObject, surface != null ? validPlacementMaterial : invalidPlacementMaterial);
            }
        }

        if (touch.phase == TouchPhase.Ended && _selectedObjectIndex != -1 && _previewObject != null)
        {
          //  PlaceObjectOnSurface();
        }
    }


    //..................Editor Testing ......................//


#if UNITY_EDITOR
    private void SimulateTouchInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Touch touch = new Touch
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = TouchPhase.Began
            };
            ProcessTouch(touch);
        }

        if (Input.GetMouseButton(0))
        {
            Touch touch = new Touch
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = TouchPhase.Moved
            };
            ProcessTouch(touch);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Touch touch = new Touch
            {
                fingerId = 0,
                position = Input.mousePosition,
                phase = TouchPhase.Ended
            };
            ProcessTouch(touch);
        }
    }
#endif
    
    public void Pausing()
    {
        ManagerHandler.Instance.collectiveDistanceManager.ToggleObjectDistanceHandlerScript(false);
        _isCreatingWall = false;
        _isCreatingFloor = false;
        if (_previewObject != null)
        {
            Destroy(_previewObject);
        }
        _previewObject = null;
        _selectedObjectIndex = -1;
    }

    public void OnFloorButtonClick()
    {
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.gameObject.SetActive(true);
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.lines = 4;
        
        ManagerHandler.Instance.collectiveDistanceManager.ToggleObjectDistanceHandlerScript(false);
        
        _isCreatingFloor = true;
        _isCreatingWall = false; // Ensure wall creation is not active
        if (_previewObject != null)
        {
            //Destroy(_previewObject);
        }
        _previewObject = null;
        _selectedObjectIndex = -1;
    }
    
    
    // Called when the wall button is clicked
    public void OnWallButtonClick()
    {
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.gameObject.SetActive(true);
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.lines = 2;
        
        ManagerHandler.Instance.collectiveDistanceManager.ToggleObjectDistanceHandlerScript(false);
        
        _isCreatingWall = true;
        _isCreatingFloor = false; // Ensure floor creation is not active
        if (_previewObject != null)
        {
           // Destroy(_previewObject);
        }
        _previewObject = null;
        _selectedObjectIndex = -1;
    }


    public GameObject canvasFurniture;
    public GameObject EvidenceScrollView;
    public Transform[] RandomPointSpawn;
    
    //public GameObject scrollViewFurniture;

    private void SelectModel(int objectIndex)
    {
        // subPanel.SetActive(true);
       // canvasFurniture.SetActive(false);
      //  if (_selectedObjectIndex == objectIndex)
     //   {
            //if (_previewObject != null)
            //{
            //    Destroy(_previewObject);
            //}
          //  _selectedObjectIndex = -1;
          //  return;
    //    }

        _selectedObjectIndex = objectIndex;

        //if (_previewObject != null)
        //{
        //    Destroy(_previewObject);
        //}
        int RandomPoint =UnityEngine.Random.Range(0, RandomPointSpawn.Length);

        _previewObject = Instantiate(currentCategoryModels[objectIndex], RandomPointSpawn[RandomPoint].position,Quaternion.identity);
           
      
       // subPanel.SetActive(false);  // Hide sub-panel after selection
        //canvasEssential.gameObject.SetActive(true); 
    }


    private void SelectModelEvidence(int objectIndex)
    {
        EvidenceSubPanel.SetActive(true); // Display the sub-panel when a model is selected

        // Destroy the previous object if it exists
        if (_previewObject != null)
        {
            //Destroy(_previewObject);
        }

        _selectedObjectIndex = objectIndex;

        // Instantiate the selected model at a random spawn point
        int RandomPoint = UnityEngine.Random.Range(0, RandomPointSpawn.Length);
        _previewObject = Instantiate(currentEvidenceModels[objectIndex], RandomPointSpawn[RandomPoint].position, Quaternion.identity);

        
    }








    // Called when an object button (like Chair, Table) is clicked
    public void SelectObject(int objectIndex)
    {
        _isCreatingWall = false;
        _isCreatingFloor = false;
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.gameObject.SetActive(false);

        ManagerHandler.Instance.collectiveDistanceManager.ToggleObjectDistanceHandlerScript(false);
        
        if (objectIndex == _selectedObjectIndex)
        {
            if (_previewObject != null)
            {
                Destroy(_previewObject);
            }
            _previewObject = null;
            _selectedObjectIndex = -1;
            return;
        }
        
        
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


    //to change the object model
    public void ChangeModel(int number)
    {
       foreach(var models in selectableObject.ModelVariants)
       {
            models.SetActive(false);
       }
        selectableObject.ModelVariants[number].SetActive(true);
    }
    public void DeleteLinesForAll()
    {
        ManagerHandler.Instance.collectiveDistanceManager.essentialDistanceManager.DeleteLines();
        //MeasureLine_WorldCanvas.DeleteAllLines();
        // gameObject.SetActive(false);

    }

    public bool EvidenceButtonClick = true;
     public void CreateEvidence()
    {

     
        //categoryPanelScrollView.SetActive(false);
        //EvidencPanel.gameObject.SetActive(true);
        CreateEvidenceButton("Dead Body");
        CreateEvidenceButton("Blood");
        CreateEvidenceButton("Knife");
        Debug.Log("Evidence created");
        //canvasEssential.gameObject.SetActive(false);
        //OnEvidenceSelected("Dead Body");
      


    }
    public GameObject EvidencPanel;
    private void CreateEvidenceButton(string categoryName)
    {
        GameObject button = Instantiate(EvidenceButtonPrefab, EvidenceListParent);
        button.name = categoryName;
        button.GetComponentInChildren<TextMeshProUGUI>().text = categoryName;
        button.GetComponent<Button>().onClick.AddListener(() => OnEvidenceSelected(categoryName));
    }

    public GameObject essentialPanel;
    public GameObject FurniturePanel;
    public GameObject EvidencePanel;
    public void EssentialPanelClick()
    {
        essentialPanel.gameObject.SetActive(true);
        FurniturePanel.gameObject.SetActive(false);
        EvidencePanel.gameObject.SetActive(false);
    }
    public void FurniturePanelClick()
    {
        essentialPanel.gameObject.SetActive(false);
        FurniturePanel.gameObject.SetActive(true);
        EvidencePanel.gameObject.SetActive(false);
    }
    public void EvidencePanelClick()
    {
        essentialPanel.gameObject.SetActive(false);
        FurniturePanel.gameObject.SetActive(false);
        EvidencePanel.gameObject.SetActive(true);
    }
}