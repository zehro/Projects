using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class IntroScene : MonoBehaviour {
    public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
            endScene();
        }
	}
    public void updateText(string str) {
        text.text = str;
    }

    public void changeTextColor(string hex) {
        Color col = text.color;
        ColorUtility.TryParseHtmlString(hex, out col);
        text.color = col;
    }
    public void endScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
