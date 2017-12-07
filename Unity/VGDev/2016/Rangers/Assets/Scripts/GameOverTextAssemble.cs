using UnityEngine;
using System.Collections;

public class GameOverTextAssemble : MonoBehaviour {

	private Vector3[] originalPositions;
	private Vector3[] originalRotations;
	private float speed = 1f;

	// Use this for initialization
	void Start () {
		originalPositions = new Vector3[transform.childCount];
		originalRotations = new Vector3[transform.childCount];

		for (int i = 1; i < transform.childCount; i++) {
			originalPositions[i] = transform.GetChild(i).position;
			originalRotations[i] = transform.GetChild(i).eulerAngles;
			transform.GetChild(i).position += new Vector3(Random.Range(-200f,200f),Random.Range(-200f,200f));
			transform.GetChild(i).eulerAngles = new Vector3(0f,Random.Range(-180f,180f),0f);
			transform.GetChild(i).localScale = Vector3.zero;
		}
	}
	
	// Update is called once per frame
	void Update () {
		speed += Time.deltaTime*100f;
		for (int i = 1; i < transform.childCount; i++) {
			transform.GetChild(i).position = Vector3.MoveTowards(transform.GetChild(i).position, originalPositions[i], Time.deltaTime*speed);
			transform.GetChild(i).rotation = Quaternion.RotateTowards(Quaternion.Euler(transform.GetChild(i).eulerAngles), Quaternion.Euler(originalRotations[i]), Time.deltaTime*speed/2f);
			transform.GetChild(i).localScale = Vector3.MoveTowards(transform.GetChild(i).localScale, Vector3.one, Time.deltaTime);
		}
		transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.x,transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y+(speed*Time.deltaTime));
	}
}
