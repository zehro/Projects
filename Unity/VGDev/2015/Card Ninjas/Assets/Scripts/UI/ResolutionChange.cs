using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Util;

public class ResolutionChange : MonoBehaviour {

	Dropdown dropdown;
	public ConfirmationDialogController CDC;

	public Resolution lastResolution;
	public string lastOption;

	public bool notFirstTime = false;

	public Selectable Quality;
	// Use this for initialization
	void Start () {
		dropdown = GetComponent<Dropdown>();
		int i = 0;
		foreach (Resolution res in Screen.resolutions) {
			Dropdown.OptionData opt = new Dropdown.OptionData();
			opt.text = res.ToString().Split('@')[0];
			dropdown.options.Add(opt);
			if (Screen.currentResolution.ToString() == res.ToString()) 
				dropdown.gameObject.transform.GetChild(0).GetComponent<Text>().text = res.ToString().Split('@')[0];
			i++;
		}


	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.transform.childCount == 5 ) {
			Navigation customNav = new Navigation();
			customNav.mode = Navigation.Mode.Automatic;
			dropdown.navigation = customNav;
		}
		else if (!this.CDC.hideBehaviour.OnScreen) {
			Navigation customNav = new Navigation();
			customNav.mode = Navigation.Mode.Explicit;
			customNav.selectOnDown = Quality;
			dropdown.navigation = customNav;
		}
	}

	public void SetResolution() {
		if (EventSystem.current.currentSelectedGameObject.transform.IsChildOf(this.gameObject.transform) || EventSystem.current.currentSelectedGameObject == this.gameObject) {
			//Debug.Log("Resolution set");
			EventSystem.current.SetSelectedGameObject(this.gameObject);

			lastResolution = Screen.currentResolution;
			lastOption = dropdown.options[dropdown.value].text;
			Screen.SetResolution(Screen.resolutions[this.dropdown.value].width, Screen.resolutions[this.dropdown.value].height, Screen.fullScreen);
			Camera.main.ResetAspect();
			CDC.BringUpKeep();
			//Debug.Log("current after set: " + EventSystem.current.currentSelectedGameObject);
			CDC.Go = ChangeResolutionBack;
			//Debug.Log("current after callback set: " + EventSystem.current.currentSelectedGameObject);
		}
	}

	public void ChangeResolutionBack() {
		Screen.SetResolution(lastResolution.width, lastResolution.height, Screen.fullScreen);
		Camera.main.ResetAspect();
		dropdown.gameObject.transform.GetChild(0).GetComponent<Text>().text = lastOption;
	}
}
