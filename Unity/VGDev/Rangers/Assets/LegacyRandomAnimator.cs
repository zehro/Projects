using UnityEngine;
using System.Collections;

public class LegacyRandomAnimator : MonoBehaviour {

	private float timeOffset;

	// Use this for initialization
	void Start () {
		timeOffset = Random.value*2f;
	}
	
	// Update is called once per frame
	void Update () {
		timeOffset -= Time.deltaTime;
		if(timeOffset <= 0) {
			GetComponent<Animation>().Play();
			Destroy(this);
		}
	}
}
