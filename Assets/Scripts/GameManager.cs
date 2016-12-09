﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using PDollarGestureRecognizer;

public class GameManager : MonoBehaviour 
{
	public static bool stopUserDrawing = false;
	public GameObject buttonQuit;

	int score = 0;
	float delayBetweenRounds = 0.5f;

	#region Answer sprites
	[SerializeField]
	Sprite correctAnswerSprite;
	[SerializeField]
	Sprite wrongAnswerSprite;

	[SerializeField]
	Image answerPopupImage;
	#endregion

	[SerializeField]
	CharTimer timer;

	Gesture[] sampleGestures;


	//  Text messages for user
	[SerializeField]
	Text textArea = null; 

	int currentGestureIndex = -1;
	int timeForCurrentGesture = 0;
	LineRenderer lineRenderer;

	public void OnButtonQuitClicked ()
	{
		Application.Quit ();
	}

	private void Start ()
	{
		buttonQuit.SetActive (false);
		stopUserDrawing = false;
		GestrureDrawer.OnUserDrawedGesture += delegate(List<Vector2> points) {
			RecognizeGesture (points);
		};
		CharTimer.OnTimeIsOverEvent += OnTimerEnded;

		lineRenderer = gameObject.AddComponent <LineRenderer> ();
		sampleGestures = LoadSampleGesturesFromXML ();
		timeForCurrentGesture = sampleGestures.Length;

		StartCoroutine (NextGestureWithDelay (0));
	}


	private void DrawGestureFromXML (Vector2[] points, float lifeTime)
	{
		if (points != null)
		{
			List<Vector3> tempPoints = new List<Vector3> ();
			lineRenderer.enabled = true;

			foreach (Vector2 point in points)
			{
				Vector3 vec = new Vector3 (point.x, point.y, 0);
				tempPoints.Add (vec);
			}
			lineRenderer.SetWidth (0.2f, 0.2f);
			lineRenderer.SetVertexCount (tempPoints.Count);
			lineRenderer.SetPositions (tempPoints.ToArray ());

			StartCoroutine (HideGesture(lifeTime));
		} 
		else
		{
			Debug.LogError ("Need points to draw gesture");
		}
	}

	private IEnumerator HideGesture (float time)
	{
		yield return new WaitForSeconds (time);	 
		lineRenderer.enabled = false;
	}


	private void RecognizeGesture(List<Vector2> pointsFromUser)
	{
		Gesture candidate = new Gesture(pointsFromUser.ToArray());
		string recognizedGestureName = PointCloudRecognizer.Classify(candidate, sampleGestures);
		AnalyzeAnswer (recognizedGestureName, sampleGestures[currentGestureIndex].Name);
	}

	/// <summary>
	/// Loads the sample gestures from XML data.
	/// </summary>
	/// <returns>The sample gestures from XM.</returns>
	private Gesture[] LoadSampleGesturesFromXML ()
	{
		//loadedGestureNames = new List<string> ();
		List<Gesture> gestures = new List<Gesture>();
		var assets_temp = Resources.LoadAll ("GestureSet\\NewGestures");

		foreach (var gestureAsset in assets_temp)
		{
			Gesture tempGesture = XMLManager.ReadGesture ((TextAsset)gestureAsset);
			gestures.Add(tempGesture);
		}

		gestures.Shuffle();
		return gestures.ToArray ();
	}
		

	private void NextGesture ()
	{
		StartCoroutine (NextGestureWithDelay (0.5f));
	}

	private IEnumerator NextGestureWithDelay (float delay)
	{
		yield return new WaitForSeconds (delay);

		timer.EnableTimer (true);
		timer.StartTimer (timeForCurrentGesture);
		timeForCurrentGesture--;

		if (currentGestureIndex < sampleGestures.Length - 1)
		{
			currentGestureIndex ++;
			DrawGestureFromXML (sampleGestures [currentGestureIndex].points, 1f);
		}
		else
			OnGameEnded ();
	}
		

	private void OnTimerEnded ()
	{
		NextGesture ();
	}


	#region User answer analyzer

	private void AnalyzeAnswer (string userGestureName, string realGestureName)
	{
		timer.StopTimer ();
		timer.EnableTimer (false);

		if (userGestureName == sampleGestures[currentGestureIndex].Name)
			CorrectAnswer ();
		else
			WrongAnswer ();
		
		NextGesture ();
	}

	private void CorrectAnswer ()
	{
		ShowAnswerPopup (true);

		score ++;
	}

	private void WrongAnswer ()
	{
		ShowAnswerPopup (false);
	}

	private void ShowAnswerPopup (bool isCorrect)
	{
		answerPopupImage.enabled = true;
		answerPopupImage.sprite = isCorrect ? correctAnswerSprite : wrongAnswerSprite;
		HideAnswerPopup ();
	}

	private void HideAnswerPopup ()
	{
		StartCoroutine (HidePopupWithDelay (0.5f));
	}

	private IEnumerator HidePopupWithDelay (float delay)
	{
		yield return new WaitForSeconds (delay);
		answerPopupImage.enabled = false;
	}

	#endregion


	private void OnGameEnded ()
	{
		timer.StopTimer ();
		timer.EnableTimer (false);

		stopUserDrawing = true;

		textArea.text = "YOU WIN! \n Score: " + score;
		buttonQuit.SetActive (true);
	}
		

	private void OnDestroy ()
	{
		Resources.UnloadUnusedAssets ();
	}
}
