using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Util;
using UnityEngine.EventSystems;

public class SliderFix1 : MonoBehaviour {

	public Slider slider;
	public SoundPlayer sp;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Right)) Navigate(CustomInput.UserInput.Right);
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Left)) Navigate(CustomInput.UserInput.Left);
	}

	#region NAVIGATION
	private void Navigate(CustomInput.UserInput direction)
	{
		if ( EventSystem.current.currentSelectedGameObject == this.gameObject) {
			switch(direction)
			{
			case CustomInput.UserInput.Left:
				slider.value -= 1;
				break;
			case CustomInput.UserInput.Right:
				slider.value += 1;
				break;
			}
			if (sp) sp.SetVolume(slider.value);
		}
	}
	#endregion
}
