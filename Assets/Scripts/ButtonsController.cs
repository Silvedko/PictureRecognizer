using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonsController : MonoBehaviour 
{

	public void OnQuitButtonClicked () 
	{
		Application.Quit ();
	}

	public void OnPlayButtonClicked ()
	{
		SceneManager.LoadScene (1);
	}
}
