using UnityEngine;
using System.Collections;

public class BowString : MonoBehaviour {

	public Transform handPos;

	private Animator playerAnim;

	private LineRenderer stringLine;

	// Use this for initialization
	void Start () {
		playerAnim = transform.root.GetComponent<Animator>();
		stringLine = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if(playerAnim.enabled) {
			stringLine.GetComponent<LineRenderer>().SetPosition(0,Vector3.right*0.5f);
			stringLine.GetComponent<LineRenderer>().SetPosition(2,-Vector3.right*0.5f);
			if (playerAnim.GetCurrentAnimatorStateInfo(1).IsName("Aim")) {
				Vector3 localHandPos = transform.InverseTransformPoint(handPos.position);
				stringLine.GetComponent<LineRenderer>().SetPosition(1,localHandPos);
			} else {
				stringLine.GetComponent<LineRenderer>().SetPosition(1,Vector3.zero);
			}
		} else {
			stringLine.GetComponent<LineRenderer>().SetPosition(0,Vector3.zero);
			stringLine.GetComponent<LineRenderer>().SetPosition(1,Vector3.zero);
			stringLine.GetComponent<LineRenderer>().SetPosition(2,Vector3.zero);
		}
	
	}
}
