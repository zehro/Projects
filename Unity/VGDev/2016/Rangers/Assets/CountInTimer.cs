using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Data;

public class CountInTimer : MonoBehaviour {

	private float startingTimeCountdown;
	private float startingSpeed, startingMagnitude, startingDuration;

	void Start() {
		startingTimeCountdown = GameManager.instance.countInTimer;
		startingSpeed = Camera.main.GetComponent<PerlinShake>().speed;
		startingMagnitude = Camera.main.GetComponent<PerlinShake>().magnitude;
		startingDuration = Camera.main.GetComponent<PerlinShake>().duration;
		Camera.main.GetComponent<PerlinShake>().duration = startingTimeCountdown;
//		Camera.main.GetComponent<PerlinShake>().PlayShake();
	}

	// Update is called once per frame
	void Update () {
		transform.GetChild(2).GetComponent<Text>().text = GameManager.instance.countInTimer.ToString("F3");
		transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(new Vector2(500,19), new Vector2(-400,19), 1 - GameManager.instance.countInTimer/startingTimeCountdown);
		transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(new Vector2(500,19), new Vector2(-400,19), 1 - GameManager.instance.countInTimer/startingTimeCountdown);
//		Camera.main.GetComponent<PerlinShake>().speed += Time.deltaTime*8f;
//		Camera.main.GetComponent<PerlinShake>().magnitude += Time.deltaTime/3f;

		Camera.main.fieldOfView -= Time.deltaTime*4f;
		Camera.main.transform.FindChild("BlurCamera").GetComponent<Camera>().fieldOfView -= Time.deltaTime*4f;

		if(GameManager.instance.countInTimer < 0) {
			Camera.main.GetComponent<PerlinShake>().speed = startingSpeed;
			Camera.main.GetComponent<PerlinShake>().magnitude = startingMagnitude;
			Camera.main.GetComponent<PerlinShake>().duration = startingDuration;
//			Camera.main.fieldOfView = 60f;
//			Camera.main.transform.FindChild("BlurCamera").GetComponent<Camera>().fieldOfView = 60f;
			Destroy(this.gameObject);
		}
	}
}
