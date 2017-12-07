using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
 * Responsible for spawning enemies at the beginning of the level
 */
public class Spawner : MonoBehaviour {
    public GameObject[] spawnPoints;
    public GameObject[] spawner;
    public int maxSpawnPerWave;
    public UnityEvent finishEvent;
    public UnityEvent startEvent;
	public GameObject player;

    ArrayList curList = new ArrayList();

    int spawnCounter = 0;
    bool triggered = false;
    bool finished = false;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
	}

    public void spawn() {
        int waveCounter = 0;
        if (spawnCounter == 0) {
            startEvent.Invoke();
        }
        while (waveCounter < maxSpawnPerWave) {
            if (spawnCounter < spawner.Length) {
                GameObject spawn = spawner[spawnCounter];
                spawnCounter += 1;
				int spawnPointNumber = Random.Range (0, spawnPoints.Length - 1); 
				Vector3 spawnPoint = spawnPoints[spawnPointNumber].GetComponent<Transform>().position;
				//this was brute force fix to enemies spawning. Works but not as effectively as the fix below. 
				//Will keep in case other fix breaks.
//				if (Mathf.Abs (player.transform.position.x - spawnPoint.x) < 4) {
//					if (player.GetComponent<Rigidbody2D> ().velocity.x < 0) {
//						spawnPoint = spawnPoint + new Vector3 (8, 0, 0);
//					} else {
//						spawnPoint = spawnPoint + new Vector3 (-8, 0, 0);
//					}
//				}

				if (Mathf.Abs (player.transform.position.x - spawnPoint.x) < 5) {
                    spawnPointNumber -= 1;
                    if (spawnPointNumber < 0) spawnPointNumber = spawnPoints.Length - 1;
					spawnPoint = spawnPoints[spawnPointNumber].GetComponent<Transform>().position;
				}

                GameObject newEnemy = Instantiate(spawn, new Vector2(spawnPoint.x + Random.Range(-2,2), spawnPoint.y + Random.Range(-2,2)), Quaternion.identity);
                curList.Add(newEnemy);
            }
            waveCounter += 1;
        }
        triggered = true;
    }

    public void removeCurrent(GameObject item) {
        curList.Remove(item);
    }
    void Update() {
        if (curList.Count <= 0 && spawnCounter < spawner.Length && triggered) {
            spawn();
        }
        if (spawnCounter >= spawner.Length && curList.Count == 0 && !finished) {
            finishEvent.Invoke();
            finished = true;
        }
    }

    public ArrayList getCurArray() {
        return curList;
    }

  
}
