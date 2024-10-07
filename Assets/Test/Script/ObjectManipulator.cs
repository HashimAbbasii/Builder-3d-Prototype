using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed for rotating the object
    public Transform selectedObject; // The currently selected object
    public Material _originalMaterial; // To store the original material of the object
    public Material selectedMaterial; // Material to apply when the object is selected
    private bool _isDragging; // To track if the object is being dragged
    public Slider scaleSlider; // Slider to control the object's scale
    public CalculateDistance distanceCalculator; // Reference to CalculateDistance script
    public LayerMask selectableLayer; // Layer mask for selectable objects
    public LayerMask placeableLayer; // Layer mask for selected objects

    public ButtonWithTextTMP floorButton;
    public GameObject removeButton; // Button for removing the selected object

    // Store a list of RectTransforms for the rotation buttons
    public RectTransform rotationKnob;
    public List<Button> rotationButtons = new();
    public RectTransform rotationImage;
    private List<RectTransform> _rotationButtonRects = new();
    public bool _isObjectSelected; // Track if the object is currently selected
    public bool isFloorSelected = false;

    private RectTransform _sliderRect; // RectTransform of the slider

    public GameObject bottomPanel;
    [Space(2)]
    [Header("Model Selection Panel")]
    public GameObject FloorTextureChangePanel;
    public GameObject ChairChangePanel;
    public GameObject TableChangePanel;
    private void Start()
    {
        // Get the RectTransform of each rotation button and store it
        // foreach (var button in rotationButtons)
        // {
        //     _rotationButtonRects.Add(button.GetComponent<RectTransform>());
        // }

        // Get the RectTransform of the slider to detect interaction
        _sliderRect = scaleSlider.GetComponent<RectTransform>();

        // Optionally, set up an event listener for the slider
        scaleSlider.onValueChanged.AddListener(ScaleObject);

        scaleSlider.transform.parent.gameObject.SetActive(false);
        floorButton.onClick.AddListener(FloorSelection);
    }

    public void FloorSelection()
    {
        isFloorSelected = true;
        floorButton.buttonText.color = Color.black;
    }

    private void Update()
    {
        // Ensure that there's only one touch on the screen
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            var ray = Camera.main.ScreenPointToRay(touch.position);

            // Handle touch start (equivalent to MouseButtonDown)
            if (touch.phase == TouchPhase.Began && !IsClickOnAnyRotationButton() && !IsClickOnSlider() && !IsClickOnRotationKnob() && !isFloorSelected)
            {
                // Use Raycast with LayerMask to only interact with objects on the selectable layer
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, selectableLayer))
                {
                    var selectedTransform = hit.transform;
                    Debug.Log("Selected Transform: " + selectedTransform.name);

                    // If the object is already selected and touched again, start dragging
                    if (_isObjectSelected && selectedObject == selectedTransform)
                    {
                        _isDragging = true; // Enable dragging
                    }
                    else
                    {
                        // If another object is touched, select the new one
                        if (!hit.collider.gameObject.CompareTag("Floor"))
                        {
                            SetSelectedObject(selectedTransform);
                            _isObjectSelected = true; // Mark object as selected
                            _isDragging = false; // Don't drag yet, only select
                        }
                    }
                }
                else
                {
                    // If touch is outside of any object or UI, deselect the object
                    DeselectObject();
                    _isObjectSelected = false; // Reset selection state
                }
            }

            // Handle floor selection
            if (touch.phase == TouchPhase.Began && isFloorSelected == true)
            {
                if (Physics.Raycast(ray, out var hit) && hit.collider.gameObject.CompareTag("Floor"))
                {
                    Debug.Log("Floor Selected");
                    var selectedTransform = hit.transform;
                    SelectedObjectForFloor(selectedTransform);
                    return;
                }
            }

            // If an object is selected, handle its movement and rotation
            if (selectedObject != null)
            {
                removeButton.SetActive(true);

                // Move the object with the touch when dragging
                if (touch.phase == TouchPhase.Moved && _isDragging)
                {
                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placeableLayer))
                    {
                        // Move object on the XZ plane (ignoring Y-axis)
                        var newPosition = hit.point;
                        selectedObject.parent.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);

                        if (hit.transform.parent?.GetComponent<SelectableObject>())
                        {
                            selectedObject.parent.transform.SetParent(hit.transform.parent);
                        }
                        else
                        {
                            selectedObject.parent.transform.parent = null;
                        }

                        // Update the distance calculation and line renderer
                        if (distanceCalculator != null)
                        {
                            distanceCalculator.CalculateDistances(selectedObject.parent.gameObject);
                        }
                    }
                }

                // Rotate the object using arrow keys or other methods
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    RotateObject(-rotationSpeed * Time.deltaTime); // Rotate left
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    RotateObject(rotationSpeed * Time.deltaTime); // Rotate right
                }

                // Stop dragging when the touch ends
                if (touch.phase == TouchPhase.Ended)
                {
                    _isDragging = false; // Stop dragging
                }
            }
            else
            {
                removeButton.SetActive(false);
                _isObjectSelected = false; // Reset selection state when no object is selected
            }
        }
    }

    // Set the object to be manipulated
    public void SetSelectedObject(Transform obj)
    {
        // Check if the same object is clicked again to toggle selection
        if (selectedObject == obj)
        {
            return; // If it's already selected, don't deselect or reselect
        }

        // Revert the material of the previously selected object
        if (selectedObject != null)
        {
            DeselectObject();
        }

        // Set the new selected object
        selectedObject = obj;

        selectedObject.gameObject.layer = LayerMask.NameToLayer("Selected");


        foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
        {
            childObjects.gameObject.layer = LayerMask.NameToLayer("Selected");
        }

        Debug.Log(selectedObject.tag);

        if (selectedObject.CompareTag("Wall") || selectedObject.CompareTag("Floor"))
        {
            // For Walls and Floor
            scaleSlider.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            // For other objects, show the slider
            scaleSlider.transform.parent.gameObject.SetActive(true);
        }
        //if (selectedObject.CompareTag("Floor")) 
        //{
        //    bottomPanel.SetActive(false);

        //}

        if (selectedObject.parent != null)
        {
            if (!selectedObject.CompareTag("Wall"))
            {
                SelectableObject selectableObject = selectedObject.parent.GetComponent<SelectableObject>();

                if (selectableObject != null)
                {
                    Vector3 originalScale = selectableObject.OriginalScale;

                    // Check for zero in OriginalScale to avoid division by zero
                    if (originalScale.x != 0 && originalScale.y != 0 && originalScale.z != 0)
                    {
                        var scaleValue = selectedObject.parent.localScale.x / originalScale.x;
                        scaleSlider.value = scaleValue;
                        // Use scaleValue as needed
                    }
                    else
                    {
                        Debug.LogError("OriginalScale cannot be zero for any component.");
                    }
                }
                else
                {
                    //Debug.LogError("SelectableObject component not found on the parent.");
                }
            }
            //else if (!selectedObject.CompareTag("Floor"))
            //{
            //    SelectableObject selectableObject = selectedObject.parent.GetComponent<SelectableObject>();

            //    if (selectableObject != null)
            //    {
            //        Vector3 originalScale = selectableObject.OriginalScale;

            //        // Check for zero in OriginalScale to avoid division by zero
            //        if (originalScale.x != 0 && originalScale.y != 0 && originalScale.z != 0)
            //        {
            //            var scaleValue = selectedObject.parent.localScale.x / originalScale.x;
            //            scaleSlider.value = scaleValue;
            //            // Use scaleValue as needed
            //        }
            //        else
            //        {
            //            Debug.LogError("OriginalScale cannot be zero for any component.");
            //        }
            //    }
            //    else
            //    {
            //        //Debug.LogError("SelectableObject component not found on the parent.");
            //    }
            //}
            else
            {
                // Debug.LogError("Selected object's parent is null.");
            }
        }

        // Change the material of the new selected object
        if (selectedObject == null) return;

        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            _originalMaterial = meshRenderer.material; // Store the original material

            meshRenderer.material = selectedMaterial; // Apply the selected material
        }

        // Recalculate distance for the selected object
        if (distanceCalculator != null)
        {
            distanceCalculator.RecalculateDistanceForSelectedObject(selectedObject.gameObject);
        }
    }


    public void SelectedObjectForFloor(Transform FloorSelected)
    {
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
        floorButton.buttonText.color = Color.white;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Floor");

        foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
        {
            childObjects.gameObject.layer = LayerMask.NameToLayer("Floor");
        }


    }


    // Check if the click is on any rotation button by checking mouse position against the RectTransforms
    private bool IsClickOnAnyRotationButton()
    {
        Vector2 localMousePosition;

        // Loop through each rotation button and check if the mouse is over any
        foreach (var rectTransform in _rotationButtonRects)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null,
                out localMousePosition);
            if (rectTransform.rect.Contains(localMousePosition))
            {
                return true; // Mouse is over one of the rotation buttons
            }
        }

        return false; // Mouse is not over any rotation buttons
    }

    // Check if the click is on the slider by checking mouse position against the slider RectTransform
    private bool IsClickOnSlider()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_sliderRect, Input.mousePosition, null,
            out localMousePosition);
        return _sliderRect.rect.Contains(localMousePosition);
    }

    private bool IsClickOnRotationKnob()
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rotationKnob, Input.mousePosition, null,
            out localMousePosition);
        return _sliderRect.rect.Contains(localMousePosition);
    }


    // Rotate the selected object by a specific angle
    public void RotateObject(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.Rotate(Vector3.up, angle, Space.Self); // Rotate around the Y-axis
        }
    }

    public void SetRotation(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.parent.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }

    // Scale the selected object based on the slider value
    public void ScaleObject(float scaleValue)
    {
        if (selectedObject != null)
        {
            //temporary Fix
            selectedObject.parent.localScale =
                selectedObject.parent.GetComponent<SelectableObject>().OriginalScale * scaleValue;
        }
    }

    // Method to revert the material to the original material
    private void RevertMaterial()
    {
        if (selectedObject == null) return;

        var meshRenderer = selectedObject.GetComponent<MeshRenderer>();

        if (meshRenderer != null && _originalMaterial != null)
        {
            Debug.Log("Revert Material");
            meshRenderer.material = _originalMaterial; // Restore the original material
        }

        _originalMaterial = null; // Clear the stored material
    }

    // Method to deselect the currently selected object
    public void DeselectObject()
    {
        foreach (var line in ManagerHandler.Instance.calculateDistance.lines)
        {
            Destroy(line.gameObject);
        }

        ManagerHandler.Instance.calculateDistance.lines.Clear();

        RevertMaterial(); // Revert the material
        if (selectedObject)
        {
            selectedObject.gameObject.layer = LayerMask.NameToLayer("Selectable");

            foreach (var childObjects in selectedObject.parent.GetComponentsInChildren<Collider>())
            {
                childObjects.gameObject.layer = LayerMask.NameToLayer("Selectable");
            }
        }

        selectedObject = null; // Deselect the object
        _isDragging = false; // Stop dragging when deselected
        scaleSlider.transform.parent.gameObject.SetActive(true);
        bottomPanel.SetActive(true);
    }



    public void DeselectForFloor()
    {
        RevertMaterial();
    }

    public void RemoveObject()
    {
        if (selectedObject == null) return;

        foreach (var line in ManagerHandler.Instance.calculateDistance.lines)
        {
            Destroy(line.gameObject);
        }

        ManagerHandler.Instance.calculateDistance.lines.Clear();

        // ManagerHandler.Instance.spawningManager.modelsSpawned.Remove(selectedObject.parent.gameObject);
        Destroy(selectedObject.parent.gameObject);

        Invoke(nameof(DeleteMissingSpawnedModels), 0.1f);

        scaleSlider.transform.parent.gameObject.SetActive(false);
        _isDragging = false; // Stop dragging when deselected
    }

    private bool IsMissing(GameObject obj)
    {
        // The object is missing if it's null or destroyed
        return obj == null || Object.ReferenceEquals(obj, null);
    }

    private void DeleteMissingSpawnedModels()
    {
        ManagerHandler.Instance.spawningManager.modelsSpawned.RemoveAll(
            item => item == null || !item || IsMissing(item));
    }

    // Method to deselect the currently selected object via button click
    public void DeselectButton()
    {
        DeselectObject(); // Call the method to deselect the object
    }

    //public void RemoveObject()
    //{
    //    if (selectedObject == null) return;

    //    Destroy(selectedObject.parent.gameObject);
    //    _isDragging = false; // Stop dragging when deselected
    //}
}
