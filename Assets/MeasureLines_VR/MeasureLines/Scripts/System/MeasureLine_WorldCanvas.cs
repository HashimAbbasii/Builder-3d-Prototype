﻿//Script by unicoea 2017.4.22
//v1.2.2b Version:
//Use World_UICanvas for render the text (Support VR)

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
//using UnityEditor; //Comment this for publish.

public class MeasureLine_WorldCanvas : MonoBehaviour {

    public enum Coordinate
    {
        Local,
        World,
    }

    public enum MetricOrImperial
    {
        Inches,
        Foot,
        Meter,
		Km,
    }

    public class LinkedObjects
    {
        public GameObject obj0;
        public GameObject obj1;
    }

    private Transform myTransform;
    private RaycastHit hit_Right0 = new RaycastHit();
    private RaycastHit hit_Right1 = new RaycastHit();
    private RaycastHit hit_Left0 = new RaycastHit();
    private RaycastHit hit_Left1 = new RaycastHit();
    private RaycastHit hit_Up0 = new RaycastHit();
    private RaycastHit hit_Up1 = new RaycastHit();
    private RaycastHit hit_Down0 = new RaycastHit();
    private RaycastHit hit_Down1 = new RaycastHit();
    private RaycastHit hit_Forward0 = new RaycastHit();
    private RaycastHit hit_Forward1 = new RaycastHit();
    private RaycastHit hit_Back0 = new RaycastHit();
    private RaycastHit hit_Back1 = new RaycastHit();
    private Transform line_L;
    private Transform line_R;
    private Transform line_U;
    private Transform line_D;
    private Transform line_F;
    private Transform line_B;
    private Vector3 screenL;
    private Vector3 screenR;
    private Vector3 screenU;
    private Vector3 screenD;
    private Vector3 screenF;
    private Vector3 screenB;
    private Vector3 midPoint;
    private float lengthL;
    private float lengthR;
    private float lengthU;
    private float lengthD;
    private float lengthF;
    private float lengthB;
    private bool showL;
    private bool showR;
    private bool showU;
    private bool showD;
    private bool showF;
    private bool showB;
    private Material mat;
    public Camera mainCamera;
    private Transform camTransform;
    public float lineWidth = 0.15f;
    // public Color lineColor = new Color(0.627f, 0.839f, 0.165f, 1);
    public Color lineColor = new Color(1f, 0f, 0f, 1);
	// static public Color StaticLineColor = new Color(0.627f, 0.839f, 0.165f, 1); 
	static public Color StaticLineColor = new Color(1f, 0f, 0f, 1); 
    public Color textColor = new Color(0, 0, 0, 1);
	static public Color StaticTextColor = new Color(0, 0, 0, 1);
    public int textSize = 100;
    public float canvasScale = 0.1f;
    public int decimalPlaces = 2;
    public MetricOrImperial metricOrImperial = MetricOrImperial.Foot;
    static public MetricOrImperial StaticMetricOrImperial = MetricOrImperial.Foot;
    public bool InitSizeForVR = false;
    public float lineWidthVR = 0.015f;
    public int textSizeVR = 100;
	static public int StaticTextSizeVR = 100;
    public float canvasScaleVR = 0.1f;
    public float splitDistanceForText = 0f;
    public float trackIntervalForText = 0.01f;
    public Vector3 pivotBias = Vector3.zero;
    public Vector3 angle = Vector3.zero;
    public Coordinate coordinate = Coordinate.Local;
    public bool AxisX_P = true;
    public bool AxisX_N = true;
    public bool AxisY_P = true;
    public bool AxisY_N = true;
    public bool AxisZ_P = true;
    public bool AxisZ_N = true;
    public Transform lineParent;
    public float maxDistance = 100f;
    public LayerMask layerMask = -1;
    private Transform axisDummy = null;
    private bool isEmptyObject = false;
    //	private float textBiasX = 10f;
    public bool canBeBlock = true;
	public bool ShowTargetLine = true;
    public bool IsShowText = true;
    public Transform[] targetObjects;
	private List<Transform> targetList = new List<Transform> ();
	private Transform[] targets;
	private Transform[] targetLines;
//	private Vector3[] targetScreen;
	private float[] targetLength;
	private bool[] showTarget;
	private Vector3[] nearPoint_Links;
	private RaycastHit hit_Target0;
	private RaycastHit hit_Target1;
	public Renderer lineRender = null;
	private int layer = 0;
	private int lastTargetLineCount = 0;
	private GameObject lineCollection;
	private GameObject wCanvas_L;
	private GameObject wCanvas_R;
	private GameObject wCanvas_U;
	private GameObject wCanvas_D;
	private GameObject wCanvas_F;
	private GameObject wCanvas_B;
	private Text uiText_L;
	private Text uiText_R;
	private Text uiText_U;
	private Text uiText_D;
	private Text uiText_F;
	private Text uiText_B;
	public List<Text> linkTexts = new List<Text>();
	private Vector3 nearPoint_L;
	private Vector3 nearPoint_R;
	private Vector3 nearPoint_U;
	private Vector3 nearPoint_D;
	private Vector3 nearPoint_F;
	private Vector3 nearPoint_B;
	private float t = 0;

    //Comment this function for publish.
    //	void OnDrawGizmos() 
    //	{
    //		//Show Label
    //		if (AxisX_P && line_R!=null && showR && InFrontOfCamera(line_R.transform.position)) {
    //			Handles.Label (line_R.transform.position, (Mathf.Round (lengthR * 100) / 100).ToString () + "m");
    //		}
    //		if (AxisX_N && line_L != null && showL && InFrontOfCamera(line_L.transform.position)) {
    //			Handles.Label (line_L.transform.position, (Mathf.Round (lengthL * 100) / 100).ToString () + "m");
    //		}
    //		if (AxisY_P && line_U != null && showU && InFrontOfCamera(line_U.transform.position)) {
    //			Handles.Label (line_U.transform.position, (Mathf.Round (lengthU * 100) / 100).ToString () + "m");
    //		}
    //		if (AxisY_N && line_D!=null && showD && InFrontOfCamera(line_D.transform.position)) {
    //			Handles.Label (line_D.transform.position, (Mathf.Round (lengthD * 100) / 100).ToString () + "m");
    //		}
    //		if (AxisZ_P && line_F != null && showF && InFrontOfCamera(line_F.transform.position)) {
    //			Handles.Label (line_F.transform.position, (Mathf.Round (lengthF * 100) / 100).ToString () + "m");
    //		}
    //		if (AxisZ_N && line_B!=null && showB && InFrontOfCamera(line_B.transform.position)) {
    //			Handles.Label (line_B.transform.position, (Mathf.Round (lengthB * 100) / 100).ToString () + "m");
    //		}
    //		//Get Really Targets
    //		GetReallyTarget ();
    //		//Refresh Target Lines
    //		if (lastTargetLineCount != targets.Length) {
    //			RefreshTargetLines();
    //			lastTargetLineCount = targets.Length;
    //		}
    //		if (targets.Length > 0 && targetLines!=null && targetLines.Length>0) {
    //			for (int i=0;i<targetLines.Length;i++){
    //				if (ShowTargetLine && showTarget[i] && targetLines[i]!=null && InFrontOfCamera(targetLines[i].transform.position)){
    //					Handles.Label (targetLines[i].transform.position, (Mathf.Round (targetLength[i] * 100) / 100).ToString () + "m");
    //				}
    //			}
    //		}
    //		if (isEmptyObject)
    //			Gizmos.DrawWireSphere (myTransform.position, 0.5f);
    //	}
    
    
    /// <summary>
    /// Clear all lines
    /// </summary>
    static public void DeleteAllLines()
    {
	    //Del all MeasureLine_WorldCanvas
	    var mws = FindObjectsOfType<MeasureLine_WorldCanvas>();
	    
	    for (var i = 0; i < mws.Length; i++)
	    {
		    MeasureLine_WorldCanvas mw = mws[i];
		    if (mw.gameObject.name == "SurfaceLineDummy")
		    {
			    Destroy(mw.gameObject);
		    }
		    // else
		    // {
			   //  Destroy(mw);
		    // }
	    }

	    //Del all SurfaceLineDummy empty object
	    var sld = GameObject.Find("SurfaceLineDummy");
	    
	    while (sld != null)
	    {
		    DestroyImmediate(sld);
		    sld = GameObject.Find("SurfaceLineDummy");
	    }
	    
	     //Del all childs of lineCollection
	     if (GameObject.Find("lineCollection") == null) return; 
	     
	     var lineCollection = GameObject.Find("lineCollection").transform;
	     var childObjects = new List<GameObject>();
	    
	     for (var i = 0; i < lineCollection.childCount; i++)
	     {
		     childObjects.Add(lineCollection.GetChild(i).gameObject);
	     }
	    
	     for (var i = 0; i < childObjects.Count; i++)
	     {
		     Destroy(childObjects[i]);
	     }
	    
	     childObjects.Clear();
	    // Clear SurfaceLink
	     surfaceLinks.Clear();
    }


    /// <summary>
	/// For Dynamic Draw Line Function
	/// </summary>
	static public void AddLine(Transform obj1, Transform obj2, bool InitSizeForVR, bool canBeBlock, bool onSurface, float splitDistanceForText = 0, bool ignoreSurfaceLinks = false)
	{
		MeasureLine_WorldCanvas mw1 = obj1.GetComponent<MeasureLine_WorldCanvas>();
		MeasureLine_WorldCanvas mw2 = obj2.GetComponent<MeasureLine_WorldCanvas>();
		MeasureLine_WorldCanvas mw = null;
		if (mw1 == null && mw2 == null){
			mw = null;
		}else if (mw1 != null && mw2 == null){
			mw = mw1;
		}else if (mw1 == null && mw2 != null){
			mw = mw2;
		}else if (mw1 != null && mw2 != null){
			if (IsExistTarget(mw1, obj2) || IsExistTarget(mw2, obj1)){
				return;
			}else{
				mw = mw1;
			}
		}
		if (mw == null) 
		{
			mw = obj1.gameObject.AddComponent<MeasureLine_WorldCanvas> ();
			mw.InitSizeForVR = InitSizeForVR;
			mw.splitDistanceForText = splitDistanceForText;
			mw.canBeBlock = canBeBlock;
			mw.AxisX_P = false;
			mw.AxisX_N = false;
			mw.AxisY_N = false;
			mw.AxisY_P = false;
			mw.AxisZ_N = false;
			mw.AxisZ_P = false;
		}
		AddTarget(mw, obj2);
		//Add to surfaceLinks
		if (onSurface && !ignoreSurfaceLinks){
			AddSurfaceLinks(obj1.gameObject, obj2.gameObject);
		}
	}

	//Delete Line in one object
	static public void DeleteLine(Transform obj1, bool onSurface)
	{
		DeleteLine(obj1, obj1, onSurface);
	}

	//Delete line between two objects
	static public void DeleteLine(Transform obj1, Transform obj2, bool onSurface)
	{
		//Delete surfaceLinks
		if (onSurface){
//			Debug.Log("DelSurfaceLinks");
			DelSurfaceLinks(obj1.gameObject, obj2.gameObject);
		}
		else
		{
			MeasureLine_WorldCanvas mw1 = obj1.GetComponent<MeasureLine_WorldCanvas>();
			MeasureLine_WorldCanvas mw2 = obj2.GetComponent<MeasureLine_WorldCanvas>();
			DelTarget(mw1, obj2);
			DelTarget(mw2, obj1);
		}
	}

	static public Dictionary<string, List<LinkedObjects>> surfaceLinks = new Dictionary<string, List<LinkedObjects>>();

	static public void DelSurfaceLinks(GameObject object0, GameObject object1)
	{
		string id_0_1 = object0.GetInstanceID().ToString() + "_" + object1.GetInstanceID().ToString();
		string id_1_0 = object1.GetInstanceID().ToString() + "_" + object0.GetInstanceID().ToString();
//		Debug.Log("DelSurfaceLinks->id_0_1 = "+id_0_1+" id_1_0 = "+id_1_0);
		if (surfaceLinks.ContainsKey(id_0_1))
		{
			List<LinkedObjects> list = surfaceLinks[id_0_1];
			List<LinkedObjects> newList = new List<LinkedObjects>();
			List<int> removeIndexList = new List<int>();
			GameObject obj0;
			GameObject obj1;
			for(int i=0;i<list.Count;i++)
			{
				obj0 = list[i].obj0;
				obj1 = list[i].obj1;
				if ((obj0.transform.parent == object0.transform && obj1.transform.parent == object1.transform) || (obj0.transform.parent == object1.transform && obj0.transform.parent == object1.transform))
				{
					//Delete in Dummy's targetObjects's list
					MeasureLine_WorldCanvas mw0 = obj0.GetComponent<MeasureLine_WorldCanvas>();
					MeasureLine_WorldCanvas mw1 = obj1.GetComponent<MeasureLine_WorldCanvas>();
					DelTarget(mw0, obj1.transform);
					DelTarget(mw1, obj0.transform);
					//Delete no measureline_worldcanvas component's one's gameobject.
					if (mw0!=null && mw1==null)
					{
						Destroy(obj1);
						if (MeasureLineHelper.IsEmpty(mw0.targetObjects))
						{
							Destroy(obj0);
						}
						removeIndexList.Add(i); //ready to remove from list, save index first
					}
					else if (mw1!=null && mw0==null)
					{
						Destroy(obj0);
						if (MeasureLineHelper.IsEmpty(mw1.targetObjects))
						{
							Destroy(obj1);
						}
						removeIndexList.Add(i); //ready to remove from list, save index first
					}
				}
			}
			//Build new list if needed
			if (removeIndexList.Count>0)
			{
				for (int i=0;i<list.Count;i++)
				{
					if (!MeasureLineHelper.CheckIntInList(removeIndexList, i))
					{
						newList.Add(list[i]);
					}
				}
				surfaceLinks[id_0_1] = newList;
				removeIndexList.Clear();
				list.Clear();
			}
		}
		else if (surfaceLinks.ContainsKey(id_1_0))
		{
			List<LinkedObjects> list = surfaceLinks[id_1_0];
			List<LinkedObjects> newList = new List<LinkedObjects>();
			List<int> removeIndexList = new List<int>();
			GameObject obj0;
			GameObject obj1;
			for(int i=0;i<list.Count;i++)
			{
				obj0 = list[i].obj0;
				obj1 = list[i].obj1;
				if ((obj0.transform.parent == object0.transform && obj1.transform.parent == object1.transform) || (obj0.transform.parent == object1.transform && obj0.transform.parent == object1.transform))
				{
					//Delete in Dummy's targetObjects's list
					MeasureLine_WorldCanvas mw0 = obj0.GetComponent<MeasureLine_WorldCanvas>();
					MeasureLine_WorldCanvas mw1 = obj1.GetComponent<MeasureLine_WorldCanvas>();
					DelTarget(mw0, obj1.transform);
					DelTarget(mw1, obj0.transform);
					//Delete no measureline_worldcanvas component's one's gameobject.
					if (mw0!=null && mw1==null)
					{
						Destroy(obj1);
						if (MeasureLineHelper.IsEmpty(mw0.targetObjects))
						{
							Destroy(obj0);
						}
						removeIndexList.Add(i); //ready to remove from list, save index first
					}
					else if (mw1!=null && mw0==null)
					{
						Destroy(obj0);
						if (MeasureLineHelper.IsEmpty(mw1.targetObjects))
						{
							Destroy(obj1);
						}
						removeIndexList.Add(i); //ready to remove from list, save index first
					}
				}
			}
			//Build new list if needed
			if (removeIndexList.Count>0)
			{
				for (int i=0;i<list.Count;i++)
				{
					if (!MeasureLineHelper.CheckIntInList(removeIndexList, i))
					{
						newList.Add(list[i]);
					}
				}
				surfaceLinks[id_1_0] = newList;
				removeIndexList.Clear();
				list.Clear();
			}
		}
	}

	static public void AddSurfaceLinks(GameObject child0, GameObject child1)
	{
		LinkedObjects item = new LinkedObjects();
		item.obj0 = child0;
		item.obj1 = child1;
		string id_0_1 = child0.transform.parent.gameObject.GetInstanceID().ToString() + "_" + child1.transform.parent.gameObject.GetInstanceID().ToString();
		string id_1_0 = child1.transform.parent.gameObject.GetInstanceID().ToString() + "_" + child0.transform.parent.gameObject.GetInstanceID().ToString();
		if (surfaceLinks.ContainsKey(id_0_1))
		{
			surfaceLinks[id_0_1].Add(item);
			//			Debug.Log("id = "+id_0_1 + " list's count = "+surfaceLinks[id_0_1].Count);
		}
		else if (surfaceLinks.ContainsKey(id_1_0))
		{
			surfaceLinks[id_1_0].Add(item);
			//			Debug.Log("id = "+id_1_0 + " list's count = "+surfaceLinks[id_1_0].Count);
		}
		else
		{
			List<LinkedObjects> list = new List<LinkedObjects>();
			list.Add(item);
			surfaceLinks.Add(id_0_1, list); //To save memory, not add the:surfaceLinks.Add(id_1_0, list), can add too.
			//			Debug.Log("id = "+id_0_1 + " list's count = "+list.Count);
		}
	}

	static public Transform staticTarget = null;
    static public bool verticalOrHorizontal = false;
    static public void DrawLine(Transform newTarget, bool InitSizeForVR, bool canBeBlock, bool onSurface, float splitDistanceForText = 0, bool threeAxis = false)
	{
//		//if click first object then click it again, then onsurface dummy will be delete.
//		if (staticTarget!=null && onSurface && staticTarget.parent == newTarget.parent)
//			Destroy(newTarget.gameObject);
		//Start
//		if (staticTarget==null || (!onSurface && staticTarget!=newTarget) || (onSurface && staticTarget.parent != newTarget.parent) )
		if (staticTarget==null || (!onSurface && staticTarget!=newTarget) || (onSurface) )
		{
			//DrawLine Target Point
			if (staticTarget!=null)
			{
				if (!threeAxis)
				{
                    if (verticalOrHorizontal)
                    {
                        Vector3 vec = newTarget.position - staticTarget.position;
                        //Horizontal Modify
                        if (Mathf.Abs(vec.y) < Mathf.Abs(vec.x) || Mathf.Abs(vec.y) < Mathf.Abs(vec.z))
                        {
                            newTarget.position = new Vector3(newTarget.position.x, staticTarget.position.y, newTarget.position.z);
                        }
                        //Vertical Modify
                        else if (Mathf.Abs(vec.y) > Mathf.Abs(vec.x) && Mathf.Abs(vec.y) > Mathf.Abs(vec.z))
                        {
                            newTarget.position = new Vector3(staticTarget.position.x, newTarget.position.y, staticTarget.position.z);
                        }
                    }
                    AddLine(staticTarget, newTarget, InitSizeForVR, canBeBlock, onSurface, splitDistanceForText);
				}
				else
				{
					//Add Three Axis Sub Lines
					AddLine(staticTarget, newTarget, InitSizeForVR, canBeBlock, onSurface, splitDistanceForText);
					GameObject axisXZObj0 = null;
					GameObject axisXYObj0 = null;
					GameObject axisXZObj1 = null;
					GameObject axisXYObj1 = null;
					GameObject lowerObj = null;
					GameObject higherObj = null;
					bool leftIsLower = true;
					MeasureLineHelper.CreateThreeAxisObj(staticTarget, newTarget, out axisXZObj0, out axisXYObj0, out axisXZObj1, out axisXYObj1, out lowerObj, out higherObj);
					AddLine(higherObj.transform, axisXZObj0.transform, InitSizeForVR, canBeBlock, onSurface, splitDistanceForText, true); //ignoreSurfaceLinks
					AddLine(axisXZObj1.transform, axisXYObj1.transform, InitSizeForVR, canBeBlock, onSurface, splitDistanceForText, true); //ignoreSurfaceLinks
					AddLine(lowerObj.transform, axisXYObj0.transform, InitSizeForVR, canBeBlock, onSurface, splitDistanceForText, true); //ignoreSurfaceLinks
				}
			}
			//Start DrawLine Start Point
			else
			{
				if (newTarget.gameObject.GetComponent<MeasureLine_WorldCanvas> () == null) 
				{
					MeasureLine_WorldCanvas mw = newTarget.gameObject.AddComponent<MeasureLine_WorldCanvas> ();
					mw.InitSizeForVR = InitSizeForVR;
					mw.splitDistanceForText = splitDistanceForText;
					mw.canBeBlock = canBeBlock;
					mw.AxisX_P = false;
					mw.AxisX_N = false;
					mw.AxisY_N = false;
					mw.AxisY_P = false;
					mw.AxisZ_N = false;
					mw.AxisZ_P = false;
				}
				staticTarget = newTarget;
			}
		}
	}
    
	static public void EndDrawLine()
	{
		staticTarget = null;
	}

	static private bool IsExistTarget(MeasureLine_WorldCanvas mw, Transform obj)
	{
		bool result = false;
		Transform[] targetObjs = mw.targetObjects;
		if (targetObjs == null) {
			result = false;
		}
		else 
		{
			List<Transform> lists = new List<Transform> ();
			for (int i = 0; i < targetObjs.Length; i++) {
				if (targetObjs[i]==obj)
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	static private void AddTarget(MeasureLine_WorldCanvas mw, Transform obj)
	{
		Transform[] targetObjs = mw.targetObjects;
        obj.gameObject.AddComponent<EndPointReference>();
        obj.gameObject.GetComponent<EndPointReference>().startPoint = mw;
        if (targetObjs == null) {
			targetObjs = new Transform[]{ obj };
		}
		else 
		{
			List<Transform> lists = new List<Transform> ();
			for (int i = 0; i < targetObjs.Length; i++) {
				if (targetObjs[i]!=obj)
				{
					lists.Add (targetObjs[i]);
				}
			}
			lists.Add (obj);
			targetObjs = lists.ToArray ();
		}
		mw.targetObjects = targetObjs;
	}

	static private void DelTarget(MeasureLine_WorldCanvas mw, Transform target)
	{
		if (mw!=null)
		{
			Transform[] targetObjs = mw.targetObjects;
			if (targetObjs!=null && targetObjs.Length>0)
			{
				List<Transform> objs = new List<Transform>();
				for (int i=0;i<targetObjs.Length;i++)
				{
					if (targetObjs[i]!=null)
						objs.Add(targetObjs[i]);
				}
				objs.Remove(target);
				targetObjs = objs.ToArray();
			}
			mw.targetObjects = targetObjs;
		}
	}


	/// <summary>
	/// For Attach to gameObject use
	/// </summary>
	void GetLineCollection()
	{
		lineCollection = GameObject.Find ("lineCollection");
		if (lineCollection == null) {
			lineCollection = new GameObject ("lineCollection");
//			lineCollection.hideFlags = HideFlags.HideInHierarchy;
		}
		lineParent = lineCollection.transform;
	}

	void Awake()
	{
		myTransform = transform;
		GetLineCollection ();
		axisDummy = new GameObject("AxisDummy").transform;
		if (coordinate == Coordinate.Local) {
			axisDummy.rotation = myTransform.rotation;
		}
		else {
			axisDummy.rotation = Quaternion.identity;
		}
		axisDummy.SetParent (myTransform);
	}

	void Start () 
	{
		if (mainCamera == null) {
			mainCamera = Camera.main;
		}
		camTransform = mainCamera.transform;
		layer = LayerMask.NameToLayer ("Ignore Raycast");
		mat = Resources.Load("Measurelines/Materials/MeasureLinesMat", typeof(Material)) as Material;
		//mat = new Material (Shader.Find ("Unlit/ColorML"));
		mat.color = lineColor;

		line_R = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_R = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_R.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_R.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		Destroy (line_R.GetComponent<Collider>());
		showR = false;
		line_R.gameObject.layer = layer;
		line_R.name = "line_R";
		line_R.parent = lineParent;

		line_L = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_L = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_L.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_L.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		Destroy (line_L.GetComponent<Collider>());
		showL = false;
		line_L.gameObject.layer = layer;
		line_L.name = "line_L";
		line_L.parent = lineParent;

		line_U = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_U = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_U.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_U.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		Destroy (line_U.GetComponent<Collider>());
		showU = false;
		line_U.gameObject.layer = layer;
		line_U.name = "line_U";
		line_U.parent = lineParent;

		line_D = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_D = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_D.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_D.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		Destroy (line_D.GetComponent<Collider>());
		showD = false;
		line_D.gameObject.layer = layer;
		line_D.name = "line_D";
		line_D.parent = lineParent;

		line_F = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_F = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_F.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_F.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		Destroy (line_F.GetComponent<Collider>());
		showF = false;
		line_F.gameObject.layer = layer;
		line_F.name = "line_F";
		line_F.parent = lineParent;

		line_B = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
		//line_B = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
		line_B.localScale = new Vector3 (lineWidth, lineWidth, 1);
		lineRender = line_B.GetComponent<Renderer> ();
		lineRender.sharedMaterial = mat;
		lineRender.enabled = false;
		Destroy (line_B.GetComponent<Collider>());
		lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lineRender.receiveShadows = false;
		showB = false;
		line_B.gameObject.layer = layer;
		line_B.name = "line_B";
		line_B.parent = lineParent;

		//Get Really Targets
		GetReallyTarget ();
		//TargetLines
		lastTargetLineCount = targets.Length;
		RefreshTargetLines ();

		//Angle Rotate and Pivot Bias
		if (coordinate == Coordinate.Local) {
			axisDummy.localEulerAngles = angle;
		}
		else {
			axisDummy.eulerAngles = angle;
		}
		axisDummy.localPosition = pivotBias;
		//Is Empty GameObject?
		isEmptyObject = (GetComponentInChildren<MeshRenderer> () == null && GetComponentInChildren<SkinnedMeshRenderer> () == null);
		//Wait For UICanvas
		CreateWorldCanvas();
		//DecimalPlaces
		if (decimalPlaces < 0)
			decimalPlaces = 0;
		else if (decimalPlaces > 8)
			decimalPlaces = 8;
		//Text and Canvas Size for VR
		if (InitSizeForVR)
		{
			lineWidth = lineWidthVR;
			canvasScale = canvasScaleVR;
			textSize = textSizeVR;
		}
		//Init t
		t = trackIntervalForText + 1;
	}

	void CreateWorldCanvas()
	{
		//LeftText
		wCanvas_L = new GameObject ("wCanvas_L", typeof(Canvas));
		wCanvas_L.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_L = AddTextToCanvas("", new GameObject ("uiText_L"));
		uiText_L.transform.SetParent (wCanvas_L.transform);
		uiText_L.transform.localPosition = Vector3.zero;
		wCanvas_L.transform.SetParent(lineParent);
		wCanvas_L.transform.localPosition = Vector3.zero;
		wCanvas_L.transform.localScale = Vector3.one * 0.04f * canvasScale;
		//RightText
		wCanvas_R = new GameObject ("wCanvas_R", typeof(Canvas));
		wCanvas_R.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_R = AddTextToCanvas("", new GameObject ("uiText_R"));
		uiText_R.transform.SetParent (wCanvas_R.transform);
		uiText_R.transform.localPosition = Vector3.zero;
		wCanvas_R.transform.SetParent (lineParent);
		wCanvas_R.transform.localPosition = Vector3.zero;
		wCanvas_R.transform.localScale = Vector3.one * 0.04f * canvasScale;
		//UpText
		wCanvas_U = new GameObject ("wCanvas_U", typeof(Canvas));
		wCanvas_U.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_U = AddTextToCanvas("", new GameObject ("uiText_U"));
		uiText_U.transform.SetParent (wCanvas_U.transform);
		uiText_U.transform.localPosition = Vector3.zero;
		wCanvas_U.transform.SetParent (lineParent);
		wCanvas_U.transform.localPosition = Vector3.zero;
		wCanvas_U.transform.localScale = Vector3.one * 0.04f * canvasScale;
		//DownText
		wCanvas_D = new GameObject ("wCanvas_D", typeof(Canvas));
		wCanvas_D.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_D = AddTextToCanvas("", new GameObject ("uiText_D"));
		uiText_D.transform.SetParent (wCanvas_D.transform);
		uiText_D.transform.localPosition = Vector3.zero;
		wCanvas_D.transform.SetParent (lineParent);
		wCanvas_D.transform.localPosition = Vector3.zero;
		wCanvas_D.transform.localScale = Vector3.one * 0.04f * canvasScale;
		//ForwardText
		wCanvas_F = new GameObject ("wCanvas_F", typeof(Canvas));
		wCanvas_F.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_F = AddTextToCanvas("", new GameObject ("uiText_F"));
		uiText_F.transform.SetParent (wCanvas_F.transform);
		uiText_F.transform.localPosition = Vector3.zero;
		wCanvas_F.transform.SetParent (lineParent);
		wCanvas_F.transform.localPosition = Vector3.zero;
		wCanvas_F.transform.localScale = Vector3.one * 0.04f * canvasScale;
		//BackwardText
		wCanvas_B = new GameObject ("wCanvas_B", typeof(Canvas));
		wCanvas_B.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
		uiText_B = AddTextToCanvas("", new GameObject ("uiText_B"));
		
		uiText_B.transform.SetParent (wCanvas_B.transform);
		uiText_B.transform.localPosition = Vector3.zero;
		wCanvas_B.transform.SetParent (lineParent);
		wCanvas_B.transform.localPosition = Vector3.zero;
		wCanvas_B.transform.localScale = Vector3.one * 0.04f * canvasScale;
	}

	public static Text AddTextToCanvas(string textString, GameObject textGameObject)
	{
		Text text = textGameObject.AddComponent<Text>();
		text.text = textString;
		text.alignment = TextAnchor.MiddleCenter;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.verticalOverflow = VerticalWrapMode.Overflow;
		Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
		text.font = ArialFont;
		text.material = ArialFont.material;
		return text;
	}

	void GetReallyTarget()
	{
		//Get Really Targets
		targetList.Clear ();
		if (targetObjects!=null && targetObjects.Length>0)
		{
			for (int i=0; i<targetObjects.Length; i++) {
				if (targetObjects[i]!=null){
					targetList.Add(targetObjects[i]);
				}
			}
		}
		targets = targetList.ToArray ();
	}

	void OnEnable()
	{
		
	}

	void OnDisable()
	{
		//Hide Common Six Lines
		if (line_R!=null)
			line_R.GetComponent<Renderer> ().enabled = false;
		if (line_L!=null)
			line_L.GetComponent<Renderer> ().enabled = false;
		if (line_U!=null)
			line_U.GetComponent<Renderer> ().enabled = false;
		if (line_D!=null)
			line_D.GetComponent<Renderer> ().enabled = false;
		if (line_F!=null)
			line_F.GetComponent<Renderer> ().enabled = false;
		if (line_B!=null)
			line_B.GetComponent<Renderer> ().enabled = false;
		//Hide Target lines
		if (targetLines!=null && targetLines.Length>0){
			for (int i=0;i<targetLines.Length;i++){
				if (targetLines[i]!=null)
					targetLines[i].GetComponent<Renderer> ().enabled = false;
			}
		}

		//UICanvas
		if (uiText_L!=null)
			uiText_L.text = "";
		if (uiText_R!=null)
			uiText_R.text = "";
		if (uiText_U!=null)
			uiText_U.text = "";
		if (uiText_D!=null)
			uiText_D.text = "";
		if (uiText_F!=null)
			uiText_F.text = "";
		if (uiText_B!=null)
			uiText_B.text = "";
		//linkTexts
		if (linkTexts.Count > 0) {
			for (int i = 0; i < linkTexts.Count; i++) {
				if (linkTexts [i]!=null)
					linkTexts [i].text = "";
			}
		}
	}

	void OnDestroy()
	{
		//Delete Common Six Lines
		if (line_L!=null)
			DestroyImmediate(line_L.gameObject);
		if (line_R!=null)
			DestroyImmediate(line_R.gameObject);
		if (line_U!=null)
			DestroyImmediate(line_U.gameObject);
		if (line_D!=null)
			DestroyImmediate(line_D.gameObject);
		if (line_F!=null)
			DestroyImmediate(line_F.gameObject);
		if (line_B!=null)
			DestroyImmediate(line_B.gameObject);
		//Destroy Target lines
		if (targetLines!=null && targetLines.Length>0){
			for (int i=0;i<targetLines.Length;i++){
				if (targetLines[i]!=null)
					DestroyImmediate(targetLines[i].gameObject);
			}
		}
	}

	void RefreshTargetLines()
	{
		if (targetLines!=null && targetLines.Length>0){
			for (int i=0;i<targetLines.Length;i++){
				if (targetLines[i]!=null)
					Destroy(targetLines[i].gameObject);
			}
		}
		targets = targetList.ToArray ();
		//Create Target Lines
		if (targets.Length > 0) 
		{
			targetLines = new Transform[targets.Length];
//			targetScreen = new Vector3[targets.Length];
			targetLength = new float[targets.Length];
			showTarget = new bool[targets.Length];

			for (int i=0; i<targets.Length; i++) {
				targetLines[i] = (Instantiate(Resources.Load("Measurelines/MeasureLine_Cube", typeof(GameObject))) as GameObject).transform;
				//targetLines[i] = GameObject.CreatePrimitive (PrimitiveType.Cube).transform;
				targetLines[i].localScale = new Vector3 (lineWidth, lineWidth, 1);
				lineRender = targetLines[i].GetComponent<Renderer> ();
				lineRender.sharedMaterial = mat;
				lineRender.enabled = false;
				DestroyImmediate (targetLines[i].GetComponent<BoxCollider>());
				lineRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				lineRender.receiveShadows = false;
				showTarget[i] = false;
				targetLines[i].gameObject.layer = layer;
				targetLines[i].name = gameObject.name+"_To_" + targets[i].name + "_line";
				GetLineCollection ();
				targetLines[i].parent = lineParent;
			}
		}

		//Clear last Target Lines's Text
		if (linkTexts.Count > 0) {
			for (int i = 0; i < linkTexts.Count; i++) {
				Destroy (linkTexts [i].transform.parent.gameObject);
			}
			linkTexts.Clear ();
		}

		nearPoint_L = line_L.position;
		nearPoint_R = line_R.position;
		nearPoint_U = line_U.position;
		nearPoint_D = line_D.position;
		nearPoint_F = line_F.position;
		nearPoint_B = line_B.position;
		//Link Texts
		if (targets.Length > 0) {
			nearPoint_Links = new Vector3[targets.Length];
			for (int i = 0; i < targetLines.Length; i++) {
				if (ShowTargetLine && showTarget [i] && targetLines [i] != null) {
					nearPoint_Links [i] = targetLines [i].position;
				}
			}
		}

		//Create Target Lines's Text
		if (targets.Length > 0) {
			for (int i=0;i<targetLines.Length;i++){
				if (ShowTargetLine && targetLines[i]!=null) {
					GameObject wCanvas_Link = new GameObject ("wCanvas_Link", typeof(Canvas));
					wCanvas_Link.GetComponent<Canvas> ().renderMode = RenderMode.WorldSpace;
					Text uiText = AddTextToCanvas("", new GameObject ("uiText_Link"));
					uiText.color = textColor;
					uiText.fontSize = textSize;
					uiText.fontStyle = FontStyle.Bold;
					uiText.transform.SetParent (wCanvas_Link.transform);
					uiText.transform.localPosition = Vector3.zero;
					uiText.text = NumberToPlaceString (targetLength[i], decimalPlaces);
					wCanvas_Link.transform.SetParent (lineParent);
					wCanvas_Link.transform.position = nearPoint_Links[i];
					wCanvas_Link.transform.localScale = Vector3.one * 0.04f * canvasScale;
					wCanvas_Link.transform.rotation = camTransform.rotation;
					linkTexts.Add (uiText);
				}
			}
		}

	}


//	bool InFrontOfCamera(Vector3 pos)
//	{
//		bool result = false;
//		if (Vector3.Dot (camTransform.forward, (pos - camTransform.position).normalized) > 0) {
//			result = true;
//		}
//		return result;
//	}

//	Vector3 ScreenToUICanvas(Vector2 screenPos, RectTransform canvasRT, Transform uiTarget)
//	{
//		Vector2 uv = new Vector2 (screenPos.x/Screen.width, screenPos.y/Screen.height);
//		Vector2 destPos = new Vector2 (canvasRT.anchoredPosition.x*2*uv.x, canvasRT.anchoredPosition.y*2*uv.y);
//		return new Vector3 (destPos.x, destPos.y, uiTarget.position.z);
//	}

	static public Vector3 NearCamPosition(Transform line, Transform cameraTran, float intervalDistance)
	{
		if (intervalDistance <= 0) {
			return line.position;
		}
		int segment = Mathf.RoundToInt (line.localScale.z / intervalDistance);
		if (segment < 3) 
		{
			return line.position;
		} 
		else 
		{
			if (segment % 2 != 0)
				segment++;
			Vector3[] segPoints = new Vector3[segment-1];
			Vector3 leftPoint = line.position + line.forward * Mathf.RoundToInt ((segment-2)/2) * intervalDistance;
			segPoints [0] = leftPoint;
			for (int i = 0; i < segment-2; i++) {
				segPoints [i + 1] = leftPoint + (-line.forward * (i + 1) * intervalDistance);
//			GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Sphere);
//			DestroyImmediate (obj.GetComponent<Collider> ());
//			obj.transform.localScale = Vector3.one / 2f;
//			obj.transform.position = segPoints [i + 1];
			}
			Vector3 result = segPoints [0];
			float minDistance = (result - cameraTran.position).sqrMagnitude;
			float tempDistance = 0;
			for (int i = 1; i < segPoints.Length; i++) {
				tempDistance = (segPoints [i] - cameraTran.position).sqrMagnitude;
				if (tempDistance < minDistance) {
					result = segPoints [i];
					minDistance = tempDistance;
				}
			}
			return result;
		}
	}

//	public string NumberToPlaceString(float number, int decimalPlaces, float decimalPow)
//	{
////		Debug.Log ("number = "+number+" decimalPlaces = "+decimalPlaces+" decimalPow = "+decimalPow);
//		string wholeStr = Mathf.Round (number * decimalPow).ToString();
//		int intLen = wholeStr.Length - decimalPlaces;
//		char[] resultChars = new char[wholeStr.Length + 2];
//		for (int i = 0; i < resultChars.Length-1; i++) {
//			if (i < intLen) {
//				resultChars [i] = wholeStr [i];
//			} else if (i == intLen) {
//				resultChars [i] = '.';
//			} else if (i > intLen) {
//				resultChars [i] = wholeStr [i-1];
//			}
//		}
//		resultChars[resultChars.Length-1] = 'm';
//		return new string(resultChars);
//	}

	public string NumberToPlaceString(float number, int decimalPlaces)
	{
		string formatString = "0.";
		for (int i = 0; i < decimalPlaces; i++) {
			formatString += "0";
		}

		if (StaticMetricOrImperial == MetricOrImperial.Foot)
		{
			number = number * 3.28084f;
			return Convert.ToDouble(number).ToString(formatString) + "ft";
		}
		else if (StaticMetricOrImperial == MetricOrImperial.Inches)
		{
			number = number * 3.28084f * 12f;
			return Convert.ToDouble(number).ToString(formatString) + "in";
		}
		else if (StaticMetricOrImperial == MetricOrImperial.Km)
		{
			number = number * 0.001f;
			return Convert.ToDouble(number).ToString(formatString) + "km";
		}
		else
		{
			return Convert.ToDouble(number).ToString(formatString) + "m";
		}
	}

	void LateUpdate()
	{
		mat.color = lineColor;
		line_L.localScale = new Vector3 (lineWidth, lineWidth, 1);
		line_R.localScale = new Vector3 (lineWidth, lineWidth, 1);
		line_U.localScale = new Vector3 (lineWidth, lineWidth, 1);
		line_D.localScale = new Vector3 (lineWidth, lineWidth, 1);
		line_F.localScale = new Vector3 (lineWidth, lineWidth, 1);
		line_B.localScale = new Vector3 (lineWidth, lineWidth, 1);

		//Color
		uiText_L.color = textColor;
		uiText_R.color = textColor;
		uiText_U.color = textColor;
		uiText_D.color = textColor;
		uiText_F.color = textColor;
		uiText_B.color = textColor;

		//Size
		uiText_L.fontSize = textSize;
		uiText_R.fontSize = textSize;
		uiText_U.fontSize = textSize;
		uiText_D.fontSize = textSize;
		uiText_F.fontSize = textSize;
		uiText_B.fontSize = textSize;

		//Decimal Places
		if (decimalPlaces < 0)
			decimalPlaces = 0;
		else if (decimalPlaces > 8)
			decimalPlaces = 8;

		//Link Object's Lines
		GetReallyTarget ();
		//Refresh Target Lines
		if (lastTargetLineCount != targets.Length) {
			RefreshTargetLines();
			lastTargetLineCount = targets.Length;
		}

		if (targets.Length > 0 && targetLines!=null && targetLines.Length>0) {
			for (int i=0;i<targetLines.Length;i++){
				if (targetLines[i]!=null)
					targetLines[i].localScale = new Vector3 (lineWidth, lineWidth, 1);
			}
		}
		//Angle Rotate and Pivot Bias
		if (coordinate == Coordinate.Local) {
			axisDummy.localEulerAngles = angle;
		}
		else {
			axisDummy.eulerAngles = angle;
		}
		axisDummy.localPosition = pivotBias;
		//Show by Axis
		if (AxisX_P) {
			LineRight ();
		} else {
			line_R.GetComponent<Renderer>().enabled = false;
			showR = false;
		}
		if (AxisX_N){
			LineLeft ();
		}else{
			line_L.GetComponent<Renderer>().enabled = false;
			showL = false;
		}
		if (AxisY_P) {
			LineUp ();
		} else {
			line_U.GetComponent<Renderer>().enabled = false;
			showU = false;
		}
		if (AxisY_N){
			LineDown ();
		}else{
			line_D.GetComponent<Renderer>().enabled = false;
			showD = false;
		}
		if (AxisZ_P) {
			LineForward ();
		} else {
			line_F.GetComponent<Renderer>().enabled = false;
			showF = false;
		}
		if (AxisZ_N){
			LineBack ();
		}else{
			line_B.GetComponent<Renderer>().enabled = false;
			showB = false;
		}
		//Show Target Lines
		if (ShowTargetLine) {
			TargetLines ();
		} else {
			if (targetLines!=null && targetLines.Length > 0) {
				for (int i=0;i<targetLines.Length;i++){
					if (targetLines[i]!=null)
					{
						targetLines[i].GetComponent<Renderer>().enabled = false;
						showTarget[i] = false;
                        linkTexts[i].enabled = false;
                    }
				}
			}
		}
		
		//StaticMetricOrImperial
		metricOrImperial = StaticMetricOrImperial;
		//StaticTextSizeVR
		textSizeVR = StaticTextSizeVR;
		if (InitSizeForVR){
			textSize = textSizeVR;
		}
		//LineColor
		lineColor = StaticLineColor;
		//TextColor
		textColor = StaticTextColor;
		
		//Line Near Camera Point Immediately when splitDistanceForText is below zero
		if (splitDistanceForText <= 0) 
		{
			nearPoint_L = line_L.position;
			nearPoint_R = line_R.position;
			nearPoint_U = line_U.position;
			nearPoint_D = line_D.position;
			nearPoint_F = line_F.position;
			nearPoint_B = line_B.position;
			//Text
			if (AxisX_N && showL) {
				uiText_L.text = NumberToPlaceString (lengthL, decimalPlaces);
				wCanvas_L.transform.position = nearPoint_L;
				wCanvas_L.transform.rotation = camTransform.rotation;
				wCanvas_L.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_L.text = "";
			}

			if (AxisX_P && showR) {
				uiText_R.text = NumberToPlaceString (lengthR, decimalPlaces);
				wCanvas_R.transform.position = nearPoint_R;
				wCanvas_R.transform.rotation = camTransform.rotation;
				wCanvas_R.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_R.text = "";
			}

			if (AxisY_P && showU) {
				uiText_U.text = NumberToPlaceString (lengthU, decimalPlaces);
				wCanvas_U.transform.position = nearPoint_U;
				wCanvas_U.transform.rotation = camTransform.rotation;
				wCanvas_U.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_U.text = "";
			}

			if (AxisY_N && showD) {
				uiText_D.text = NumberToPlaceString (lengthD, decimalPlaces);
				wCanvas_D.transform.position = nearPoint_D;
				wCanvas_D.transform.rotation = camTransform.rotation;
				wCanvas_D.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_D.text = "";
			}

			if (AxisZ_P && showF) {
				uiText_F.text = NumberToPlaceString (lengthF, decimalPlaces);
				wCanvas_F.transform.position = nearPoint_F;
				wCanvas_F.transform.rotation = camTransform.rotation;
				wCanvas_F.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_F.text = "";
			}

			if (AxisZ_N && showB) {
				uiText_B.text = NumberToPlaceString (lengthB, decimalPlaces);
				wCanvas_B.transform.position = nearPoint_B;
				wCanvas_B.transform.rotation = camTransform.rotation;
				wCanvas_B.transform.localScale = Vector3.one * 0.04f * canvasScale;
			} else {
				uiText_B.text = "";
			}
			//Link Texts
			if (targets.Length > 0) {
				nearPoint_Links = new Vector3[targets.Length];
				for (int i = 0; i < targetLines.Length; i++) {
					if (ShowTargetLine && showTarget [i] && targetLines [i] != null) {
						nearPoint_Links [i] = targetLines [i].position;
					}
				}
			}
			//Link Target Lines's Text
			if (linkTexts.Count > 0) {
				for (int i=0;i<linkTexts.Count;i++)
				{
					Text uiText = linkTexts[i];
					uiText.color = textColor;
					uiText.fontSize = textSize;
					uiText.transform.localPosition = Vector3.zero;
					uiText.text = NumberToPlaceString (targetLength[i], decimalPlaces);
					uiText.transform.parent.position = nearPoint_Links[i];
					uiText.transform.parent.localScale = Vector3.one * 0.04f * canvasScale;
					uiText.transform.parent.rotation = camTransform.rotation;
				}
			}
		} 
		else //Update Near Camera Point Every trackIntervalForText
		{
			if (t < trackIntervalForText) {
				t = t + Time.deltaTime;
			} else {
				t = 0;
				if (AxisX_N && showL) {
					nearPoint_L = NearCamPosition (line_L, camTransform, splitDistanceForText);
				}
				if (AxisX_P && showR) {
					nearPoint_R = NearCamPosition (line_R, camTransform, splitDistanceForText);
				}
				if (AxisY_P && showU) {
					nearPoint_U = NearCamPosition (line_U, camTransform, splitDistanceForText);
				}
				if (AxisY_N && showD) {
					nearPoint_D = NearCamPosition (line_D, camTransform, splitDistanceForText);
				}
				if (AxisZ_P && showF) {
					nearPoint_F = NearCamPosition (line_F, camTransform, splitDistanceForText);
				}
				if (AxisZ_N && showB) {
					nearPoint_B = NearCamPosition (line_B, camTransform, splitDistanceForText);
				}
				//Text
				if (AxisX_N && showL) {
					uiText_L.text = NumberToPlaceString (lengthL, decimalPlaces);
					wCanvas_L.transform.position = nearPoint_L;
					wCanvas_L.transform.rotation = camTransform.rotation;
					wCanvas_L.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_L.text = "";
				}

				if (AxisX_P && showR) {
					uiText_R.text = NumberToPlaceString (lengthR, decimalPlaces);
					wCanvas_R.transform.position = nearPoint_R;
					wCanvas_R.transform.rotation = camTransform.rotation;
					wCanvas_R.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_R.text = "";
				}

				if (AxisY_P && showU) {
					uiText_U.text = NumberToPlaceString (lengthU, decimalPlaces);
					wCanvas_U.transform.position = nearPoint_U;
					wCanvas_U.transform.rotation = camTransform.rotation;
					wCanvas_U.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_U.text = "";
				}

				if (AxisY_N && showD) {
					uiText_D.text = NumberToPlaceString (lengthD, decimalPlaces);
					wCanvas_D.transform.position = nearPoint_D;
					wCanvas_D.transform.rotation = camTransform.rotation;
					wCanvas_D.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_D.text = "";
				}

				if (AxisZ_P && showF) {
					uiText_F.text = NumberToPlaceString (lengthF, decimalPlaces);
					wCanvas_F.transform.position = nearPoint_F;
					wCanvas_F.transform.rotation = camTransform.rotation;
					wCanvas_F.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_F.text = "";
				}

				if (AxisZ_N && showB) {
					uiText_B.text = NumberToPlaceString (lengthB, decimalPlaces);
					wCanvas_B.transform.position = nearPoint_B;
					wCanvas_B.transform.rotation = camTransform.rotation;
					wCanvas_B.transform.localScale = Vector3.one * 0.04f * canvasScale;
				} else {
					uiText_B.text = "";
				}

				//Link Texts
				if (targets.Length > 0) {
					nearPoint_Links = new Vector3[targets.Length];
					for (int i = 0; i < targetLines.Length; i++) {
						if (ShowTargetLine && showTarget [i] && targetLines [i] != null) {
							nearPoint_Links[i] = NearCamPosition (targetLines[i], camTransform, splitDistanceForText);
						}
					}
				}
				//Link Target Lines's Text
				if (linkTexts.Count > 0) {
					for (int i=0;i<linkTexts.Count;i++)
					{
						Text uiText = linkTexts[i];
						uiText.color = textColor;
						uiText.fontSize = textSize;
						uiText.transform.localPosition = Vector3.zero;
						uiText.text = NumberToPlaceString (targetLength[i], decimalPlaces);
						uiText.transform.parent.position = nearPoint_Links[i];
						uiText.transform.parent.localScale = Vector3.one * 0.04f * canvasScale;
						uiText.transform.parent.rotation = camTransform.rotation;
					}
				}
			}
		}

			

	}


	void Update () 
	{
        //UICanvas
        if (uiText_L != null)
            uiText_L.enabled = IsShowText;
        if (uiText_R != null)
            uiText_R.enabled = IsShowText;
        if (uiText_U != null)
            uiText_U.enabled = IsShowText;
        if (uiText_D != null)
            uiText_D.enabled = IsShowText;
        if (uiText_F != null)
            uiText_F.enabled = IsShowText;
        if (uiText_B != null)
            uiText_B.enabled = IsShowText;
        //linkTexts
        if (linkTexts.Count > 0)
        {
            for (int i = 0; i < linkTexts.Count; i++)
            {
                if (linkTexts[i] != null)
                    linkTexts[i].enabled = IsShowText;
            }
        }
    }

    void TargetLines()
    {
        if (targets.Length > 0)
        {
            for (int i = 0; i < targetLines.Length; i++)
            {
                if (canBeBlock)
                {
                    if (Physics.Raycast(axisDummy.position, (targets[i].position - axisDummy.position).normalized, out hit_Target0, maxDistance, layerMask))
                    {
                        if ((hit_Target0.transform == targets[i]) || (hit_Target0.transform != targets[i] && !MeasureLineHelper.ExistInArray(hit_Target0.transform, targets)))
                        {
                            if (Physics.Raycast(hit_Target0.point, (axisDummy.position - targets[i].position).normalized, out hit_Target1, maxDistance, layerMask))
                            {
                                if (FindParentUpwards(hit_Target1.transform, myTransform))
                                {
                                    midPoint = (hit_Target0.point + hit_Target1.point) / 2;
                                }
                                else
                                {
                                    midPoint = (hit_Target0.point + axisDummy.position) / 2;
                                }
                            }
                            else
                            {
                                midPoint = (hit_Target0.point + axisDummy.position) / 2;
                            }
                            float length = (hit_Target0.point - midPoint).magnitude * 2;
                            targetLength[i] = length;
                            Transform targetLine = targetLines[i];
                            targetLine.GetComponent<Renderer>().enabled = true;
                            showTarget[i] = true;
                            targetLine.position = midPoint;
                            Vector3 vec = hit_Target0.point - midPoint;
                            if (vec != Vector3.zero)
                                targetLine.rotation = Quaternion.LookRotation(vec);
                            targetLine.localScale = new Vector3(targetLine.localScale.x, targetLine.localScale.y, length);
                            if (IsShowText)
                            {
                                linkTexts[i].enabled = true;
                            }
                        }
                        else
                        {
                            targetLines[i].GetComponent<Renderer>().enabled = false;
                            showTarget[i] = false;
                            linkTexts[i].enabled = false;
                        }
                    }
                    else
                    {
                        targetLines[i].GetComponent<Renderer>().enabled = false;
                        showTarget[i] = false;
                        linkTexts[i].enabled = false;
                    }
                }
                else if ((targets[i].position - axisDummy.position).magnitude <= maxDistance)
                {
                    midPoint = (targets[i].position + axisDummy.position) / 2;
                    float length = (targets[i].position - midPoint).magnitude * 2;
                    targetLength[i] = length;
                    Transform targetLine = targetLines[i];
                    targetLine.GetComponent<Renderer>().enabled = true;
                    showTarget[i] = true;
                    targetLine.position = midPoint;
                    Vector3 vec = targets[i].position - midPoint;
                    if (vec != Vector3.zero)
                        targetLine.rotation = Quaternion.LookRotation(vec);
                    targetLine.localScale = new Vector3(targetLine.localScale.x, targetLine.localScale.y, length);
                    if (IsShowText)
                    {
                        linkTexts[i].enabled = true;
                    }
                }
            }
        }
        else
        {
            if (showTarget!=null && showTarget.Length > 0)
            {
                for (int i = 0; i < showTarget.Length; i++)
                {
                    showTarget[i] = false;
                }
            }
        }
    }

    void LineBack()
	{
		if (Physics.Raycast (axisDummy.position, -axisDummy.forward, out hit_Back0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Back0.point, axisDummy.forward, out hit_Back1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Back1.transform, myTransform))
				{
					midPoint = (hit_Back0.point + hit_Back1.point)/2;
				}
				else
				{
					midPoint = (hit_Back0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Back0.point + axisDummy.position)/2;
			}
			lengthB = (hit_Back0.point - midPoint).magnitude*2;
			line_B.GetComponent<Renderer>().enabled = true;
			showB = true;
			line_B.position = midPoint;
			Vector3 vec = hit_Back0.point - midPoint;
			if (vec!=Vector3.zero)
				line_B.rotation = Quaternion.LookRotation(vec);
			line_B.localScale = new Vector3(line_B.localScale.x, line_B.localScale.y, lengthB);
		}
		else
		{
			line_B.GetComponent<Renderer>().enabled = false;
			showB = false;
		}
	}

	void LineForward()
	{
		if (Physics.Raycast (axisDummy.position, axisDummy.forward, out hit_Forward0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Forward0.point, -axisDummy.forward, out hit_Forward1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Forward1.transform, myTransform))
				{
					midPoint = (hit_Forward0.point + hit_Forward1.point)/2;
				}
				else
				{
					midPoint = (hit_Forward0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Forward0.point + axisDummy.position)/2;
			}
			lengthF = (hit_Forward0.point - midPoint).magnitude*2;
			line_F.GetComponent<Renderer>().enabled = true;
			showF = true;
			line_F.position = midPoint;
			Vector3 vec = hit_Forward0.point - midPoint;
			if (vec!=Vector3.zero)
				line_F.rotation = Quaternion.LookRotation(vec);
			line_F.localScale = new Vector3(line_F.localScale.x, line_F.localScale.y, lengthF);
		}
		else
		{
			line_F.GetComponent<Renderer>().enabled = false;
			showF = false;
		}
	}

	void LineDown()
	{
		if (Physics.Raycast (axisDummy.position, -axisDummy.up, out hit_Down0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Down0.point, axisDummy.up, out hit_Down1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Down1.transform, myTransform))
				{
					midPoint = (hit_Down0.point + hit_Down1.point)/2;
				}
				else
				{
					midPoint = (hit_Down0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Down0.point + axisDummy.position)/2;
			}
			lengthD = (hit_Down0.point - midPoint).magnitude*2;
			line_D.GetComponent<Renderer>().enabled = true;
			showD = true;
			line_D.position = midPoint;
			Vector3 vec = hit_Down0.point - midPoint;
			if (vec!=Vector3.zero)
				line_D.rotation = Quaternion.LookRotation(vec);
			line_D.localScale = new Vector3(line_D.localScale.x, line_D.localScale.y, lengthD);
		}
		else
		{
			line_D.GetComponent<Renderer>().enabled = false;
			showD = false;
		}
	}

	void LineUp()
	{
		if (Physics.Raycast (axisDummy.position, axisDummy.up, out hit_Up0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Up0.point, -axisDummy.up, out hit_Up1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Up1.transform, myTransform))
				{
					midPoint = (hit_Up0.point + hit_Up1.point)/2;
				}
				else
				{
					midPoint = (hit_Up0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Up0.point + axisDummy.position)/2;
			}
			lengthU = (hit_Up0.point - midPoint).magnitude*2;
			line_U.GetComponent<Renderer>().enabled = true;
			showU = true;
			line_U.position = midPoint;
			Vector3 vec = hit_Up0.point - midPoint;
			if (vec!=Vector3.zero)
				line_U.rotation = Quaternion.LookRotation(vec);
			line_U.localScale = new Vector3(line_U.localScale.x, line_U.localScale.y, lengthU);
		}
		else
		{
			line_U.GetComponent<Renderer>().enabled = false;
			showU = false;
		}
	}

	void LineLeft()
	{
		if (Physics.Raycast (axisDummy.position, -axisDummy.right, out hit_Left0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Left0.point, axisDummy.right, out hit_Left1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Left1.transform, myTransform))
				{
					midPoint = (hit_Left0.point + hit_Left1.point)/2;
				}
				else
				{
					midPoint = (hit_Left0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Left0.point + axisDummy.position)/2;
			}
			lengthL = (hit_Left0.point - midPoint).magnitude*2;
			line_L.GetComponent<Renderer>().enabled = true;
			showL = true;
			line_L.position = midPoint;
			Vector3 vec = hit_Left0.point - midPoint;
			if (vec!=Vector3.zero)
				line_L.rotation = Quaternion.LookRotation(vec);
			line_L.localScale = new Vector3(line_L.localScale.x, line_L.localScale.y, lengthL);
		}
		else
		{
			line_L.GetComponent<Renderer>().enabled = false;
			showL = false;
		}
	}

	void LineRight()
	{
		if (Physics.Raycast (axisDummy.position, axisDummy.right, out hit_Right0, maxDistance, layerMask)) 
		{
			if (Physics.Raycast (hit_Right0.point, -axisDummy.right, out hit_Right1, maxDistance, layerMask))
			{
				if (FindParentUpwards(hit_Right1.transform, myTransform))
				{
					midPoint = (hit_Right0.point + hit_Right1.point)/2;
				}
				else
				{
					midPoint = (hit_Right0.point + axisDummy.position)/2;
				}
			}
			else
			{
				midPoint = (hit_Right0.point + axisDummy.position)/2;
			}
			lengthR = (hit_Right0.point - midPoint).magnitude*2;
			line_R.GetComponent<Renderer>().enabled = true;
			showR = true;
			line_R.position = midPoint;
			Vector3 vec = hit_Right0.point - midPoint;
			if (vec!=Vector3.zero)
				line_R.rotation = Quaternion.LookRotation(vec);
			line_R.localScale = new Vector3(line_R.localScale.x, line_R.localScale.y, lengthR);
		}
		else
		{
			line_R.GetComponent<Renderer>().enabled = false;
			showR = false;
		}
	}

	static public bool FindParentUpwards(Transform child, Transform targetParent)
	{
		bool result = false;
		if (child == targetParent) {
			result = true;
		}
		else{
			Transform parent = child.parent;
			while (parent!=null) {
				if (parent == targetParent){
					result = true;
					break;
				}
				else{
					parent = parent.parent;
				}
			}
		}
		return result;
	}

}



