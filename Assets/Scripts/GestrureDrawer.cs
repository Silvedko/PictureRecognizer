using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class GestrureDrawer : MonoBehaviour 
{
	public GameObject trailPrefab;
	public string currentFigureName;
	public delegate void GestureDrawerDelegate (List <Vector2> points);
	public static event GestureDrawerDelegate OnUserDrawedGesture;

	GameObject thisTrailObj;
	TrailRenderer trail;
	Vector3 startPos;
	Plane objPlane;

	List<Vector2> pointsFromUser;

	void Start () 
	{
		objPlane = new Plane (Camera.main.transform.forward * -1, this.transform.position);
		pointsFromUser = new List<Vector2> ();
	}

	void Update ()
	{
		if(!GameManager.stopUserDrawing)
			DrawFigureFromTouch ();
	}

	void DrawFigureFromTouch ()
	{
		if ((Input.touchCount > 0) && Input.GetTouch (0).phase == TouchPhase.Began || Input.GetMouseButtonDown (0))
		{
			thisTrailObj = (GameObject)Instantiate (trailPrefab, this.transform.position, Quaternion.identity);

			Ray mRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;


			if (objPlane.Raycast (mRay, out rayDistance))
			{
				startPos = mRay.GetPoint (rayDistance);
				pointsFromUser.Add (startPos);
			}
		} 
		else if ((Input.touchCount > 0) && Input.GetTouch (0).phase == TouchPhase.Moved || Input.GetMouseButton (0))
		{
			Ray mRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			float rayDistance;

			if (objPlane.Raycast (mRay, out rayDistance))
			{
				thisTrailObj.transform.position = mRay.GetPoint (rayDistance);

				pointsFromUser.Add (thisTrailObj.transform.position);
			}
		} 
		else if ((Input.touchCount > 0) && Input.GetTouch (0).phase == TouchPhase.Ended || Input.GetMouseButtonUp (0))
		{
			OnUserDrawedGesture (pointsFromUser);
			Destroy (thisTrailObj, 0.5f);
		}

	}

//	Gesture[] LoadSampleGesturesFromXML ()
//	{
//		
//		List<Gesture> gestures = new List<Gesture>();
//		var assets_temp = Resources.LoadAll ("GestureSet\\NewGestures");
//
//		foreach (var gestureAsset in assets_temp)
//		{
//			gestures.Add(XMLManager.ReadGesture((TextAsset)gestureAsset));
//		}
//		return gestures.ToArray();
//	}





}
