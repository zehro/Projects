using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehavior : MonoBehaviour {
	public int health;
	private int maxHealth = 80;
	public GameObject[] allEnemies;
	public GameObject[] spawnPoints;
	private float timer = 0;


	[Range(0, 2)]
	private int state = 0;

	//0 - IDLE
	//1 - SPAWNING
	//2 - DEATH


	// Use this for initialization
	void Start () {
		health = maxHealth;

	}
	
	// Update is called once per frame
	void Update () {
//		Vector3 spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].GetComponent<Transform>().position;
//		Instantiate (allEnemies [0], new Vector2 (spawnPoint.x + Random.Range (-2, 2),
//			spawnPoint.y + Random.Range (-2, 2)), Quaternion.identity);

		if (health <= 0) {
			Death ();
			state = 2;
		}
		if (state == 1 && timer <= 0) {
			state = 0;
		} else if (state == 1) {
			timer -= Time.deltaTime;
		}
	}

	public void Hurt() {
		Vector3 spawnPoint1 = spawnPoints [Random.Range (0, spawnPoints.Length - 1)].GetComponent<Transform> ().position;
		Vector3 spawnPoint2 = spawnPoints [Random.Range (0, spawnPoints.Length - 1)].GetComponent<Transform> ().position;
		Vector3 spawnPoint3 = spawnPoints [Random.Range (0, spawnPoints.Length - 1)].GetComponent<Transform> ().position;
		if (state != 1) {
			if (health > (maxHealth / 3)) {
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint1.x + Random.Range (-2, 2),
					spawnPoint1.y + Random.Range (-2, 2)), Quaternion.identity);
			} else if (health > (2 * maxHealth / 3)) {
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint1.x + Random.Range (-2, 2),
					spawnPoint1.y + Random.Range (-2, 2)), Quaternion.identity);
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint2.x + Random.Range (-2, 2),
					spawnPoint2.y + Random.Range (-2, 2)), Quaternion.identity);
			} else {
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint1.x + Random.Range (-2, 2),
					spawnPoint1.y + Random.Range (-2, 2)), Quaternion.identity);
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint2.x + Random.Range (-2, 2),
					spawnPoint2.y + Random.Range (-2, 2)), Quaternion.identity);
				Instantiate (allEnemies [Random.Range (0, allEnemies.Length)], new Vector2 (spawnPoint3.x + Random.Range (-2, 2),
					spawnPoint3.y + Random.Range (-2, 2)), Quaternion.identity);
			}
			timer = .5f;
		}
		health -= 10;
	}

	public void Death() {
		Destroy (this.gameObject);
	}
}
