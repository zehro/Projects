using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DelayedTextCopy : MonoBehaviour {

	public float delay;
	private float delayTimer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(delayTimer > 0) {
			delayTimer -= Time.deltaTime;
		} else if (delayTimer != -100) {
			delayTimer = -100;
			GetComponent<Text>().text = transform.parent.GetComponent<Text>().text;
			GetComponent<Text>().color = new Color(transform.parent.GetComponent<Text>().color.r,transform.parent.GetComponent<Text>().color.g,transform.parent.GetComponent<Text>().color.b,GetComponent<Text>().color.a);
		}
	}

	public void CopyText() {
		delayTimer = delay;
	}
}
