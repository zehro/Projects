using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Util;
using UnityEngine.EventSystems;
using Assets.Scripts.Managers;

public class SliderFix : MonoBehaviour {

	public Slider slider;
	public bool isSfx;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (CustomInput.BoolHeld(CustomInput.UserInput.Right)) Navigate(CustomInput.UserInput.Right);
		if (CustomInput.BoolHeld(CustomInput.UserInput.Left)) Navigate(CustomInput.UserInput.Left);
	}

	#region NAVIGATION
	private void Navigate(CustomInput.UserInput direction)
	{
		if ( EventSystem.current.currentSelectedGameObject == this.gameObject) {
			switch(direction)
			{
			case CustomInput.UserInput.Left:
				slider.value -= .01f;
				break;
			case CustomInput.UserInput.Right:
				slider.value += .01f;
				break;
			}
		}

		if (isSfx)
			GameManager.SFXVol = slider.value;
		else
			GameManager.MusicVol = slider.value;
	}
	#endregion
}
