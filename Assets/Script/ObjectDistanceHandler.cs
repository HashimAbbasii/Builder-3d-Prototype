using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistanceHandler : MonoBehaviour
{
	public bool verticalOrHorizontal = false;
	public LayerMask layerMask;
	public List<MeasureLine_WorldCanvas> lineCanvasList = new();
	
	private Vector3 hitPos;
	private Transform lastHitTransform;
	private bool isDrawLine = true;
	private bool prevIsDrawLine = true;
	private bool showSubAxis = false;


	[ContextMenu("ListLines")]
	public void ListLinesFunction()
	{
		lineCanvasList.Clear();
		var mws = FindObjectsOfType<MeasureLine_WorldCanvas>();
	    
		foreach (var mw in mws)
		{
			lineCanvasList.Add(mw);
		}

		foreach (var mw in lineCanvasList)
		{
			mw.lineWidth = 0.05f;
			mw.gameObject.name = "LineStartPoint";
			foreach (var to in mw.targetObjects)
			{
				to.gameObject.name = "LineEndPoint";
			}

			mw.lineRender.transform.parent = transform;
			mw.linkTexts[0].transform.parent.parent = transform;
		}
	}
	
	private void Update()
	{
		//MeasureLine_WorldCanvas's verticalOrHorizontal
		MeasureLine_WorldCanvas.verticalOrHorizontal = verticalOrHorizontal;
		//Add Line
		if (!Input.GetKeyDown(KeyCode.Mouse0)) return;
		
		isDrawLine = true;
		if (prevIsDrawLine != isDrawLine)
		{
			lastHitTransform = null;
			prevIsDrawLine = isDrawLine;
		}

		var hitObj = MouseRayer.GetMouseRayHit(Camera.main, out hitPos, layerMask);

		if (hitObj == null) return;

		if (!isDrawLine) return;

		var hitDummy = new GameObject("SurfaceLineDummy")
		{
			transform =
			{
				position = hitObj.transform.position
			}
		};
		hitDummy.transform.SetParent(hitObj.transform);
						
		MeasureLine_WorldCanvas.DrawLine(hitDummy.transform, false, false, true, 6f, showSubAxis);
						
		if (lastHitTransform != null)
		{
			MeasureLine_WorldCanvas.EndDrawLine();
			Invoke(nameof(ListLinesFunction),0.1f);
			lastHitTransform = null;
		}
		else
		{
			lastHitTransform = hitObj.transform;
		}
	}

	public void DeleteAllLines()
	{
		MeasureLine_WorldCanvas.DeleteAllLines();
		lastHitTransform = null;
	}
}
