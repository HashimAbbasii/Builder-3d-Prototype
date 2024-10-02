using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class LineDistanceCreator : MonoBehaviour
{
	public bool onSurface = true;
	public bool verticalOrHorizontal = false;
	private Vector3 _hitPos;
	private Transform _lastHitTransform;
	private bool _isDrawLine = true;
	private bool _prevIsDrawLine = true;
	private readonly bool showSubAxis = false;

	private Transform _startPointx;
	private Transform _startPointz;
	private Transform _endPointx;
	private Transform _endPointz;
	public Vector3 hitPoint;
	public Vector3 spx;
	public Vector3 spz;
	public Vector3 epx;
	public Vector3 epz;
	
	
	private void Update()
	{
		if (_startPointx != null && _startPointz != null && _endPointx != null && _endPointz != null)
		{
			spx = _startPointx.position;
			spz = _startPointz.position;
			epx = _endPointx.position;
			epz = _endPointz.position;
		}
		
		//MeasureLine_WorldCanvas's verticalOrHorizontal
		MeasureLine_WorldCanvas.verticalOrHorizontal = verticalOrHorizontal;
		//Add Line
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			for (var i = 0; i < 4; i++)
			{
				_isDrawLine = true;
				if (_prevIsDrawLine != _isDrawLine)
				{
					_lastHitTransform = null;
					_prevIsDrawLine = _isDrawLine;
				}

				var hitObj = MouseRayer.GetMouseRayHit(Camera.main, out _hitPos);

				if (hitObj != null)
				{
					if (_isDrawLine)
					{
						if (!onSurface)
						{
							MeasureLine_WorldCanvas.DrawLine(hitObj.transform, false, false, onSurface, 6f);
							if (_lastHitTransform != null)
							{
								MeasureLine_WorldCanvas.EndDrawLine();
								_lastHitTransform = null;
							}
							else
							{
								_lastHitTransform = hitObj.transform;
							}
						}
						else
						{
							var hitDummy = new GameObject("SurfaceLineDummy")
							{
								transform =
								{
									position = _hitPos
								}
							};

							switch (i)
							{
								case 0:
									_startPointx = hitDummy.transform;
									break;
								case 1:
									_endPointx = hitDummy.transform;
									break;
								case 2:
									_startPointz = hitDummy.transform;
									break;
								case 3:
									_endPointz = hitDummy.transform;
									break;
							}
							
							hitDummy.transform.SetParent(hitObj.transform);
							MeasureLine_WorldCanvas.DrawLine(hitDummy.transform, false, false, onSurface, 6f,
								showSubAxis);
							if (_lastHitTransform != null)
							{
								MeasureLine_WorldCanvas.EndDrawLine();
								_lastHitTransform = null;
							}
							else
							{
								_lastHitTransform = hitObj.transform;
							}
						}
					}
				}
			}
		}

		if (Input.GetKey(KeyCode.Mouse0))
		{
			if (_endPointx != null && _endPointz != null)
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out var hit))
				{
					hitPoint = hit.point;
					_endPointx.position = new Vector3(hit.point.x, _startPointx.position.y, _startPointx.position.z);
					_endPointz.position = new Vector3(_startPointz.position.x, _startPointz.position.y , hit.point.z);
				}
			}
		}

		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			
		}
		
	}
}
