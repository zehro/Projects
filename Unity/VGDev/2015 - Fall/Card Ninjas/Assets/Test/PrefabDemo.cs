using UnityEngine;
using System.Collections;

public class PrefabDemo : MonoBehaviour {

	public GameObject enemy;

	// Use this for initialization
	void Start () 
	{
		Instantiate(enemy,new Vector3(3,10,0),Quaternion.identity);
	}

	// Update is called once per frame
	void Update () 
	{
		Instantiate(enemy,new Vector3(Random.value * 23.0f,Random.value*25.0f,Random.value * 24.0f),Quaternion.identity);
	}
}
