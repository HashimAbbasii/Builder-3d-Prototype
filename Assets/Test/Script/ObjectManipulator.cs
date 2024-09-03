using UnityEngine;
using UnityEngine.UI;

public class ObjectManipulator : MonoBehaviour
{
    public float rotationSpeed = 100f;
    private Transform selectedObject;
    private Material originalMaterial;
    public Material selectedMaterial;
    private bool isDragging = false;

    void Update()
    {
        if (selectedObject != null)
        {
            // Move the object with the mouse if the left mouse button is held down
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Assuming you want to move the object on the XZ plane
                    Vector3 newPosition = hit.point;
                    selectedObject.position = new Vector3(newPosition.x, selectedObject.position.y, newPosition.z);
                    isDragging = true;
                }
            }

            // Stop dragging when the left mouse button is released
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
            }
        }
    }

    // Set the object to be manipulated
    public void SetSelectedObject(Transform obj)
    {
        // Revert the material of the previously selected object, if any
        if (selectedObject != null)
        {
            MeshRenderer meshRenderer = selectedObject.GetComponent<MeshRenderer>();
            if (meshRenderer != null && originalMaterial != null)
            {
                meshRenderer.material = originalMaterial;
            }
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
    public Slider ScaleSlider;
    // Scale the selected object based on slider value
    public void ScaleObject(float scaleValue)
    {
        float valueIncreasedDecreased=ScaleSlider.value;
        Debug.Log("Value" + valueIncreasedDecreased);
        
        if (selectedObject != null)
        {
        //    // Scale only on X and Z axes, keeping Y axis scale unchanged
        //    Debug.Log("Scaling object with value: " + scaleValue); // Debug line
            Vector3 newScale = new Vector3(valueIncreasedDecreased, selectedObject.localScale.y, valueIncreasedDecreased);
            selectedObject.localScale = newScale;
        }
    }
}
