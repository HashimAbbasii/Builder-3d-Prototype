using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public ObjectManipulator manipulator;

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

            if (Physics.Raycast(ray, out hit))
            {
                SelectableObject selectable = hit.collider.GetComponent<SelectableObject>();

                if (selectable != null)
                {
                    Debug.Log("Object selected: " + hit.collider.name);
                    manipulator.SetSelectedObject(hit.collider.transform);
                }
                else
                {
                    Debug.Log("Object deselected");
                    manipulator.SetSelectedObject(null); // Deselect if not a selectable object
                }
            }
        }
    }
}
