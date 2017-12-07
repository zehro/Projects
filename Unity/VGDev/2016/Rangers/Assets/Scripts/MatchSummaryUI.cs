using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine.SceneManagement;
using Assets.Scripts.Data;

public class MatchSummaryUI : MonoBehaviour {

	public Text congratsText;
	public Selectable startingButton;

	private float hTimer, vTimer, delay = 0.1f;

	private bool dpadPressed;
	// Use this for initialization
	void Start () {
		if (MatchSummaryManager.winner != PlayerID.None)
			congratsText.text = ProfileManager.instance.GetProfile(MatchSummaryManager.winner).Name + " Wins!";
		else
			congratsText.text = "Tied!";
		Navigator.defaultGameObject = startingButton.gameObject;
	}

	void Update() {
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
	}

	public void Rematch() {
		SceneManager.LoadScene(GameManager.lastLoadedLevel, LoadSceneMode.Single);
	}

	public void MainMenu() {
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
