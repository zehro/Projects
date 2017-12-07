using UnityEngine;
using System.Collections;

public class AddCPUButton : MainMenuButton {

	[HideInInspector]
	public bool opened = false;

	private bool recentChange = false;
	private Coroutine flipAndChange;
	private Color initialColor;
	private static bool animating;

    protected override void HandlePressed () {
		if(animating) return;
        base.HandlePressed ();
		if(flipAndChange != null) StopCoroutine(flipAndChange);
		flipAndChange = StartCoroutine(FlipAndChange());
		animating = true;
    }

	public void Animate() {
		if(animating) return;
		if(flipAndChange != null) StopCoroutine(flipAndChange);
		flipAndChange = StartCoroutine(FlipAndChange());
		animating = true;
	}

	void OnEnable() {
		GetComponent<BoxCollider>().enabled = true;
	}

	private IEnumerator FlipAndChange() {
        float timer = 0f;
		GetComponent<BoxCollider>().enabled = false;
		if(initialColor == null && !opened) initialColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
		if(recentChange) {
			recentChange = false;
			yield return null;
		} else {
			recentChange = true;
			opened = !opened;
		}
        while (timer < 1f) {
            timer += Time.deltaTime*2f;
			if(opened) {
				transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(transform.localEulerAngles.z, 180f, timer));
				transform.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(initialColor, Color.red, timer);
			} else {
				transform.localEulerAngles = new Vector3(0f,0f,Mathf.Lerp(transform.localEulerAngles.z, 0f, timer));
				transform.GetChild(0).GetComponent<Renderer>().material.color = Color.Lerp(Color.red, initialColor, timer);
			}
            yield return new WaitForEndOfFrame();
        }
		recentChange = false;
		GetComponent<BoxCollider>().enabled = true;
		animating = false;
		yield return null;
    }
}