using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHandler : MonoBehaviour
{
   public bool RotationHandler=false;
    public bool ScalingEnabled=false;
    public bool RemoveObject=false;
    public Slider scaleSlider;
    public GameObject SelectedObjectForScaling;
    public FloorHander floorHandler;
    private Vector3 initialScale;


    [Header("Selected Object")]
    public GameObject selectedGameObject;
    public SelectObject selectedObjectbyUser;
    void Start()
    {
        floorHandler =floorHandler.GetComponent<FloorHander>();
        scaleSlider =scaleSlider.GetComponent<Slider>();
        scaleSlider.minValue = 0.5f;  // Set appropriate min value
        scaleSlider.maxValue = 2.0f; // Set appropriate max value
        scaleSlider.value = 1.0f; // Default value

        // Add listener for slider value change
        scaleSlider.onValueChanged.AddListener(OnSliderValueChanged);
        RotationHandler =false;
        ScalingEnabled=false;
        RemoveObject = false;
}

    // Update is called once per frame
    void Update()
    {
    //    if (RotationHandler && Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log("Rotate Mode");
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);        
    //        RaycastHit hit;
    //        if(Physics.Raycast(ray, out hit))
    //        {
    //            if (hit.collider != null)
    //            {
    //                SelectedObjectForScaling=hit.collider.gameObject;
    //                RotatingObject(hit.collider.gameObject);
    //               // Exit rotation mode after rotating the object
    //            }
    //        }
            
    //    }

        //if (ScalingEnabled && Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("Rotate Mode");
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        if (hit.collider != null)
        //        {
        //            ScaleObject(hit.collider.gameObject);
        //            // Exit rotation mode after rotating the object
        //        }
        //    }

        //}


        if (RemoveObject && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Remove Object");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject selectedObject = hit.collider.gameObject;
                if (selectedObject != null)
                {
                   Destroy(selectedObject);
                    // Exit rotation mode after rotating the object
                }
            }

        }

    }

    #region RemoveObject
    public void RemoveObjectFromScene()
    {
        RemoveObject=true;
        RotationHandler = false;
        ScalingEnabled = false;

    }






    #endregion




    #region _RotatingMethod

    public void RotateObject(float rot)
    {
        try
        {
            // Check if the selectedGameObject is null
            if (selectedGameObject == null)
            {
                throw new NullReferenceException("selectedGameObject is null.");
            }

            // Update the selectedGameObject from selectedObjectbyUser
            selectedGameObject = selectedObjectbyUser.SelectedObject;

            // Perform the rotation on the selectedGameObject
            selectedGameObject.transform.Rotate(0, rot, 0);

            // Additional operations
            RemoveObject = false;
            // RotationHandler = true; // Uncomment if needed
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("An unexpected error occurred: " + ex.Message);
        }
    }

    public void RotateObject()
    {
        try
        {
            // Check if the selectedGameObject is null
            if (selectedGameObject == null)
            {
                throw new NullReferenceException("selectedGameObject is null.");
            }

            // Update the selectedGameObject from selectedObjectbyUser
            selectedGameObject = selectedObjectbyUser.SelectedObject;

            // Perform the rotation on the selectedGameObject
            selectedGameObject.transform.Rotate(0, 45, 0);

            // Additional operations
            RemoveObject = false;
            // RotationHandler = true; // Uncomment if needed
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("An unexpected error occurred: " + ex.Message);
        }
    }
    
    public void ResetRotation()
    {
        try
        {
            // Check if the selectedGameObject is null
            if (selectedGameObject == null)
            {
                throw new NullReferenceException("selectedGameObject is null.");
            }

            // Update the selectedGameObject from selectedObjectbyUser
            selectedGameObject = selectedObjectbyUser.SelectedObject;

            // Reset the rotation of the selectedGameObject to (0, 0, 0)
            selectedGameObject.transform.rotation = Quaternion.identity;

            // Additional operations
            RemoveObject = false;
            // RotationHandler = true; // Uncomment if needed
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("An unexpected error occurred: " + ex.Message);
        }
    }
    public void DisableRotationMode()
    {
        RotationHandler = false;
        RemoveObject = false;
        Debug.Log("Rotation Mode Disabled");
    }

    #endregion


    #region _ScaleObject

    public void StartScaling()
    {
        if (selectedGameObject == null)
        {
            Debug.Log("Empty gameObject");
        }
        selectedGameObject = selectedObjectbyUser.SelectedObject;
      //  selectedGameObject = floorHandler.instantiatedFloor; // Assign the object to be scaled
        if (selectedGameObject != null)
        {
            initialScale = selectedGameObject.transform.localScale; // Store its initial scale
            ScalingEnabled = true;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        if (selectedGameObject != null && ScalingEnabled)
        {
            RotationHandler = false;

            // Apply the slider value directly to the object's scale
            selectedGameObject.transform.localScale = new Vector3(value, 1, value);

            //Debug.Log("NEW SCALE: " + SelectedObjectForScaling.transform.localScale);
        }
    }
    public void DisableScaleMode()
    {
        ScalingEnabled = false;
        Debug.Log("Rotation Mode Disabled");
    }

    #endregion _ScaleObject




}
