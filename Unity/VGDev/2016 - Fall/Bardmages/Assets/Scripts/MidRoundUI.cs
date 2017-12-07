using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MidRoundUI : MonoBehaviour {

	public RectTransform[] playerScoreBoxes;

	private static int playerWinner;
	private static bool updateUI;

	void Start() {
		int i = 0;
		for(; i < Assets.Scripts.Data.RoundHandler.Instance.Bards.Length; i++) {
			for(int j = 0; j < 5; j++) { 
				playerScoreBoxes[i].GetChild(j).GetChild(0).GetComponent<Image>().CrossFadeAlpha(0f,2f,true);
			}
		}
		for(; i < 4; i++) {
			playerScoreBoxes[i].parent.gameObject.SetActive(false);
		}
		Assets.Scripts.Data.RoundHandler.Instance.RegisterUI(ShowUI);
	}

	void Update() {
//		for(int i = 1; i < 5; i++) {
//			if(Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode),"Alpha"+i))) {
//				Assets.Scripts.Data.RoundHandler.Instance.AddScore((PlayerID)i);
//				AddScore(i-1);
//			}
//		}
		if(updateUI && playerWinner != -1) {
			StartCoroutine(OpenUIAnim());
			updateUI = false;
		}
	}

	public static void ShowUI(int player) {
		playerWinner = player;
		updateUI = true;
	}

	public void AddScore(int player) {
		StartCoroutine(AddScoreAnim(player));
	}

	private IEnumerator OpenUIAnim() {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime;

			GetComponent<CanvasGroup>().alpha = timer;

			yield return new WaitForEndOfFrame();
		}

		AddScore(playerWinner);

		yield return null;
	}

	private IEnumerator CloseUIAnim() {
		float timer = 0f;

		while(timer < 1f) {
			timer += Time.deltaTime;

			GetComponent<CanvasGroup>().alpha = 1-timer;

			yield return new WaitForEndOfFrame();
		}

		Assets.Scripts.Data.RoundHandler.Instance.BeginRound();

		yield return null;
	}

	private IEnumerator AddScoreAnim(int player) {
		float timer = 0f;
		playerScoreBoxes[player].GetChild(Assets.Scripts.Data.RoundHandler.Instance.Scores[player]-1).GetChild(0).GetComponent<Image>().CrossFadeAlpha(1f,2f,true);
		RectTransform tempRect = playerScoreBoxes[player].GetChild(Assets.Scripts.Data.RoundHandler.Instance.Scores[player]-1).GetChild(0).GetComponent<RectTransform>();
		while (timer < 1f) {
			timer += Time.deltaTime;

			tempRect.localScale = Vector3.Lerp(Vector3.one*2f,Vector3.one,timer);
			tempRect.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(360f, 0f, timer));

			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(2f);

		StartCoroutine(CloseUIAnim());

		yield return null;
	}
}
