using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatisticManager : MonoBehaviour {

	public static StatisticManager instance;

	public Dictionary<PlayerID, Statistic> statistics;

	// Use this for initialization
	void Awake () {
		statistics = new Dictionary<PlayerID, Statistic>();

		if(instance != null && instance != this)
			Destroy(this.gameObject);
		else
			instance = this;

		DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
//	void Update () {
//		
//	}
}
