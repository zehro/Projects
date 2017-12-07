using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Util;

public class SettingsMenuController : MonoBehaviour {

	public Text subtitleTab;
	public UIHideBehaviour subtitleHide;

	public GameObject Video;
	public GameObject VideoSelect;

	public GameObject Audio;
	public GameObject AudioSelect;

	public GameObject Settings;
	public GameObject SettingsSelect;

	public EventSystem es;
	
	// Update is called once per frame
	void Update () {
		if(CustomInput.BoolFreshPressDeleteOnRead(CustomInput.UserInput.Cancel) && (!Settings.activeInHierarchy))
			ChangeToSettings();
	}

	public void ChangeToVideo() {
		subtitleTab.text = "Video";
		subtitleHide.OnScreen = true;

		Settings.SetActive(false);
		Video.SetActive(true);
		
		es.SetSelectedGameObject(VideoSelect);
	}

	public void ChangeToAudio() {
		subtitleTab.text = "Audio";
		subtitleHide.OnScreen = true;

		Settings.SetActive(false);
		Audio.SetActive(true);
		
		es.SetSelectedGameObject(AudioSelect);
	}

	public void ChangeToSettings() {
		subtitleHide.OnScreen = false;


		Video.SetActive(false);
		Audio.SetActive(false);
		Settings.SetActive(true);

		es.SetSelectedGameObject(SettingsSelect);
	}
}
