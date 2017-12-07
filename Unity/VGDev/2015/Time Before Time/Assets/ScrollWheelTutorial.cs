﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollWheelTutorial : MonoBehaviour {

	public GameObject aim;

//	private float angle = 90f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.GetSiblingIndex() != 0) {
			GetComponent<CanvasGroup>().alpha = 0;
		} else {
			GetComponent<CanvasGroup>().alpha = Mathf.MoveTowards(GetComponent<CanvasGroup>().alpha, 1f, Time.deltaTime/2f);
		}


		if(aim.GetComponent<PhysicsModifyable>().mass > 0 || aim.GetComponent<PhysicsModifyable>().charge != 0) {
			Destroy(this.gameObject);
		}

//		transform.GetChild(0).right = (Vector2)((-RectTransformUtility.WorldToScreenPoint(Camera.main,transform.GetChild(0).position)) + (Vector2)Camera.main.WorldToScreenPoint(aim.transform.position));

//		Camera.main.WorldToScreenPoint()

//		transform.GetChild(0).LookAt(aim.transform);
//		transform.GetChild(0).eulerAngles = new Vector3(0,0,transform.GetChild(0).eulerAngles.z);

//		float angle = Vector2.Angle(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)) - Camera.main.WorldToScreenPoint(aim.transform.position),
//		                            (Vector2)Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)) - (Vector2)RectTransformUtility.WorldToScreenPoint(Camera.main,transform.GetChild(0).position));
////		if(angle > 0) {
////			transform.GetChild(0).RotateAround(transform.position, transform.forward, Time.deltaTime*20f);
////		}
//		Debug.Log(angle);
//		Debug.DrawLine(Camera.main.ScreenToWorldPoint(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f))), aim.transform.position);
//		Debug.Log(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)));
//		Debug.Log(Camera.main.WorldToScreenPoint(aim.transform.position));
//		Debug.Log(Vector2.Angle(Camera.main.WorldToScreenPoint(aim.transform.position), Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f))));
	}
}
