using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugRandomThings : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
//		transform.localScale = new Vector3(LevelManager.instance.Timer/LevelManager.instance.Tempo,1f,1f);
		if(LevelManager.instance.PerfectTiming() == 0) transform.GetComponent<Image>().color = Color.green;
		else transform.GetComponent<Image>().color = Color.clear;
//		transform.GetComponent<Image>().color = new Color(0,1,0,1-LevelManager.instance.Timer/LevelManager.instance.Tempo);
	}
}
