using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    public ObjectManipulator manipulator;

    void Start()
    {
        manipulator = FindObjectOfType<ObjectManipulator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<SelectableObject>() != null)
                {
                    Debug.Log("Select Object");
                    manipulator.SetSelectedObject(hit.collider.transform);
                }
                else
                {
                    manipulator.SetSelectedObject(null); // Deselect if not a selectable object
                }
            }
        }
    }
}
