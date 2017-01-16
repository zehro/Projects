using UnityEngine;
using System.Collections;

public class Skybox : MonoBehaviour
{
	void Start()
	{
		Material mat = GetComponent<MeshRenderer>().material;
		mat.renderQueue = 2999;
	}

	void Update()
	{
		transform.eulerAngles = new Vector3(260,Time.time,0);
	}
}