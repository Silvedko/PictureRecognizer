using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent (typeof(UnityEngine.UI.Text))]
public class CharTimer : MonoBehaviour 
{
	public delegate void TimerDelegate ();
	public static event TimerDelegate OnTimeIsOverEvent;

	private bool stopTimer = true;

	Text timerText;
	float endTime;

	/// <summary>
	/// Values for String.Join for correct displaying text
	/// </summary>
	string[] values = new string[2];

	public void StopTimer ()
	{
		stopTimer = true;
	}

	public void StartTimer (int time)
	{
		if (time > 0)
		{
			stopTimer = false;
			endTime = time;
		} 
		else
		{
			StopTimer ();
			EnableTimer (false);
		}
	}

	public void EnableTimer (bool isEnable)
	{
		if(timerText != null)
			timerText.enabled = isEnable;
	}

	void Awake ()
	{
		InitTimer ();
	}

	void InitTimer () 
	{
		endTime = Time.time;
		timerText = gameObject.GetComponent <Text> ();
	}

	void Update () 
	{
		if(!stopTimer)
		{
			endTime -= Time.deltaTime;

			if (endTime < 0) 
			{
				endTime = 0;
				//stopUpdate = true;

				if (OnTimeIsOverEvent != null)
					OnTimeIsOverEvent ();
			}

			values [0] = SetCorrectNumbers((int)endTime / 60);
			values [1] = SetCorrectNumbers((int)endTime % 60);

			timerText.text = String.Join (":", values);
		}
	}

	string SetCorrectNumbers (int number)
	{
		string num;
		if (number < 10 && number >= 0)
			num = 0 + number.ToString ();
		else
			num = number.ToString ();

		return num;
	}
}
