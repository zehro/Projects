using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public AudioClip[] clips;
    public AudioSource audio1;
    public AudioSource audio2;
    public float maxVol = 0.1f;
	// Use this for initialization
	void Start () {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume");
        
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void swap() {
        StartCoroutine(swapNum());
    }

    public void swapReverse() {
        StartCoroutine(swapReverseNum());
    }

    IEnumerator swapNum () {
        while (audio1.volume > 0 || audio2.volume < maxVol) {
            if (audio1.volume > 0) {
                audio1.volume -= 0.01f;
            }

            if (audio2.volume < maxVol) {
                audio2.volume += 0.01f;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator swapReverseNum() {
        while (audio2.volume > 0 || audio1.volume < maxVol) {
            if (audio2.volume > 0) {
                audio2.volume -= 0.01f;
            }

            if (audio1.volume < maxVol) {
                audio1.volume += 0.01f;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
}
