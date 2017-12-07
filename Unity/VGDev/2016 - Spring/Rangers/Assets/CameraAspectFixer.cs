using UnityEngine;
using System.Collections;

public class CameraAspectFixer : MonoBehaviour {

	private Texture2D blank;
	private bool widescreen, GUIDRAWN;

	private Rect view;

	// Use this for initialization
	void Start () {
		float currentRatio = Screen.width/(float)Screen.height;
		float expectedRatio = 1024f/576f; //1.777778

		if(currentRatio > expectedRatio) { //pillar boxes
			float fix = Screen.height*expectedRatio;
			float widthDiff = fix/Screen.width;
			view = new Rect((1-widthDiff)/2f,0,widthDiff,1);
		} else if (currentRatio < expectedRatio) { // widescreen
			float fix = Screen.width/expectedRatio;
			float heightDiff = fix/Screen.height;
			view = new Rect(0f,(1-heightDiff)/2f,1,heightDiff);
			widescreen = true;
		} else {
			view = GetComponent<Camera>().rect;
		}

		blank = new Texture2D(1,1);
		blank.SetPixel(1,1,Color.black);
		blank.Apply();
	}

	void Update() {
		if(GUIDRAWN) {
			GetComponent<Camera>().rect = view;
			Destroy(this);
		}
	}

	void OnGUI() {
		if(widescreen) {
			GUI.DrawTexture(new Rect(0,0,Screen.width, view.y*Screen.height),blank);
			GUI.DrawTexture(new Rect(0,Screen.height - (view.y*Screen.height),Screen.width, view.y*Screen.height),blank);
		} else {
			GUI.DrawTexture(new Rect(0,0, view.x*Screen.width, Screen.height),blank);
			GUI.DrawTexture(new Rect(Screen.width - (view.x*Screen.width),0,view.x*Screen.width, Screen.height),blank);
		}
		GUIDRAWN = true;
	}
	
	// Update is called once per frame
//	void Update () {
//
//	}
}
