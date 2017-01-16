using UnityEngine;
using System.Collections;

public class CameraFlashingScript : MonoBehaviour
{

	public float flashMin, flashMax;
	public int numFlash;
	public float flashBrightness = 8f;

	private int actualNumFlash;
	private float flashTimer;


	// Use this for initialization
	void Start () 
	{
		actualNumFlash = ((int)(Random.value*numFlash));
	}
	
	// Update is called once per frame
	void Update ()
	{
		flashTimer -= Time.deltaTime;
		if(flashTimer <= 0) {
			for(int i = 0; i < actualNumFlash; i++) 
			{
				transform.GetChild((int)(Random.value*transform.childCount)).GetComponent<LensFlare>().brightness = flashBrightness;
			}
			actualNumFlash = ((int)(Random.value*numFlash));
			flashTimer = Random.Range(flashMin, flashMax);
		}
		for(int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild(i).GetComponent<LensFlare>().brightness -= Time.deltaTime*flashBrightness;
		}
	}
}
