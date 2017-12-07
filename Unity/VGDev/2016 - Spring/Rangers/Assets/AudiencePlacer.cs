using UnityEngine;
using System.Collections.Generic;

public class AudiencePlacer : MonoBehaviour {

	private List<GameObject> crowd;
	private List<GameObject> seats;

	// Use this for initialization
	void Start () {
		crowd = new List<GameObject>();
		seats = new List<GameObject>();

		for(int i = 0; i < transform.childCount; i++) {
			if(transform.GetChild(i).gameObject.name.Contains("Cube")) {
				for(int j = 0; j < transform.GetChild(i).childCount; j++) {
					seats.Add(transform.GetChild(i).GetChild(j).gameObject);
				}
			} else if (transform.GetChild(i).gameObject.name.Contains("Audience")) {
				crowd.Add(transform.GetChild(i).gameObject);
			}
		}

		foreach (GameObject g in crowd) {
			int randIndex = Random.Range(0,seats.Count);
			while(seats[randIndex].transform.childCount != 0) {
				randIndex++;
				if (randIndex >= seats.Count) randIndex = 0;
			}
			g.transform.parent = seats[randIndex].transform;
			g.transform.localRotation = Quaternion.identity;
			g.transform.localPosition = Vector3.zero + seats[randIndex].transform.forward*0.5f - Vector3.up*0.5f;
		}

		Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
