using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;

public class Elasticity : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public float value = 0f;
	private Slider slider = null;
	private bool isPressed = false;

	void Update ()
	{
		if(slider == null)
		{
			slider = GetComponent<Slider> ();
		}

		else
		{
			if(!isPressed)
			{
				slider.value = Mathf.Lerp (slider.value, 0f, Time.deltaTime * 3.0f);
			}
		}

		value = slider.value;
	}

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		isPressed = true;
		Debug.Log ("Start Press");
	}
	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		isPressed = false;
		Debug.Log ("End Press");
	}

	#endregion
}
