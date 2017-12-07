using UnityEngine;
using System.Collections;

public class ShipLauncher : MonoBehaviour
{
	public GameObject ship;
	public float interval;
	float t;

	void Start()
	{
		t = 0;
	}
	
	void Update()
	{
		t += Time.deltaTime;
		
		if (t > interval)
		{
			t = 0;
			Vector3 randomSpawn = transform.position + new Vector3(Random.Range(-350, 350), Random.Range(-400, 400), Random.Range(-350, 350));
			Quaternion randomRotation = Random.rotation;
			Instantiate(ship, randomSpawn, randomRotation);
		}
	}
}