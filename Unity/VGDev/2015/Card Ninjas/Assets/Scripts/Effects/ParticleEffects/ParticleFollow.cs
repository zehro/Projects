using UnityEngine;
using System.Collections;

public class ParticleFollow : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
		if (target) 
			transform.position = target.transform.position;	
		this.GetComponent<ParticleSystem>().Stop();	
		this.GetComponent<ParticleSystem>().Play();
	}

	// Update is called once per frame
	void Update () {
		if (target) 
			transform.position = target.transform.position;
		else {
			this.GetComponent<ParticleSystem>().Stop();

			if (!this.GetComponent<ParticleSystem>().IsAlive()) {
				Destroy(this);
			}
		}
	}
}
