using UnityEngine;
using System.Collections;

public class UIHideBehaviour : MonoBehaviour {

	public Transform OnScreenPos;
	public Transform OffScreenPos;

	public bool OnScreen;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (OnScreen) {
			this.transform.position = Vector3.MoveTowards(this.transform.position, this.OnScreenPos.position, 1000*Time.deltaTime);
		}
		else {
			this.transform.position = Vector3.MoveTowards(this.transform.position, this.OffScreenPos.position, 1000*Time.deltaTime);
		}
	}
}
