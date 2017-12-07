using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Util;

public class MainMenuController : MonoBehaviour {
	
	public SoundPlayer sp;

	public GameObject buttonParent;
	public GameObject goalButtonParent;

	public GameObject[] buttons;
	public GameObject[] goalButtons;

	public bool isCredits;

	// TODO: write custom editor for this script that allows you to specify if cards are needed
	public GameObject cardParent;
	public GameObject goalCardParent;

	private GameObject[] cards;
	private GameObject[] goalCards;

	void Start () {
		// First find all the appropriate buttons
		goalButtonParent.SetActive(true);
		Button[] temp = buttonParent.GetComponentsInChildren<Button>();
		buttons = new GameObject[temp.Length];
		for (int i = 0; i < temp.Length; i++) {
			buttons[i] = temp[i].gameObject;
		}

		temp = goalButtonParent.GetComponentsInChildren<Button>();
		goalButtons = new GameObject[temp.Length];
		for (int i = 0; i < temp.Length; i++) {
			goalButtons[i] = temp[i].gameObject;
		}
		goalButtonParent.SetActive(false);

		// Now do the cards if they exist
		if (cardParent) {
			goalCardParent.SetActive(true);
			Image[] temp1 = cardParent.GetComponentsInChildren<Image>();
			cards = new GameObject[temp1.Length];
			for (int i = 0; i < temp1.Length; i++) {
				cards[i] = temp1[i].gameObject;
			}
			
			temp1 = goalCardParent.GetComponentsInChildren<Image>();
			goalCards = new GameObject[temp1.Length];
			for (int i = 0; i < temp1.Length; i++) {
				goalCards[i] = temp1[i].gameObject;
			}
			goalCardParent.SetActive(false);
		}
	}

	// Move buttons to proper placement if they are out of place
	void Update () {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].transform.position = Vector3.MoveTowards(buttons[i].transform.position, 
                                            goalButtons[i].transform.position, Time.deltaTime * 1000.0f);
		}

		if (cardParent) {
			for (int i = 0; i < cards.Length; i++) {
				cards[i].transform.position = Vector3.MoveTowards(cards[i].transform.position, 
				                                                  goalCards[i].transform.position, Time.deltaTime * 1000.0f);
				cards[i].transform.eulerAngles = goalCards[i].transform.eulerAngles;
			}
		}
	}

	public void MoveButtons() {
		// TODO: function that lets me not have all this pointer switching stuff be hard coded twice.
		// to lazy to fix right now and don't think it'll matter.

		// Reassign all button references to proper place after navigation, then cards if applicable
		if(CustomInput.BoolFreshPress(CustomInput.UserInput.Down) || this.isCredits && CustomInput.BoolFreshPress(CustomInput.UserInput.Left)) {
			GameObject temp = buttons[0];
			for (int i = 1; i < buttons.Length; i++) {
				buttons[i-1] = buttons[i];
			}
			buttons[buttons.Length-1] = temp;

			temp.transform.position = goalButtons[buttons.Length-1].transform.position;
			
			if (cardParent) {
				GameObject tempCard = cards[0];
				for (int i = 1; i < cards.Length; i++) {
					cards[i-1] = cards[i];
					cards[i-1].transform.SetSiblingIndex(i-1);
				}
				cards[cards.Length-1] = tempCard;
			}
		}
		else if(CustomInput.BoolFreshPress(CustomInput.UserInput.Up) || this.isCredits && CustomInput.BoolFreshPress(CustomInput.UserInput.Right)) {
			GameObject temp = buttons[buttons.Length-1];
			for (int i = buttons.Length-2; i >= 0; i--) {
				buttons[i+1] = buttons[i];
			}
			buttons[0] = temp;

			temp.transform.position = goalButtons[0].transform.position;

			if (cardParent) {
				GameObject tempCard = cards[cards.Length-1];
				for (int i = cards.Length-2; i >= 0; i--) {
					cards[i+1] = cards[i];
					cards[i+1].transform.SetSiblingIndex(i+1);
				}
				cards[0] = tempCard;
			}
		}

		// Play sound effect
		sp.PlaySong(1);
	}
}
