using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuPlayerInfoBlock : MonoBehaviour {

	public PlayerID playerID;

	private Text tagText;
	//private Image tagBG;
	private GameObject pressToOpen;
	private GameObject pressToJoin;
	private GameObject nameCreator;
	private GameObject nameCreatorInstructions;
	private GameObject primaryColorHolder, secondaryColorHolder;
	private Image playerNumIndicator;

	private bool menuOpen = false;
	private Vector3 startingPosition;


	// Use this for initialization
	void OnEnable () {
		tagText = transform.FindChild("TagBox").FindChild("PlayerTag").GetComponent<Text>();
		//tagBG = transform.FindChild("TagBox").GetComponent<Image>();
		pressToJoin = transform.FindChild("PressToJoin").gameObject;
		pressToOpen = transform.FindChild("StartToOpen").gameObject;
		nameCreator = transform.FindChild("NameCreator").gameObject;
		primaryColorHolder = transform.FindChild("Customization").FindChild("PrimaryColorSelector").FindChild("PrimaryColorHolder").gameObject;
		secondaryColorHolder = transform.FindChild("Customization").FindChild("SecondaryColorSelector").FindChild("SecondaryColorHolder").gameObject;
		nameCreatorInstructions = transform.FindChild("Instructions").gameObject;
		playerNumIndicator = transform.FindChild("Player" + (int)playerID + "Indicator").GetChild((int)playerID - 1).GetComponent<Image>();
		ShowPressToJoinGraphic();
		startingPosition = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		if(nameCreator.activeInHierarchy) {
			if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start,playerID)
				&& tagText.text.Length > 0 && tagText.text.ToCharArray()[tagText.text.Length-1] != ' ') {
				ProfileManager.instance.AddProfile(new ProfileData(tagText.text), playerID);
				HideNameCreator();
				tagText.color = ProfileManager.instance.GetProfile(playerID).SecondaryColor;
				tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).PrimaryColor;
				playerNumIndicator.color = Color.white;
			}
		}
		else if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, playerID)) {
			TogglePlayerMenu();
		}
		if(menuOpen) {
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, startingPosition + new Vector3(34,150,0),Time.deltaTime*(Vector3.Distance(transform.localPosition,startingPosition)*20f+2f));
			if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,playerID) != 0) {
				float h = 0f;
				float s = 0f;
				float v = 0f;
				Color.RGBToHSV(ProfileManager.instance.GetProfile(playerID).PrimaryColor, out h, out s, out v);

				h += Time.deltaTime*ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,playerID)*0.5f;
				if(h > 1)  {
					h = 0;
				}
				if(h < 0)  {
					h = 0.99f;
				}

				Color final = Color.HSVToRGB(h, 0.75f, 0.75f);
				ProfileManager.instance.GetProfile(playerID).PrimaryColor = final;
			}
			if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,playerID) != 0) {
				float h = 0f;
				float s = 0f;
				float v = 0f;
				Color.RGBToHSV(ProfileManager.instance.GetProfile(playerID).SecondaryColor, out h, out s, out v);
				h += Time.deltaTime*ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,playerID)*0.5f;
				if(h > 1)  {
					h = 0;
				}
				if(h < 0)  {
					h = 0.99f;
				}

				Color final = Color.HSVToRGB(h, 0.75f, 0.75f);
				ProfileManager.instance.GetProfile(playerID).SecondaryColor = final;
			}
			primaryColorHolder.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).PrimaryColor;
			secondaryColorHolder.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).SecondaryColor;
			tagText.color = ProfileManager.instance.GetProfile(playerID).SecondaryColor;
			tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).PrimaryColor;
		} else {
			transform.localPosition = Vector3.MoveTowards(transform.localPosition, startingPosition,Time.deltaTime*(Vector3.Distance(transform.localPosition,startingPosition + new Vector3(34,150,0))*20f+2f));
		}
	}

	public void PlayerAdded() {
		if(ProfileManager.instance.ProfileExists(playerID)) {
			SetTag(ProfileManager.instance.GetProfile(playerID).Name);
			tagText.color = ProfileManager.instance.GetProfile(playerID).SecondaryColor;
			tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).PrimaryColor;
			HidePressToJoinGraphic();

			playerNumIndicator.color = Color.white;
		} else {
			ShowNameCreator();
		}
	}

	/// <summary>
	/// Adds an AI player to the block.
	/// </summary>
	public void AIAdded() {
		SetTag("AI " + (int)playerID);
		ProfileManager.instance.AddProfile(new ProfileData(tagText.text), playerID);
		ProfileManager.instance.GetProfile(playerID).SetAI();
		HidePressToJoinGraphic(false);
		playerNumIndicator.color = Color.white;
		tagText.color = Color.red;
		tagText.transform.parent.GetComponent<Image>().color = Color.black;
	}

	/// <summary>
	/// Removes the player from the block.
	/// </summary>
	public void PlayerRemoved() {
		ProfileManager.instance.RemoveProfile(playerID);
		menuOpen = false;
		nameCreatorInstructions.SetActive(false);
	}

	/// <summary>
	/// Checks if the block is unoccupied.
	/// </summary>
	/// <returns>Whether the block is unoccupied.</returns></returns>
	public bool IsOpen() {
		return playerNumIndicator.color == Color.red;
	}

	/// <summary>
	/// Reverts the block to an unoccupied state.
	/// </summary>
	public void SetOpen() {
		SetTag("####");
		ShowPressToJoinGraphic();
		playerNumIndicator.color = Color.red;
	}

	public void TogglePlayerMenu() {
		menuOpen = !menuOpen;
		pressToOpen.SetActive(!pressToOpen.activeSelf);
	}

	/// <summary>
	/// Shows the join graphic on the block.
	/// </summary>
	public void ShowPressToJoinGraphic() {
		pressToOpen.SetActive(false);
		pressToJoin.SetActive(true);
	}

	public void HidePressToJoinGraphic(bool showOpen = true) {
		pressToOpen.SetActive(showOpen);
		pressToJoin.SetActive(false);
	}

	/// <summary>
	/// Gets the text currently displayed on the block.
	/// </summary>
	/// <returns>The text currently displayed on the block.</returns>
	public string GetTag() {
		return tagText.text;
	}

	public void SetTag(string text) {
		tagText.text = text;
		if(text.Contains("AI")) {
			playerNumIndicator.color = Color.white;
			tagText.color = Color.red;
			tagText.transform.parent.GetComponent<Image>().color = Color.black;
		} else if(!text.Contains("#")) {
			if(ProfileManager.instance.ProfileExists(playerID)) {
				tagText.color = ProfileManager.instance.GetProfile(playerID).SecondaryColor;
				tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(playerID).PrimaryColor;
			}
		} else {
			playerNumIndicator.color = Color.red;
			tagText.color = Color.red;
			tagText.transform.parent.GetComponent<Image>().color = Color.black;
		}
	}

	public void ShowNameCreator() {
		nameCreator.SetActive(true);
		pressToJoin.SetActive(false);
		nameCreatorInstructions.SetActive(true);
		SetTag("");
	}

	public void HideNameCreator() {
		nameCreator.SetActive(false);
		pressToOpen.SetActive(true);
		nameCreatorInstructions.SetActive(false);
	}

	/// <summary>
	/// Checks if options are being selected in the block.
	/// </summary>
	/// <returns>Whether options are being selected in the block.</returns>
	public bool Occupied() {
		return nameCreator.activeInHierarchy || menuOpen;
	}

	/// <summary>
	/// Resets the position of the color menu.
	/// </summary>
	public void ResetMenu() {
		transform.localPosition = startingPosition;
		menuOpen = false;
	}
}
