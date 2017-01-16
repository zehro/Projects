using UnityEngine;
using System.Collections;

public class platMove : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.right * Mathf.Cos(Time.time) * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.transform.parent = null;
        }
    }
}