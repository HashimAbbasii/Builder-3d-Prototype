using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutOfBound : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("OnMouseDown If");
        }
        else
        {
            Debug.Log("OnMouseDown Else");
        }
    }

    private void OnMouseUp()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("OnMouseUp If");
        }
        else
        {
            Debug.Log("OnMouseUp Else");
        }
    }
    
}
