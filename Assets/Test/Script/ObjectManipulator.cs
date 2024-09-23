using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 100f;   // Speed for rotating the object
    public Transform selectedObject;     // The currently selected object
    private Material _originalMaterial;   // To store the original material of the object
    public Material selectedMaterial;    // Material to apply when the object is selected
    private bool _isDragging;     // To track if the object is being dragged
    public Slider scaleSlider;           // Slider to control the object's scale
    public CalculateDistance distanceCalculator; // Reference to CalculateDistance script
    public LayerMask selectableLayer;    // Layer mask for selectable objects
    public LayerMask placeableLayer;    // Layer mask for selected objects
    public GameObject removeButton;      // Button for removing the selected object

    public List<Button> rotationButtons = new();
    private List<RectTransform> _rotationButtonRects = new();
    private bool _isObjectSelected; // Track if the object is currently selected
    private RectTransform _sliderRect;    // RectTransform of the slider

    private void Start()
    {
        foreach (var button in rotationButtons)
        {
            _rotationButtonRects.Add(button.GetComponent<RectTransform>());
        }
        _sliderRect = scaleSlider.GetComponent<RectTransform>();
        scaleSlider.onValueChanged.AddListener(ScaleObject);
    }

    private void Update()
    {
        if (selectedObject == null)
        {
            removeButton.SetActive(false);

            // Check if a new object is being clicked to select
            if (Input.GetMouseButtonDown(0) && !IsClickOnAnyRotationButton() && !IsClickOnSlider())
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, selectableLayer))
                {
                    // New object clicked, select it
                    SetSelectedObject(hit.transform);
                    _isObjectSelected = true; // Mark the object as selected
                }
                else
                {
                    DeselectObject(); // Deselect if clicked elsewhere
                    _isObjectSelected = false;
                }
            }
        }
        else
        {
            // An object is selected; enable the remove button
            removeButton.SetActive(true);

            // Check if the object is clicked again to start dragging
            if (Input.GetMouseButtonDown(0) && !IsClickOnAnyRotationButton() && !IsClickOnSlider())
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, selectableLayer))
                {
                    if (hit.transform == selectedObject)
                    {
                        // Object is already selected, start dragging
                        _isDragging = true;
                    }
                    else
                    {
                        // New object clicked, select the new object
                        SetSelectedObject(hit.transform);
                        _isDragging = false; // Only select, don't drag yet
                    }
                }
                else
                {
                    DeselectObject(); // Deselect if clicked elsewhere
                    _isObjectSelected = false;
                }
            }

            // Drag the object if it's selected and the mouse button is held down
            if (Input.GetMouseButton(0) && _isDragging)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placeableLayer))
                {
                    // Move object on the XZ plane (ignoring Y-axis)
                    var newPosition = hit.point;
                    selectedObject.parent.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);

                    // Update the distance calculation and line renderer
                    if (distanceCalculator != null)
                    {
                        distanceCalculator.CalculateDistances(selectedObject.parent.gameObject);
                    }
                }
            }

            // Stop dragging when the mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            // Rotate the object using arrow keys
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                RotateObject(-rotationSpeed * Time.deltaTime); // Rotate left
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                RotateObject(rotationSpeed * Time.deltaTime); // Rotate right
            }
        }
    }

    public void SetSelectedObject(Transform obj)
    {
        if (selectedObject == obj) return; // If it's already selected, do nothing

        // Deselect the current object if another object is selected
        if (selectedObject != null)
        {
            DeselectObject();
        }

        selectedObject = obj;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Selected");

        // Change the material of the new selected object
        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _originalMaterial = meshRenderer.material; // Store original material
            meshRenderer.material = selectedMaterial; // Apply selected material
        }

        // Recalculate distance for the selected object
        if (distanceCalculator != null)
        {
            distanceCalculator.RecalculateDistanceForSelectedObject(selectedObject.gameObject);
        }

        _isObjectSelected = true; // Mark as selected
        _isDragging = false; // Set dragging to false initially
    }

    public void DeselectObject()
    {
        RevertMaterial();
        if (selectedObject != null)
        {
            selectedObject.gameObject.layer = LayerMask.NameToLayer("Selectable");
        }
        selectedObject = null;
        _isDragging = false;
    }

    public void RotateObject(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.Rotate(Vector3.up, angle, Space.Self); // Rotate around Y-axis
        }
    }

    public void ScaleObject(float scaleValue)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.localScale = selectedObject.parent.GetComponent<SelectableObject>().OriginalScale * scaleValue;
        }
    }

    private bool IsClickOnAnyRotationButton()
    {
        Vector2 localMousePosition;
        foreach (var rectTransform in _rotationButtonRects)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localMousePosition);
            if (rectTransform.rect.Contains(localMousePosition)) return true;
        }
        return false;
    }

    private bool IsClickOnSlider()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_sliderRect, Input.mousePosition, null, out localMousePosition);
        return _sliderRect.rect.Contains(localMousePosition);
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

    public void RemoveObject()
    {
        if (selectedObject == null) return;

        Destroy(selectedObject.parent.gameObject);
        _isDragging = false; // Stop dragging when deselected
    }
}
