using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Util;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Managers;

public class MenusController : MonoBehaviour {

	
	public UIHideBehaviour MainMenu1;
	public UIHideBehaviour MainMenu2;
	public GameObject MenuSelected;

	public UIHideBehaviour Settings;
	public GameObject SettingsSelected;
	public SettingsMenuController SettingsController;

	public UIHideBehaviour LevelSelect1;
	public UIHideBehaviour LevelSelect2;
	public GameObject LevelSelected;

	public UIHideBehaviour Tutorial;
	public GameObject TutorialSelected;

	public GameObject CurrentDefault;

	public GameObject No;
	public bool isNew;

	// Use this for initialization
	void Start () {
		GameManager.SFXVol = 1;
		GameManager.MusicVol = 1;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

		if ((CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel) || CustomInput.BoolFreshPress(CustomInput.UserInput.Pause))  && Settings == null)
			FindObjectOfType<UILevelSwitch>().BackToMenu();
		else if(CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel) && (Settings.OnScreen && SettingsController.Settings.activeInHierarchy || LevelSelect1.OnScreen))
			GoToMainMenu();

		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Accept)) navigateAccept();

		
		
		if ((MainMenu1.OnScreen || LevelSelect1.OnScreen) && EventSystem.current.currentSelectedGameObject != null)
			CurrentDefault = EventSystem.current.currentSelectedGameObject;

		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Up)) Navigator.Navigate(CustomInput.UserInput.Up, CurrentDefault);
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Down)) Navigator.Navigate(CustomInput.UserInput.Down, CurrentDefault);
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Right)) Navigator.Navigate(CustomInput.UserInput.Right, CurrentDefault);
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Left)) Navigator.Navigate(CustomInput.UserInput.Left, CurrentDefault);
	}


	public void GoToSettings() {
		if (MainMenu1.OnScreen) MenuSelected = EventSystem.current.currentSelectedGameObject;
		MainMenu1.OnScreen = false;
		MainMenu2.OnScreen = false;
		
		LevelSelect1.OnScreen = false;
		LevelSelect2.OnScreen = false;
		
		Tutorial.OnScreen = false;

		Settings.OnScreen = true;
		CurrentDefault = SettingsSelected;
		EventSystem.current.SetSelectedGameObject(SettingsSelected);
	}

	public void GoToMainMenu() {
		if (LevelSelect1.OnScreen) LevelSelected = EventSystem.current.currentSelectedGameObject;
		Settings.OnScreen = false;

		LevelSelect1.OnScreen = false;
		LevelSelect2.OnScreen = false;
		
		Tutorial.OnScreen = false;
		
		MainMenu1.OnScreen = true;
		MainMenu2.OnScreen = true;
		CurrentDefault = MenuSelected;
		EventSystem.current.SetSelectedGameObject(MenuSelected);
	}

	public void GoToLevelSelect() {
		if (MainMenu1.OnScreen) MenuSelected = EventSystem.current.currentSelectedGameObject;
		Settings.OnScreen = false;
		
		MainMenu1.OnScreen = false;
		MainMenu2.OnScreen = false;

		Tutorial.OnScreen = false;

		LevelSelect1.OnScreen = true;
		LevelSelect2.OnScreen = true;
		CurrentDefault = LevelSelected;
		EventSystem.current.SetSelectedGameObject(LevelSelected);
	}

	public void GoToTutorial() {
		if (MainMenu1.OnScreen) MenuSelected = EventSystem.current.currentSelectedGameObject;
		Settings.OnScreen = false;
		
		MainMenu1.OnScreen = false;
		MainMenu2.OnScreen = false;
		
		LevelSelect1.OnScreen = false;
		LevelSelect2.OnScreen = false;

		Tutorial.OnScreen = true;

		CurrentDefault = TutorialSelected;
		EventSystem.current.SetSelectedGameObject(TutorialSelected);
	}

	#region NAVIGATION
	/// <summary>
	/// Override for the accept navigation event
	/// </summary>
	/// <remarks>Necessary for the weird dropdown edge cases</remarks>
	private void navigateAccept()
	{
		GameObject next = EventSystem.current.currentSelectedGameObject;
        if (next == null) return;
		var pointer = new PointerEventData(EventSystem.current);
		Toggle tempTog = next.GetComponent<Toggle>();
		bool isNew = true;
		if (tempTog) isNew = !tempTog.isOn;
		ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, pointer, ExecuteEvents.submitHandler);
		if (next.transform.parent.parent.parent.gameObject.GetComponent<ScrollRect>() != null && isNew) {
			EventSystem.current.SetSelectedGameObject(No);
		}
		return;
	}
	#endregion
}
