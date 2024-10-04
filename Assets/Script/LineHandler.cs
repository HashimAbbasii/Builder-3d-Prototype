using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LineHandler : MonoBehaviour
{
    public GameObject relatedObject;
    public LineRenderer lineRenderer;
    public RectTransform worldCanvas;
    
    public TMP_Text distanceText;

    private void Update()
    {
        if (lineRenderer == null || worldCanvas == null) return;

        // Calculate the distance between the two points of the line
        var distance = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));

        // Remap the distance to a scale value between 0.2 and 0.3
        var rmv = ExtraStaticFunctions.RemapValue(distance, 0, 3, 0.2f, 0.3f);

        // Set the scale of the world canvas based on the remapped distance
        worldCanvas.localScale = new Vector3(rmv, rmv, rmv);

        // Calculate the midpoint of the line
        var wcx = lineRenderer.GetPosition(0).x + (lineRenderer.GetPosition(1).x - lineRenderer.GetPosition(0).x) / 2;
        var wcy = lineRenderer.GetPosition(0).y + (lineRenderer.GetPosition(1).y - lineRenderer.GetPosition(0).y) / 2;
        var wcz = lineRenderer.GetPosition(0).z + (lineRenderer.GetPosition(1).z - lineRenderer.GetPosition(0).z) / 2;

        // Set the position of the world canvas to the midpoint of the line
        worldCanvas.anchoredPosition3D = new Vector3(wcx, wcy, wcz);

        // // If the line is almost horizontal, rotate the world canvas to match
        // if (Math.Abs(lineRenderer.GetPosition(0).x - lineRenderer.GetPosition(1).x) < 0.01f)
        // {
        //     // var z = distance / 2;
        //     // worldCanvas.rotation = Quaternion.Euler(90,0,0);
        //     // if (lineRenderer.GetPosition(0).z >= lineRenderer.GetPosition(1).z)
        //     // {
        //     //     worldCanvas.anchoredPosition3D = new Vector3(0, 1, -z);
        //     // }
        //     // else
        //     // {
        //     //     worldCanvas.anchoredPosition3D = new Vector3(0, 1, z);
        //     // }
        // }
        // // If the line is almost vertical, rotate the world canvas to match
        // else if (Math.Abs(lineRenderer.GetPosition(0).z - lineRenderer.GetPosition(1).z) < 0.01f)
        // {
        //     // var x = distance / 2;
        //     //
        //     // if (lineRenderer.GetPosition(0).x >= lineRenderer.GetPosition(1).x)
        //     // {
        //     //     worldCanvas.rotation = Quaternion.Euler(90,-90,0);
        //     //     worldCanvas.anchoredPosition3D = new Vector3(-x, 1, 0);
        //     // }
        //     // else
        //     // {
        //     //     worldCanvas.rotation = Quaternion.Euler(90,90,0);
        //     //     worldCanvas.anchoredPosition3D = new Vector3(x, 1, 0);
        //     // }
        // }
    }
}