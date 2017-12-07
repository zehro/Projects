using UnityEngine;
using System.Collections;

public class WinnerUI : MonoBehaviour {

	public GameObject[] playerModels;
	int winnerIndex;

	// Use this for initialization
	void Start () {
		for(int i = Assets.Scripts.Data.Data.Instance.NumOfPlayers; i < 4; i++) {
			playerModels[i].SetActive(false);
		}
		int bestScore = 0, index = -1;
		for(int i = 0; i < Assets.Scripts.Data.Data.Instance.FinalScores.Length; i++) {
			if(Assets.Scripts.Data.Data.Instance.FinalScores[i] > bestScore) {
				bestScore = Assets.Scripts.Data.Data.Instance.FinalScores[i];
				index = i;
			}
		}
		GetComponent<Animator>().SetInteger("PlayerWin", index + 1);
		winnerIndex = index+1;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Return) ||
			ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, (PlayerID)(winnerIndex))) {
			Assets.Scripts.Data.Data.Instance.loadScene("MenuTest");
		}
	}
}
