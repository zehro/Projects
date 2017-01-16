using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleVisible : MonoBehaviour {

	public string textCheckValue;
	public GameObject obj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowHide(Text textUI) {
		if(textUI.text.Equals(textCheckValue)) {
			obj.SetActive(true);
		} else {
			obj.SetActive(false);
		}
	}
}
