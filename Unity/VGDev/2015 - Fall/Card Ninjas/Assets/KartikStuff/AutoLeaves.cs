using UnityEngine;
using System.Collections;

public class AutoLeaves : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for(int i = 0; i < transform.childCount; i++) {
//			transform.GetChild(i).LookAt(Camera.main.transform);
			transform.GetChild(i).eulerAngles = new Vector3(Random.value*180, 0f, 260 + Random.value*20);
//			transform.GetChild(i).eulerAngles += new Vector3(0f, 0f, Random.value*180 - 360);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
