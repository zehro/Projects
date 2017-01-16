using UnityEngine;
using System.Collections;

public class MainMenuConstructionLine : MonoBehaviour {

	public float minValue = 150f, maxValue = -150f, hSpeed = 10f, vSpeed = 0f;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++) {
			if (hSpeed > 0)
				transform.GetChild(i).localPosition = Vector3.right*minValue + Vector3.right*50*(i+1);
			else
				transform.GetChild(i).localPosition = Vector3.right*minValue - Vector3.right*50*(i+1);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < transform.childCount; i++) {
			transform.GetChild(i).Translate(-transform.right*Time.deltaTime*hSpeed, Space.World);

			if(hSpeed > 0) {
				if(transform.GetChild(i).localPosition.x < maxValue) {
					transform.GetChild(i).gameObject.SetActive(false);
					transform.GetChild(i).localPosition = Vector3.right*minValue;
				} else if(transform.GetChild(i).localPosition.x < minValue) {
					transform.GetChild(i).gameObject.SetActive(true);
					transform.GetChild(i).Translate(Vector3.up*Time.deltaTime*vSpeed, Space.World);
					if(transform.GetChild(i).localPosition.x < 0.5f && transform.GetChild(i).localPosition.x > -0.5f) {
						transform.GetChild(i).gameObject.GetComponent<Animator>().SetTrigger("Start");
					}
				}
			} else {
				if(transform.GetChild(i).localPosition.x > maxValue) {
					transform.GetChild(i).gameObject.SetActive(false);
					transform.GetChild(i).localPosition = Vector3.right*minValue;
				} else if(transform.GetChild(i).localPosition.x > minValue) {
					transform.GetChild(i).gameObject.SetActive(true);
					transform.GetChild(i).Translate(Vector3.up*Time.deltaTime*vSpeed, Space.World);
					if(transform.GetChild(i).localPosition.x < 0.5f && transform.GetChild(i).localPosition.x > -0.5f) {
						transform.GetChild(i).gameObject.GetComponent<Animator>().SetTrigger("Start");
					}
				}
			}
		}
	}
}
