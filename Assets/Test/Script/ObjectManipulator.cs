using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 100f;   // Speed for rotating the object
    public Transform selectedObject;     // The currently selected object
    private Material originalMaterial;   // To store the original material of the object
    public Material selectedMaterial;    // Material to apply when the object is selected
    private bool isDragging = false;     // To track if the object is being dragged
    public Slider scaleSlider;           // Slider to control the object's scale
    public CalculateDistance distanceCalculator; // Reference to CalculateDistance script
    public LayerMask selectableLayer;    // Layer mask for selectable objects
    public LayerMask placeableLayer;    // Layer mask for selected objects
    public GameObject removeButton;      // Button for removing the selected object

    // Store a list of RectTransforms for the rotation buttons
    public List<Button> rotationButtons = new List<Button>();
    private List<RectTransform> rotationButtonRects = new List<RectTransform>();

    private RectTransform sliderRect;    // RectTransform of the slider

    void Start()
    {
        // Get the RectTransform of each rotation button and store it
        foreach (Button button in rotationButtons)
        {
            rotationButtonRects.Add(button.GetComponent<RectTransform>());
        }

        // Get the RectTransform of the slider to detect interaction
        sliderRect = scaleSlider.GetComponent<RectTransform>();

        // Optionally, set up an event listener for the slider
        scaleSlider.onValueChanged.AddListener(ScaleObject);
    }

    private void Update()
    {
        // Check if the mouse is clicked and it's not over any UI elements (rotation buttons or slider)
        if (Input.GetMouseButtonDown(0) && !IsClickOnAnyRotationButton() && !IsClickOnSlider())
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Use Raycast with LayerMask to only interact with objects on the selectable layer
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, selectableLayer))
            {
                var selectedTransform = hit.transform;

                // This object is selectable, set it as the selected object
                SetSelectedObject(selectedTransform);
            }
            else
            {
                // Deselect if the click is not on any rotation buttons or the slider
                if (!IsClickOnAnyRotationButton() && !IsClickOnSlider())
                {
                    DeselectObject();
                }
            }
        }

        // Continue manipulating the selected object if it exists
        if (selectedObject != null)
        {
            removeButton.SetActive(true);

            // Move the object with the mouse when the left mouse button is held down
            if (Input.GetMouseButton(0) && isDragging)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, placeableLayer))
                {
                    // Move object on the XZ plane (ignoring Y-axis)
                    var newPosition = hit.point;

                    // if (hit.transform.GetComponentInParent<SelectableObject>() && hit.transform.GetComponentInParent<SelectableObject>().canBePlacedOnObject)
                    // {
                    //     
                    // }
                    
                    selectedObject.position = new Vector3(newPosition.x, newPosition.y, newPosition.z);
                    
                    
                    // Update the distance calculation and line renderer
                    if (distanceCalculator != null)
                    {
                        distanceCalculator.CalculateDistances(selectedObject.gameObject);
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

            // Stop dragging when the mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
        else
        {
            removeButton.SetActive(false);
        }
    }

    // Check if the click is on any rotation button by checking mouse position against the RectTransforms
    private bool IsClickOnAnyRotationButton()
    {
        Vector2 localMousePosition;

        // Loop through each rotation button and check if the mouse is over any
        foreach (var rectTransform in rotationButtonRects)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localMousePosition);
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
        RectTransformUtility.ScreenPointToLocalPointInRectangle(sliderRect, Input.mousePosition, null, out localMousePosition);
        return sliderRect.rect.Contains(localMousePosition);
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
            // if (selectedObject)
            //     selectedObject.gameObject.layer = LayerMask.NameToLayer("Selectable");
            // RevertMaterial();

            DeselectObject();
        }

        // Set the new selected object
        selectedObject = obj;
        selectedObject.gameObject.layer = LayerMask.NameToLayer("Selected");
        
        
        // Change the material of the new selected object
        if (selectedObject != null)
        {
            MeshRenderer meshRenderer = selectedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                originalMaterial = meshRenderer.material; // Store the original material
                meshRenderer.material = selectedMaterial; // Apply the selected material
            }

            // Recalculate distance for the selected object
            if (distanceCalculator != null)
            {
                distanceCalculator.RecalculateDistanceForSelectedObject(selectedObject.gameObject);
            }

            isDragging = true; // Enable dragging when a new object is selected
        }
    }

    // Rotate the selected object by a specific angle
    public void RotateObject(float angle)
    {
        if (selectedObject != null)
        {
            selectedObject.Rotate(Vector3.up, angle, Space.Self); // Rotate around the Y-axis
        }
    }

    // Scale the selected object based on the slider value
    public void ScaleObject(float scaleValue)
    {
        if (selectedObject != null)
        {
            selectedObject.localScale = selectedObject.parent.GetComponent<SelectableObject>().OriginalScale * scaleValue;
        }
    }

    // Method to revert the material to the original material
    private void RevertMaterial()
    {
        if (selectedObject != null)
        {
            MeshRenderer meshRenderer = selectedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && originalMaterial != null)
            {
                meshRenderer.material = originalMaterial; // Restore the original material
            }
            originalMaterial = null; // Clear the stored material
        }
    }

    // Method to deselect the currently selected object
    public void DeselectObject()
    {
        RevertMaterial(); // Revert the material
        if (selectedObject)
            selectedObject.gameObject.layer = LayerMask.NameToLayer("Selectable");
        selectedObject = null; // Deselect the object
        isDragging = false; // Stop dragging when deselected
    }

    public void RemoveObject()
    {
        if (selectedObject != null)
        {
            Destroy(selectedObject.gameObject);
            isDragging = false; // Stop dragging when deselected
        }
    }

    // Method to deselect the currently selected object via button click
    public void DeselectButton()
    {
        DeselectObject(); // Call the method to deselect the object
    }
}
