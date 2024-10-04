using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistanceHandler : MonoBehaviour
{
	public bool OnSurface = true;
	private Vector3 hitPos;
	private Transform lastHitTransform;
	private bool isDrawLine = true;
	private bool prevIsDrawLine = true;
	private bool showSubAxis = false;
	public bool verticalOrHorizontal = false;

	public List<MeasureLine_WorldCanvas> lineCanvasList = new();

	[ContextMenu("ListLines")]
	public void ListLinesFunction()
	{
		lineCanvasList.Clear();
		var mws = FindObjectsOfType<MeasureLine_WorldCanvas>();
	    
		foreach (var mw in mws)
		{
			lineCanvasList.Add(mw);
			// if (mw.gameObject.name == "SurfaceLineDummy")
			// {
			// 	Destroy(mw.gameObject);
			// }
			// else
			// {
			// 	Destroy(mw);
			// }
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
		
		//Del all SurfaceLineDummy empty object
		// var sld = GameObject.Find("SurfaceLineDummy");
	 //    
		// while (sld != null)
		// {
		// 	DestroyImmediate(sld);
		// 	sld = GameObject.Find("SurfaceLineDummy");
		// }

		//Del all childs of lineCollection
		// var lineCollection = GameObject.Find("lineCollection").transform;
		// var childObjects = new List<GameObject>();
		//
		// for (var i = 0; i < lineCollection.childCount; i++)
		// {
		// 	childObjects.Add(lineCollection.GetChild(i).gameObject);
		// }
		//
		// for (var i = 0; i < childObjects.Count; i++)
		// {
		// 	Destroy(childObjects[i]);
		// }
	}
	
	void Update()
	{
		//MeasureLine_WorldCanvas's verticalOrHorizontal
		MeasureLine_WorldCanvas.verticalOrHorizontal = verticalOrHorizontal;
		//Add Line
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			isDrawLine = true;
			if (prevIsDrawLine != isDrawLine)
			{
				lastHitTransform = null;
				prevIsDrawLine = isDrawLine;
			}

			var hitObj = MouseRayer.GetMouseRayHit(Camera.main, out hitPos);

			if (hitObj != null)
			{
				if (isDrawLine)
				{
					if (!OnSurface)
					{
						MeasureLine_WorldCanvas.DrawLine(hitObj.transform, false, false, OnSurface, 6f);
						if (lastHitTransform != null)
						{
							MeasureLine_WorldCanvas.EndDrawLine();
							lastHitTransform = null;
						}
						else
						{
							lastHitTransform = hitObj.transform;
						}
					}
					else
					{
						var hitDummy = new GameObject("SurfaceLineDummy")
						{
							transform =
							{
								position = hitObj.transform.position
							}
						};
						hitDummy.transform.SetParent(hitObj.transform);
						
						MeasureLine_WorldCanvas.DrawLine(hitDummy.transform, false, false, OnSurface, 6f, showSubAxis);
						
						
						
						if (lastHitTransform != null)
						{
							MeasureLine_WorldCanvas.EndDrawLine();
							lastHitTransform = null;
						}
						else
						{
							lastHitTransform = hitObj.transform;
						}
					}
				}
			}
		}
		//Delete Line
		else if (Input.GetKeyDown(KeyCode.Mouse1))
		{
			isDrawLine = false;
			if (prevIsDrawLine != isDrawLine)
			{
				lastHitTransform = null;
				prevIsDrawLine = isDrawLine;
			}

			var hitObj = MouseRayer.GetMouseRayHit(Camera.main, out hitPos);
			if (lastHitTransform != null)
			{
				if (lastHitTransform != hitObj.transform)
				{
					MeasureLine_WorldCanvas.DeleteLine(lastHitTransform, hitObj.transform, OnSurface);
					lastHitTransform = null;
				}
				else
				{
					MeasureLine_WorldCanvas.DeleteLine(hitObj.transform, OnSurface);
					lastHitTransform = null;
				}
			}
			else
			{
				lastHitTransform = hitObj.transform;
			}
		}
		//Delete All Lines
		else if (Input.GetKeyDown(KeyCode.Mouse2))
		{
			MeasureLine_WorldCanvas.DeleteAllLines();
			lastHitTransform = null;
		}
	}
}
