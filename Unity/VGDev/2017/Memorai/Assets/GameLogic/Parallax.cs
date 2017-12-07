using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [Range(-20, 20)]
    public float scaleFactor = 5;
    public Transform target;
    Vector3 origin;
	// Use this for initialization
	void Start () {
        origin = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        float newXpos = target.position.x - origin.x;
        transform.position = Vector3.Slerp(transform.position, new Vector3(origin.x + (newXpos * scaleFactor * 0.01f), origin.y), 1f);
	}
}
