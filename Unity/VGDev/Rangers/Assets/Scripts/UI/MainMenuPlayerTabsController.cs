using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.UI;

public class MainMenuPlayerTabsController : MonoBehaviour {

	private MainMenuPlayerInfoBlock[] infoBlocks = new MainMenuPlayerInfoBlock[4];

	/// <summary> The timer to delay adding AI players. </summary>
	private float aiAddTimer;
	/// <summary> Delay before another AI player can be added. </summary>
	private float AIADDDELAY = 0.15f;

	// Use this for initialization
	void OnEnable () {
		for(int i = 0; i < infoBlocks.Length; i++) {
			infoBlocks[i] = transform.GetChild(i).GetComponent<MainMenuPlayerInfoBlock>();
		}

		for (int i = 0; i < ControllerManager.instance.NumPlayers; i++) {
			if(ControllerManager.instance.IsAI((PlayerID)i+1)) infoBlocks[i].SetTag("AI " + (i + 1));
			else infoBlocks[i].PlayerAdded();
		}



	}
	
	// Update is called once per frame
	void Update () {
		if(ControllerManager.instance.AddPlayer(ControllerInputWrapper.Buttons.Start)) {
			if (ControllerManager.instance.CountAI() == ControllerManager.instance.NumPlayers - 1) {
				for (int i = Mathf.Min(3, ControllerManager.instance.NumPlayers - 1); i > 0; i--) {
					infoBlocks[i].SetTag("AI " + (i + 1));
					infoBlocks[i].HidePressToJoinGraphic(false);
				}
				ProfileManager.instance.ShiftProfiles();
				infoBlocks[0].PlayerAdded();
			} else {
				infoBlocks[ControllerManager.instance.NumPlayers - 1].PlayerAdded();
			}
		}
		int removedPlayer = ControllerManager.instance.AllowPlayerRemoval(ControllerInputWrapper.Buttons.Back);
		if(removedPlayer >= 1) {
			RemovePlayer(removedPlayer - 1);
		}
		if(aiAddTimer <= 0) {
			if(ControllerManager.instance.AddAI(ControllerInputWrapper.Buttons.RightBumper)) {
				infoBlocks[ControllerManager.instance.NumPlayers - 1].AIAdded();
				aiAddTimer = AIADDDELAY;
			}
			removedPlayer = ControllerManager.instance.AllowAIRemoval(ControllerInputWrapper.Buttons.LeftBumper);
			if(removedPlayer >= 1) {
				RemovePlayer(removedPlayer - 1);
				aiAddTimer = AIADDDELAY;
			}
		}

		if(ControllerManager.instance.NumPlayers == 0) {
			foreach (MainMenuPlayerInfoBlock block in infoBlocks) {
				block.ResetMenu();
			}
			MenuManager.instance.CallSplash();
		}

		aiAddTimer -= Time.deltaTime;
	}

	/// <summary>
	/// Gets the block with the given index.
	/// </summary>
	/// <returns>The block with the given index./returns>
	/// <param name="blockIndex">The index of the block to get.</param>
	public MainMenuPlayerInfoBlock GetBlock(int blockIndex) {
		return infoBlocks[blockIndex];
	}

	/// <summary>
	/// Unregisters a player.
	/// </summary>
	/// <param name="blockIndex">The index of the block to unregister.</param></param>
	private void RemovePlayer(int blockIndex) {
		for (int i = blockIndex; i <= infoBlocks.Length; i++) {
			if (i == infoBlocks.Length - 1 || infoBlocks[i + 1].IsOpen()) {
				infoBlocks[i].SetOpen();
				break;
			} else {
				string newTag;
				if (ControllerManager.instance.IsAI((PlayerID)(i + 1))) {
					newTag = "AI " + (i + 1);
				} else {
					newTag = infoBlocks[i + 1].GetTag();
					if(!newTag.Contains("#")) {
						infoBlocks[i].PlayerAdded();
					}
				}
				infoBlocks[i].SetTag(newTag);
			}
		}
		infoBlocks[blockIndex].PlayerRemoved();
	}
}
