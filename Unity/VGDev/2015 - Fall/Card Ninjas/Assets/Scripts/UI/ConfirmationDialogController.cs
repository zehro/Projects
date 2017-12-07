using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.Scripts.Util;

/// <summary>
/// Confirmation dialog controller.
/// </summary>
public class ConfirmationDialogController : MonoBehaviour {
	
	// In Game Dialog GameObjects
	#region public members
	public GameObject Confirmation;
	public GameObject ConfirmationSelect;
	
	public GameObject Keep;
	public GameObject KeepSelect;
	
	public EventSystem es;
	public UIHideBehaviour hideBehaviour;

	public delegate void ConfirmKeepAction();
	public ConfirmKeepAction Go;
	public float Timer = 5.0f;
	public Action NotConfirmKeepAction;

	public GameObject PreviousSelected;
	#endregion
	
	#region Monobehaviour
	
	public void Update() {
		if (CustomInput.BoolFreshPress(CustomInput.UserInput.Cancel)) {
			if (hideBehaviour.OnScreen)
				this.DismissDialog();
		}

		if (Keep.activeSelf && this.hideBehaviour.OnScreen) {
			Timer -= Time.deltaTime;
			if (Timer < 0.0f) {
				if (Go != null) Go();
				this.DismissDialog();
			}
		}
	}
	#endregion
	
	#region DialogControls
	public void BringUpConfirm() {
		Keep.SetActive(false);
		Confirmation.SetActive(true);
		
		hideBehaviour.OnScreen = true;

		PreviousSelected = es.currentSelectedGameObject;
		es.SetSelectedGameObject(ConfirmationSelect);
	}

	public void BringUpKeep() {
		//Debug.Log ("KeepDialog");
		Confirmation.SetActive(false);
		Keep.SetActive(true);
		
		hideBehaviour.OnScreen = true;

		PreviousSelected = es.currentSelectedGameObject;
		es.SetSelectedGameObject(KeepSelect);
		//Debug.Log("current after keep: " + es.currentSelectedGameObject);
	}

	public void DismissDialog() {
		if(NotConfirmKeepAction != null) NotConfirmKeepAction();

		hideBehaviour.OnScreen = false;
		this.Timer = 5.0f;
		this.Go = null;
		this.NotConfirmKeepAction = null;
		es.SetSelectedGameObject(PreviousSelected);
		this.PreviousSelected = null;
	}
	#endregion

	#region Button Action Calls
	public void ButtonActionCall() {
		if (Go != null) Go();
		this.DismissDialog();
	}
	#endregion
}
