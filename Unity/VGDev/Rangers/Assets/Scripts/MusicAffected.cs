using UnityEngine;
using System.Collections;

public class MusicAffected : MonoBehaviour {

	public Vector3 translate, scale, rotate;
	public float sampleScale;
	public AudioSource source;

	private Vector3 initialPos, initialScale, initialRotate;

	// Use this for initialization
	void Start () {
		initialPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(source.isPlaying && source.time > 0.1f) {
			float[] samples = new float[10];
			source.clip.GetData(samples,source.timeSamples);
			float totalSample = 0f;
			for(int i = 0; i < samples.Length; i++) {
				totalSample += samples[i];
			}
			totalSample *= sampleScale;
			transform.position = initialPos + new Vector3(totalSample*translate.x,totalSample*translate.y, totalSample*translate.z);
		}
	}
}
