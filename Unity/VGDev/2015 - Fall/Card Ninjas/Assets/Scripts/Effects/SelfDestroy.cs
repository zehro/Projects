﻿using UnityEngine;
using System.Collections;

public class SelfDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void selfDestroy()
	{
		Destroy(this.gameObject);
	}
}
