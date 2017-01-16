using UnityEngine;
using System.Collections;
using Assets.Scripts.Tokens;

public class TokenRotater : MonoBehaviour {

	public float speed = 1f;
	
	// Update is called once per frame
	void Update () {
		if(transform.eulerAngles.y < 180)
			transform.Rotate(Vector3.up*Mathf.Abs((transform.eulerAngles.y+10f)*Time.deltaTime)*speed);
		else
			transform.Rotate(Vector3.up*Mathf.Abs(((360-transform.eulerAngles.y)+10f)*Time.deltaTime)*speed);

		
	}
}
