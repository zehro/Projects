using UnityEngine;
using System.Collections;

public class BeatAffectedLight : MonoBehaviour {

	private float initialIntensity;

	public float beatOffset;

	// Use this for initialization
	void Start () {
		initialIntensity = GetComponent<Light>().intensity;
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Light>().intensity = initialIntensity * LevelManager.instance.BeatValue(beatOffset);
	}
}
