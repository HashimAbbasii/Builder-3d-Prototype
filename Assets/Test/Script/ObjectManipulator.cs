using UnityEngine;
using UnityEngine.UI;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed for rotating the object
    public Transform selectedObject;  // The currently selected object
    private Material originalMaterial; // To store the original material of the object
    public Material selectedMaterial;  // Material to apply when the object is selected
    private bool isDragging = false;   // To track if the object is being dragged
    public Slider scaleSlider;         // Slider to control the object's scale
    public CalculateDistance distanceCalculator; // Reference to CalculateDistance script

    void Update()
    {
        // Only process if an object is selected
        if (selectedObject != null)
        {
            // Move the object with the mouse when the left mouse button is held down
            if (Input.GetMouseButton(0) && isDragging)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Move object on the XZ plane (ignoring Y-axis)
                    Vector3 newPosition = hit.point;
                    selectedObject.position = new Vector3(newPosition.x, selectedObject.position.y, newPosition.z);

                    // Update the distance calculation and line renderer
                    if (distanceCalculator != null)
                    {
                        distanceCalculator.CalculateDistances(selectedObject.gameObject);
                    }
                }
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

            // Stop dragging when the mouse button is released
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }

    // Set the object to be manipulated
    public void SetSelectedObject(Transform obj)
    {
        // Check if the same object is clicked again to toggle selection
        if (selectedObject == obj)
        {
            DeselectObject();
            return;
        }

        // Revert the material of the previously selected object
        if (selectedObject != null)
        {
            RevertMaterial();
        }

        // Set the new selected object
        selectedObject = obj;

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
            // Scale on all axes
            Vector3 newScale = new Vector3(scaleValue, scaleValue, scaleValue);
            selectedObject.localScale = newScale;
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
        selectedObject = null; // Deselect the object
        isDragging = false; // Stop dragging when deselected
    }

    // Method to deselect the currently selected object via button click
    public void DeselectButton()
    {
        DeselectObject(); // Call the method to deselect the object
    }
}
