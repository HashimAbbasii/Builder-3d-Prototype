using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    [SerializeField]
    public GameObject SelectedObject;
    public Material selectedMaterial; // The material to apply when an object is selected
    private Material originalMaterial; // To store the original material of the previously selected object
    public Button selectButton1;
    public Button selectRotation1; // Button 1 to activate selection mode
    public Button selectRotation2;
    public Button selectRotation3;
    public Button selectRotation4;
    public Button selectRotation5;
    public Button selectRotation6;
    public Button selectResetRotation;


    public Button selectButton2; // Button 2 to activate selection mode
    public Button selectButton3; // Button 3 to activate selection mode
    private bool isSelectionModeActive = false; // To track if selection mode is active
    public ObjectHandler selectedObject;


    void Start()
    {
        // Assign the activate selection mode to the three specific buttons
        if (selectButton1 != null)
        {
            selectButton1.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectButton2 != null)
        {
            selectButton2.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectButton3 != null)
        {
            selectButton3.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation1 != null)
        {
            selectRotation1.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation2 != null)
        {
            selectRotation2.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation3 != null)
        {
            selectRotation3.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation4 != null)
        {
            selectRotation4.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation5 != null)
        {
            selectRotation5.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectRotation6 != null)
        {
            selectRotation6.onClick.AddListener(ActivateSelectionMode);
        }
        if (selectResetRotation != null)
        {
            selectResetRotation.onClick.AddListener(ActivateSelectionMode);
        }


        // Add listeners to all buttons to deactivate selection, except the select buttons
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            if (button != selectButton1 && button != selectButton2 && button != selectButton3 && button !=selectRotation1 && button != selectRotation2 && button != selectRotation3 &&  button != selectRotation4 && button != selectRotation5 && button != selectRotation6 &&  button != selectResetRotation)
            {
                button.onClick.AddListener(DeactivateSelection);
            }
        }
    }

    void Update()
    {
        if (isSelectionModeActive && Input.GetMouseButtonDown(0))
        {
            SelectObjectWithRay();
        }
    }

    void ActivateSelectionMode()
    {
        isSelectionModeActive = true;
        selectedObject.RemoveObject = false;
        Debug.Log("Selection mode activated.");
    }

    void DeactivateSelection()
    {
        if (SelectedObject != null && originalMaterial != null)
        {
            SelectedObject.GetComponent<Renderer>().material = originalMaterial;
            SelectedObject = null;
            originalMaterial = null;
        }
        isSelectionModeActive = false;
        Debug.Log("Selection mode deactivated.");
    }

    void SelectObjectWithRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Ray hit: " + hit.collider.name);

            if (hit.collider != null)
            {
                GameObject newSelectedObject = hit.collider.gameObject;

                // If a new object is selected, update the selection
                if (newSelectedObject != SelectedObject)
                {
                    // If there was a previously selected object, reset its material
                    if (SelectedObject != null && originalMaterial != null)
                    {
                        SelectedObject.GetComponent<Renderer>().material = originalMaterial;
                    }

                    // Assign the new selected object
                    SelectedObject = newSelectedObject;
                    selectedObject.selectedGameObject = SelectedObject;

                    // Store the original material
                    Renderer objRenderer = SelectedObject.GetComponent<Renderer>();
                    originalMaterial = objRenderer.material;

                    // Change the material of the selected object
                    objRenderer.material = selectedMaterial;

                    Debug.Log("Object selected: " + SelectedObject.name);
                }
            }
        }
        else
        {
            Debug.Log("Ray missed.");
        }
    }
}
