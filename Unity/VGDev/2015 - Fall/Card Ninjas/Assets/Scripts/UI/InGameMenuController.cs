using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Util;
using Assets.Scripts.Managers;

/// <summary>
/// In game menu controller.
/// </summary>
/// <remarks>Handles activating and deactivating pause and game-end screens.</remarks>
public class InGameMenuController : MonoBehaviour {

	// In Game Menu GameObjects
	#region public members
	public GameObject Pause;
	public GameObject PauseSelect;

	public GameObject Win;
	public GameObject WinSelect;

	public GameObject Lose;
	public GameObject LoseSelect;

	public GameObject P1Win;
	public GameObject P1WinSelect;

	public GameObject P2Win;
	public GameObject P2WinSelect;

	public EventSystem es;
	public UIHideBehaviour hideBehaviour;

	public GameObject CurrentDefault;

	public bool IsMultiplayer;
	#endregion

	#region Monobehaviour
 
	public void Update() {
		//Debug.Log(es.currentSelectedGameObject);
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Pause)) {
			if (hideBehaviour.OnScreen)
				this.DismissDialog();
			else
				this.BringUpPause();
		}

		if (hideBehaviour.OnScreenPos.position == hideBehaviour.transform.position && hideBehaviour.OnScreen) {
			if (CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel)) this.DismissDialog();
		}

		if (!this.IsMultiplayer) {
			if (GameManager.Player1Win) {
				this.BringUpWin();
				GameManager.Player1Win = false;
			}
			else if (GameManager.Player1Lose) {
				this.BringUpLose();
				GameManager.Player1Lose = false;
			}
		}
		else {
			if (GameManager.Player1Win) {
				this.BringUpP1Win();
				GameManager.Player1Win = false;
			}
			else if (GameManager.Player1Lose) {
				this.BringUpP2Win();
				GameManager.Player1Lose = false;
			}
		}

		if (GameManager.State == Enums.GameStates.Paused) {
			if (CustomInput.BoolFreshPress(CustomInput.UserInput.Up)) Navigator.Navigate(CustomInput.UserInput.Up, CurrentDefault);
			if (CustomInput.BoolFreshPress(CustomInput.UserInput.Down)) Navigator.Navigate(CustomInput.UserInput.Down, CurrentDefault);
			if (CustomInput.BoolFreshPress(CustomInput.UserInput.Right)) Navigator.Navigate(CustomInput.UserInput.Right, CurrentDefault);
			if (CustomInput.BoolFreshPress(CustomInput.UserInput.Left)) Navigator.Navigate(CustomInput.UserInput.Left, CurrentDefault);
			if (CustomInput.BoolFreshPressDeleteOnRead(CustomInput.UserInput.Accept)) Navigator.CallSubmit();
		}
	}
	#endregion
	
	#region MenuControls
	/// <summary>
	/// Brings up p2 wins window.
	/// </summary>
	public void BringUpP2Win() {
		Win.SetActive(false);
		Lose.SetActive(false);
		P1Win.SetActive(false);
		Pause.SetActive(false);
		P2Win.SetActive(true);
		
		hideBehaviour.OnScreen = true;

		CurrentDefault = P2WinSelect;
		es.SetSelectedGameObject(P2WinSelect);
        GameManager.Pause = true;
    }

	/// <summary>
	/// Brings up p1 wins window.
	/// </summary>
	public void BringUpP1Win() {
		Win.SetActive(false);
		Lose.SetActive(false);
		Pause.SetActive(false);
		P2Win.SetActive(false);
		P1Win.SetActive(true);
		
		hideBehaviour.OnScreen = true;
		
		es.SetSelectedGameObject(P1WinSelect);
        GameManager.Pause = true;
    }

	/// <summary>
	/// Brings up Single Player lose window.
	/// </summary>
	public void BringUpLose() {
		Win.SetActive(false);
		Pause.SetActive(false);
		P1Win.SetActive(false);
		P2Win.SetActive(false);
		Lose.SetActive(true);
		
		hideBehaviour.OnScreen = true;

		CurrentDefault = LoseSelect;
		es.SetSelectedGameObject(LoseSelect);
        GameManager.Pause = true;
    }

	/// <summary>
	/// Brings up Single Player win window.
	/// </summary>
	public void BringUpWin() {
		Pause.SetActive(false);
		Lose.SetActive(false);
		P1Win.SetActive(false);
		P2Win.SetActive(false);
		Win.SetActive(true);
		
		hideBehaviour.OnScreen = true;

		CurrentDefault = WinSelect;
		es.SetSelectedGameObject(WinSelect);
        GameManager.Pause = true;
    }

	/// <summary>
	/// Brings up pause window.
	/// </summary>
	public void BringUpPause() {
		Win.SetActive(false);
		Lose.SetActive(false);
		P1Win.SetActive(false);
		P2Win.SetActive(false);
		Pause.SetActive(true);

		hideBehaviour.OnScreen = true;

		CurrentDefault = PauseSelect;
		es.SetSelectedGameObject(PauseSelect);
        GameManager.Pause = true;
    }

	/// <summary>
	/// Dismisses the current in game dialog.
	/// </summary>
	public void DismissDialog() {
		hideBehaviour.OnScreen = false;
		GameManager.Pause = false;
        es.SetSelectedGameObject(null);
	}
	#endregion
}
