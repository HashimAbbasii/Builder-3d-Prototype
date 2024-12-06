using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public ObjectManipulator manipulator;
    public LayerMask surfaceLayer;

    // Public flag to indicate if an object is selected
    public bool isObjectSelected { get; private set; }

    void Start()
    {
        if (manipulator == null)
        {
            manipulator = FindObjectOfType<ObjectManipulator>();
            if (manipulator == null)
            {
                Debug.LogError("ObjectManipulator script not found in the scene.");
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, surfaceLayer))
            {
                SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();

                if (selectable != null)
                {
                    Debug.Log("Object selected: " + hit.collider.name);
                    isObjectSelected = true; // Set flag when an object is selected
                    manipulator.SetSelectedObject(hit.collider.transform);
                }
                else
                {
                    Debug.Log("Object deselected");
                    isObjectSelected = false; // Clear flag when deselecting
                    manipulator.SetSelectedObject(null);
                }
            }
        }
    }
}
