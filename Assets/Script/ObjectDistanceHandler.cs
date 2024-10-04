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

	void Update () 
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

			Debug.Log("ODH I");
			
			if (hitObj != null)
			{
				Debug.Log("ODH II");
				
				if (isDrawLine)
				{
					Debug.Log("ODH III");
					if (!OnSurface)
					{
						MeasureLine_WorldCanvas.DrawLine(hitObj.transform, false, false, OnSurface, 6f);
						if (lastHitTransform != null) {
							MeasureLine_WorldCanvas.EndDrawLine ();
							lastHitTransform = null;
						} else {
							lastHitTransform = hitObj.transform;
						}
					}
					else
					{
						Debug.Log("ODH IIIE");
						
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
							MeasureLine_WorldCanvas.EndDrawLine ();
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
			GameObject hitObj = MouseRayer.GetMouseRayHit(Camera.main, out hitPos);
			if (lastHitTransform != null) {
				if (lastHitTransform != hitObj.transform) {
					MeasureLine_WorldCanvas.DeleteLine(lastHitTransform, hitObj.transform, OnSurface);
					lastHitTransform = null;
				}
				else
				{
					MeasureLine_WorldCanvas.DeleteLine(hitObj.transform, OnSurface);
					lastHitTransform = null;
				}
			} else {
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
