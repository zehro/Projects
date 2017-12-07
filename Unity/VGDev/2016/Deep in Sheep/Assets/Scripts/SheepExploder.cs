using UnityEngine;
using System.Collections;

public class SheepExploder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, GetComponent<ParticleSystem>().duration);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
