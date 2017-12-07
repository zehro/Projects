﻿using UnityEngine;
using System.Collections;

public class TimeControlledParticles : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem particles = GetComponent<ParticleSystem>();
		if (particles != null) {
			GetComponent<ParticleSystem>().playbackSpeed = Mathf.Abs(Player.instance.timeScale);
		}
	}
}
