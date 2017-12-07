using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class EnvironmentQualitySettings : MonoBehaviour {

	public static EnvironmentQualitySettings instance;

	public int qualityLevel = 1;

	void OnEnable() {
		instance = this;
		qualityLevel = Mathf.Clamp(qualityLevel,1,4);
	}

	// Use this for initialization
	void Start () {
		switch(qualityLevel) {
		case 1:
			Camera.main.GetComponent<DepthOfField>().enabled = false;
			Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = false;
			Camera.main.GetComponent<SunShafts>().enabled = false;
			break;
		case 2:
			Camera.main.GetComponent<DepthOfField>().enabled = true;
			Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = false;
			Camera.main.GetComponent<SunShafts>().enabled = false;
			break;
		case 3:
			Camera.main.GetComponent<DepthOfField>().enabled = true;
			Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = true;
			Camera.main.GetComponent<SunShafts>().enabled = false;
			break;
		case 4:
			Camera.main.GetComponent<DepthOfField>().enabled = true;
			Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = false;
			Camera.main.GetComponent<SunShafts>().enabled = true;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
