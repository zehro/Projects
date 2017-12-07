using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class persistentAudio : MonoBehaviour {

    void OnEnable() {
        StartCoroutine(waitForDestroy());
    }
	// Use this for initialization
	void Start () {
        GameObject.DontDestroyOnLoad(this);	
	}

    IEnumerator waitForDestroy() {
        float clipLength = GetComponent<AudioSource>().clip.length;
        yield return new WaitForSeconds(clipLength);
        Destroy(gameObject);
    }
}
