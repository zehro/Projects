using UnityEngine;
using System.Collections;

public class CycleChildCanvasAlpha : MonoBehaviour {

	private int index;
	private float changeTime = 3f;
	private float curTime = 3f;

	// Use this for initialization
	void Start () {
		for(int i = 1; i < transform.childCount; i++) {
			transform.GetChild(i).GetComponent<CanvasGroup>().alpha = 0f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		curTime -= Time.deltaTime;
		if(curTime <= 0) {
			curTime = changeTime;
			index = (index+1)%transform.childCount;
		}
		for(int i = 0; i < transform.childCount; i++) {
			if(index == i) {
				transform.GetChild(i).GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(transform.GetChild(i).GetComponent<CanvasGroup>().alpha,1,Time.deltaTime);
			} else {
				transform.GetChild(i).GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(transform.GetChild(i).GetComponent<CanvasGroup>().alpha,0,Time.deltaTime);
			}
		}
	}
}
