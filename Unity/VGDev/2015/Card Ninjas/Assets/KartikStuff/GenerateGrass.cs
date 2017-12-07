using UnityEngine;
using System.Collections;

public class GenerateGrass : MonoBehaviour
{

	public int thickness;
	public int xwidth,zwidth;

	void Start ()
    {
		int numGrass = thickness*EnvironmentQualitySettings.instance.qualityLevel;
		for(int i = 0; i < numGrass; i++)
        {
			GameObject temp = (GameObject)GameObject.Instantiate(this.gameObject, transform.position + new Vector3(-Random.value*xwidth,0f, Random.value*zwidth), transform.rotation);
			Destroy(temp.GetComponent<GenerateGrass>());
			temp.transform.localScale *= 20f/(Vector3.Distance(temp.transform.position,Camera.main.transform.position));
		}
	}

	void OnDrawGizmos()
    {
		Gizmos.DrawWireCube(transform.position + new Vector3(-xwidth/2f,0f,zwidth/2f), new Vector3(xwidth,1, zwidth));
	}
}
