using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Data;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseUI : MonoBehaviour {

	public Text pause1,pause2,pause3;

	public bool pauseMenuOpen;

	public Selectable startingButton, controlsButton;

	public GameObject controlsMenu;
	public GameObject pauseButtons;

	private float hTimer, vTimer, delay = 0.1f;
	private bool dpadPressed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(pauseMenuOpen || GameManager.instance.IsPaused) {
			transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(871,275f), Time.deltaTime*(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10f+10f));
			if(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y > 120) {
				pause1.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause1.rectTransform.anchoredPosition3D,new Vector3(-1.5f,0,0),Mathf.Abs(pause1.rectTransform.anchoredPosition3D.x)*Time.deltaTime);
				pause2.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause2.rectTransform.anchoredPosition3D,new Vector3(1.5f,0,0),pause2.rectTransform.anchoredPosition3D.x*Time.deltaTime);
				pause3.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause3.rectTransform.anchoredPosition3D,Vector3.zero,pause3.rectTransform.anchoredPosition3D.y*Time.deltaTime);
			}
			Navigator.defaultGameObject = startingButton.gameObject;
			if(!EventSystem.current.currentSelectedGameObject) EventSystem.current.SetSelectedGameObject(startingButton.gameObject);
			// No axis is being pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) == 0)
			{
				// Reset the timer so that we don't continue scrolling
				hTimer = 0;
			}
			// Horizontal joystick is held right
			// Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) > ControllerManager.CUSTOM_DEADZONE)
			{
				// If we can move and it is time to move
				if (hTimer >= delay)
				{
					// Move and reset timer
					Navigator.Navigate(Enums.MenuDirections.Right);
					hTimer = 0;
				}
				hTimer += Time.deltaTime;
			}
			// Horizontal joystick is held left
			// Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE)
			{
				// If we can move and it is time to move
				if (hTimer >= delay)
				{
					// Move and reset timer
					Navigator.Navigate(Enums.MenuDirections.Left);
					hTimer = 0;
				}
				hTimer += Time.deltaTime;
			}

			// No axis is being pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) == 0)
			{
				// Reset the timer so that we don't continue scrolling
				vTimer = 0;
			}
			// Horizontal joystick is held right
			// Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) > ControllerManager.CUSTOM_DEADZONE)
			{
				// If we can move and it is time to move
				if (vTimer >= delay)
				{
					// Move and reset timer
					Navigator.Navigate(Enums.MenuDirections.Up);
					vTimer = 0;
				}
				vTimer += Time.deltaTime;
			}
			// Horizontal joystick is held left
			// Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE)
			{
				// If we can move and it is time to move
				if (vTimer >= delay)
				{
					// Move and reset timer
					Navigator.Navigate(Enums.MenuDirections.Down);
					vTimer = 0;
				}
				vTimer += Time.deltaTime;
			}

			// Have dpad functionality so that player can have precise control and joystick quick navigation
			// Check differently for Windows vs OSX

			// No dpad button is pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) == 0 && (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) == 0)) dpadPressed = false;
			// Dpad right is pressed; treating as DPADRightOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) > 0 && !dpadPressed)
			{
				dpadPressed = true;
				Navigator.Navigate(Enums.MenuDirections.Right);
			}
			// Dpad right is pressed; treating as DPADLeftOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) < 0 && !dpadPressed)
			{
				dpadPressed = true;
				Navigator.Navigate(Enums.MenuDirections.Left);
			}
			// Dpad up is pressed; treating as DPADUpOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) > 0 && !dpadPressed)
			{
				dpadPressed = true;
				Navigator.Navigate(Enums.MenuDirections.Up);
			}
			// Dpad down is pressed; treating as DPADDownOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) < 0 && !dpadPressed)
			{
				dpadPressed = true;
				Navigator.Navigate(Enums.MenuDirections.Down);
			}

			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, PlayerID.One)) Navigator.CallSubmit();
		} else {
			transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(871,0f), Time.deltaTime*(650-transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*2f));
			pause1.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause1.rectTransform.anchoredPosition3D,new Vector3(-500f,0,0),Mathf.Abs(pause1.rectTransform.anchoredPosition3D.x*10f-1f)*Time.deltaTime);
			pause2.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause2.rectTransform.anchoredPosition3D,new Vector3(500f,0,0),(pause2.rectTransform.anchoredPosition3D.x*10f+1f)*Time.deltaTime);
			pause3.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause3.rectTransform.anchoredPosition3D,new Vector3(0,200f,0),(pause3.rectTransform.anchoredPosition3D.y*10f+1f)*Time.deltaTime);
		}
	}

	public void ResumePressed() {
		GameManager.instance.IsPaused = false;
	}

	public void ControlsPressed() {
		EventSystem.current.SetSelectedGameObject(controlsButton.gameObject);
		pauseButtons.SetActive(false);
		controlsMenu.SetActive(true);
	}

	public void BackFromControls() {
		EventSystem.current.SetSelectedGameObject(startingButton.gameObject);
		pauseButtons.SetActive(true);
		controlsMenu.SetActive(false);
	}

	public void RestartMatch() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void EndMatch() {
		GameManager.instance.IsPaused = false;
		GameManager.instance.GameOver();
	}
}
