using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class ObjectManipulator : MonoBehaviour
{
    [Header("Object Selection Settings")]
    public float rotationSpeed = 100f;
    public Transform selectedObject;
    public Material _originalMaterial;
    public Material selectedMaterial;

    [Header("Interaction States")]
    public bool _isDragging = false;
    public bool _isObjectSelected = false;
    public bool isFloorSelected = false;

    [Header("UI References")]
    public GameObject sliderParent;
    public Slider scaleSlider;
    public ButtonWithTextTMP floorButton;
    public GameObject removeButton;
    public RectTransform rotationKnob;
    public GameObject bottomPanel;

    [Header("Layer Masks")]
    public LayerMask selectableLayer;
    public LayerMask placeableLayer;

    [Header("Component References")]
    public SpawningManager spawningManager;
    public CalculateDistance distanceCalculator;
    public CircularRangeControl circularRangeControl;

    [Header("Panel References")]
    public GameObject FloorTextureChangePanel;
    public GameObject ChairChangePanel;
    public GameObject TableChangePanel;

    private RectTransform _sliderRect;

    private void Start()
    {
        _sliderRect = scaleSlider.GetComponent<RectTransform>();
        scaleSlider.onValueChanged.AddListener(ScaleObject);
        scaleSlider.transform.parent.gameObject.SetActive(false);
        floorButton.onClick.AddListener(FloorSelection);
        bottomPanel.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (spawningManager.pauseCondition) return;

        if (Input.touchCount == 1)
        {
            Debug.Log("Touch Count " +Input.touchCount);
            if (ManagerHandler.Instance.spawningManager.IsCreatingFloor ||
                ManagerHandler.Instance.spawningManager.IsCreatingWall)
                return;

            Touch touch = Input.GetTouch(0);
            var ray = Camera.main.ScreenPointToRay(touch.position);

            HandleObjectSelection(touch, ray);
            HandleObjectMovement(touch, ray);
        }
    }

    private void HandleObjectSelection(Touch touch, Ray ray)
    {
        // Ensure no action is taken when interacting with UI
        if (IsPointerOverUIElement())
        {
            Debug.Log("UI interaction detected, ignoring selection.");
            return;
        }

        if (touch.phase == TouchPhase.Began &&
            !IsClickOnSlider() &&
            !IsClickOnRotationKnob() &&
            !isFloorSelected)
        {
            if (IsClickOnBottomPanel()) return;

            float sphereRadius = 0.5f;
            if (Physics.SphereCast(ray, sphereRadius, out var hit, Mathf.Infinity, selectableLayer))
            {
                var selectedTransform = hit.transform;

                if (_isObjectSelected && selectedObject == selectedTransform)
                {
                    _isDragging = true;
                }
                else
                {
                    if (!hit.collider.gameObject.CompareTag("Floor"))
                    {
                        SetSelectedObject(selectedTransform);
                        _isObjectSelected = true;
                        _isDragging = false;
                    }
                }
            }
            else
            {
                Debug.Log("No object selected");
                Invoke(nameof(DeselectObject), 0.3f);
                _isObjectSelected = false;
            }
        }

        // Handle floor selection
        if (touch.phase == TouchPhase.Began && isFloorSelected)
        {
            if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject.CompareTag("Floor"))
            {
                var selectedTransform = hit.transform;
                SelectedObjectForFloor(selectedTransform);
            }
        }
    }
    private void HandleObjectMovement(Touch touch, Ray ray)
    {
        if (selectedObject != null)
        {
            removeButton.SetActive(true);

            // Move object when dragging
            if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placeableLayer))
                {
                    var newPosition = hit.point;
                    selectedObject.parent.position = newPosition;

                    UpdateObjectParenting(hit);
                    UpdateDistanceCalculation();
                }
            }

            // Rotate object with arrow keys
            HandleRotationInput();

            // Stop dragging
            if (touch.phase == TouchPhase.Ended)
            {
                _isDragging = false;
            }
        }
        else
        {
            removeButton.SetActive(false);
            _isObjectSelected = false;
        }
    }


    private bool IsPointerOverUIElement()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the first touch is over a UI element
            return IsTouchOverUIElement(touch);
        }

        // Fallback to mouse check for editor/standalone
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Specific method to check if a touch is over a UI element
    private bool IsTouchOverUIElement(Touch touch)
    {
        // Create a pointer event data for the touch position
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current)
        {
            position = touch.position
        };

        // Create a list to store raycast results
        List<RaycastResult> results = new List<RaycastResult>();

        // Raycast using the event system
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        // Return true if any UI element was hit
        return results.Count > 0;
    }



    public void SetRotation(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }



    private void UpdateObjectParenting(RaycastHit hit)
    {
        if (hit.transform.parent?.GetComponent<SelectableObject>())
        {
            selectedObject.parent.transform.SetParent(hit.transform.parent);
        }
        else
        {
            selectedObject.parent.transform.parent = null;
        }
    }

    private void UpdateDistanceCalculation()
    {
        if (distanceCalculator != null)
        {
            distanceCalculator.CalculateDistances(selectedObject.parent.gameObject);
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateObject(-rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateObject(rotationSpeed * Time.deltaTime);
        }
    }

    public void FloorSelection()
    {
        ManagerHandler.Instance.collectiveDistanceManager.ToggleObjectDistanceHandlerScript(false);
        isFloorSelected = true;
        floorButton.buttonText.color = ManagerHandler.Instance.uiManager.canvasHandler.textSelectedColor;
        rotationKnob.gameObject.SetActive(false);
        sliderParent.SetActive(false);
    }

    public void SetSelectedObject(Transform obj)
    {
        Debug.Log("C");
        if (obj != null)
        {
            rotationKnob.gameObject.SetActive(true);
            sliderParent.SetActive(true);
        }

        if (selectedObject == obj) return;

        if (selectedObject != null)
        {
            DeselectObject();
        }

        selectedObject = obj;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Selected");

        foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
        {
            childObjects.gameObject.layer = LayerMask.NameToLayer("Selected");
        }

        ConfigureSliderAndUI();
      ApplySelectedMaterial();
       RecalculateDistance();
    }

    private void ConfigureSliderAndUI()
    {
        if (selectedObject.CompareTag("Wall") || selectedObject.CompareTag("Floor"))
        {
            scaleSlider.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            scaleSlider.transform.parent.gameObject.SetActive(true);
            UpdateScaleSliderValue();
        }
    }

    private void UpdateScaleSliderValue()
    {
        if (selectedObject.parent != null && !selectedObject.CompareTag("Wall"))
        {
            SelectableObject selectableObject = selectedObject.parent.GetComponent<SelectableObject>();

            if (selectableObject != null)
            {
                Vector3 originalScale = selectableObject.OriginalScale;

                if (originalScale.x != 0 && originalScale.y != 0 && originalScale.z != 0)
                {
                    var scaleValue = selectedObject.parent.localScale.x / originalScale.x;
                    scaleSlider.value = scaleValue;
                }
            }
        }
    }

    private void ApplySelectedMaterial()
    {
        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _originalMaterial = meshRenderer.material;
            meshRenderer.material = selectedMaterial;
        }
    }

    private void RecalculateDistance()
    {
        if (distanceCalculator != null)
        {
            distanceCalculator.RecalculateDistanceForSelectedObject(selectedObject.gameObject);
        }
    }

    public void RotateObject(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.Rotate(Vector3.up, angle, Space.Self);
        }
    }

    public void ScaleObject(float scaleValue)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.localScale =
                selectedObject.parent.GetComponent<SelectableObject>().OriginalScale * scaleValue;
        }
    }

    public void DeselectObject()
    {
        RevertMaterial();
        ResetObjectLayers();
        ResetUIElements();
    }

    private void RevertMaterial()
    {
        if (selectedObject == null) return;

        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null && _originalMaterial != null)
        {
            meshRenderer.material = _originalMaterial;
        }

        _originalMaterial = null;
    }

    private void ResetObjectLayers()
    {
        if (selectedObject)
        {
            selectedObject.gameObject.layer = LayerMask.NameToLayer("Selectable");

            foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
            {
                childObjects.gameObject.layer = LayerMask.NameToLayer("Selectable");
            }
        }

        selectedObject = null;
        _isDragging = false;
    }

    private void ResetUIElements()
    {
        scaleSlider.transform.parent.gameObject.SetActive(true);
        bottomPanel.SetActive(true);
        circularRangeControl.imageSelected.fillAmount = 0;
        circularRangeControl._currentValue = 0;
        circularRangeControl.angle.text = "";
    }

    // Utility methods for interaction detection
    private bool IsClickOnSlider()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _sliderRect, Input.mousePosition, null, out localMousePosition);
        return _sliderRect.rect.Contains(localMousePosition);
    }

    private bool IsClickOnRotationKnob()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rotationKnob, Input.mousePosition, null, out localMousePosition);
        return _sliderRect.rect.Contains(localMousePosition);
    }

    private bool IsClickOnBottomPanel()
    {
        Vector2 localMousePosition;
        RectTransform bottomPanelRect = bottomPanel.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bottomPanelRect, Input.mousePosition, null, out localMousePosition);
        return bottomPanelRect.rect.Contains(localMousePosition);
    }

    // Additional methods for object removal and floor selection
    public void RemoveObject()
    {
        // Object removal logic remains the same as in your original script
        if (selectedObject == null) return;

        // foreach (var line in ManagerHandler.Instance.calculateDistance.lines)
        // {
        //     Destroy(line.gameObject);
        // }
        //
        // ManagerHandler.Instance.calculateDistance.lines.Clear();

        // ManagerHandler.Instance.spawningManager.modelsSpawned.Remove(selectedObject.parent.gameObject);

        var startPoint = selectedObject.GetComponentsInChildren<MeasureLine_WorldCanvas>();
        var endPoint = selectedObject.GetComponentsInChildren<EndPointReference>();

        for (int i = 0; i < startPoint.Length; i++)
        {
            MeasureLine_WorldCanvas line = startPoint[i];
            ManagerHandler.Instance.collectiveDistanceManager.objectDistanceHandler.lineCanvasList.Remove(line);
            foreach (var end in line.targetObjects)
            {
                Destroy(end.gameObject);
            }

            Destroy(line.gameObject);
        }

        for (int i = 0; i < endPoint.Length; i++)
        {
            MeasureLine_WorldCanvas line = endPoint[i].startPoint;
            ManagerHandler.Instance.collectiveDistanceManager.objectDistanceHandler.lineCanvasList.Remove(line);
            foreach (var end in line.targetObjects)
            {
                Destroy(end.gameObject);
            }

            Destroy(line.gameObject);
        }

        Destroy(selectedObject.parent.gameObject);

        Invoke(nameof(DeleteMissingSpawnedModels), 0.1f);

        // Invoke(nameof(TurnOffRemoveButton), 0.125f);
        scaleSlider.transform.parent.gameObject.SetActive(false);
        _isDragging = false; // Stop dragging when deselected
        removeButton.SetActive(false);
        Debug.Log("Remove");

    }

    public void SelectedObjectForFloor(Transform FloorSelected)
    {
        // Floor selection logic remains the same as in your original script
        if (selectedObject == FloorSelected)
        {
            Debug.Log("Deselect");
            DeselectForFloor();
            return;
        }

        selectedObject = FloorSelected;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("SelectedFloor");

        foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
        {
            childObjects.gameObject.layer = LayerMask.NameToLayer("SelectedFloor");
        }

        if (selectedObject == null) return;
        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _originalMaterial = meshRenderer.material; // Store the original material
            meshRenderer.material = selectedMaterial; // Apply the selected material
        }

        isFloorSelected = false;
        floorButton.buttonText.color = ManagerHandler.Instance.uiManager.canvasHandler.textUnselectedColor;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Floor");

        foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
        {
            childObjects.gameObject.layer = LayerMask.NameToLayer("Floor");
        }


    }

    private void DeleteMissingSpawnedModels()
    {
        ManagerHandler.Instance.spawningManager.modelsSpawned.RemoveAll(
            item => item == null || !item || IsMissing(item));
    }
    private bool IsMissing(GameObject obj)
    {
        // The object is missing if it's null or destroyed
        return obj == null || Object.ReferenceEquals(obj, null);
    }
    public void DeselectForFloor()
    {
        RevertMaterial();
    }




}