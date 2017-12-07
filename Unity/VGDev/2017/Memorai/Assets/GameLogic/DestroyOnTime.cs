using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to destroy objects after a certain amount of time
 */
public class DestroyOnTime : MonoBehaviour {
    bool destroyOnStart = true;
    bool timerStarted = false;
    public float waitTime = 3.0f;
    void Start () {
        if (destroyOnStart) {
            StartCoroutine(destroyTime());
        }
	}
    
    IEnumerator destroyTime() {
        if (!timerStarted) {
            timerStarted = true;
            yield return new WaitForSeconds(waitTime);
            Destroy(gameObject);
        }
    }

    //Used if you want to trigger a destruction later
    public void startDestroy() {
        StartCoroutine(destroyTime());
    }
}
