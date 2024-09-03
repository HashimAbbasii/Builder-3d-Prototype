using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public GameObject[] objectsToPlace; // Array to hold possible objects (Chair, Table, Wall, etc.)
    public LayerMask floorLayer; // Layer for the floor
    private GameObject selectedObject; // The currently selected object to place
    private bool isObjectSelected = false;

    // This method is called when a button is clicked to select an object
    public void SelectObject(int index)
    {
        if (index >= 0 && index < objectsToPlace.Length)
        {
            selectedObject = objectsToPlace[index];
            isObjectSelected = true;
        }
        else
        {
            Debug.LogError("Invalid object index.");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isObjectSelected && Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }


    void PlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something on the floor layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayer))
        {
            if (hit.collider != null)
            {
                Instantiate(selectedObject, hit.point, Quaternion.identity);
                isObjectSelected = false; // Deselect the object after placing it
            }
        }
    }
}
