using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof (Image))]
public class AnswerPopup : MonoBehaviour 
{
	public Image popupImage;

	public void ShowPopup (Sprite sprite)
	{
		popupImage.sprite = sprite;
		popupImage.enabled = true;
	}


	public void HidePopup ()
	{
		popupImage.enabled = false;
	}

	public void HidePopupWithDelay (float delay)
	{
		StartCoroutine (_HidePopupWithDelay (delay));
	}

	private IEnumerator _HidePopupWithDelay (float delay)
	{
		yield return new WaitForSeconds (delay);
		HidePopup ();
	}

	void Start () 
	{
		if (popupImage == null)
			popupImage = GetComponent <Image> ();
	}
}
