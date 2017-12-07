using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour {

	public AudioClip[] whoosh, step;

	public AudioClip affirm, negative, click, jump;

	public AudioClip arrowPull, arrowShoot, arrowHit, death1, death2;

	public static SFXManager instance;

	// Use this for initialization
	void Start () {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(this.gameObject);
		}
		DontDestroyOnLoad(this.gameObject);
	}

	void OnEnable() {
		transform.parent = Camera.main.transform;
		transform.localPosition = Vector3.zero;
		transform.parent = null;
	}

	public void PlayAffirm() {
		GetComponent<AudioSource>().PlayOneShot(affirm);
	}

	public void PlayJump() {
		GetComponent<AudioSource>().PlayOneShot(jump,0.5f);
	}

	public void PlayFootstep() {
		GetComponent<AudioSource>().PlayOneShot(step[Random.Range(0,step.Length)], 0.5f);
	}

	public void PlayWhoosh() {
		GetComponent<AudioSource>().PlayOneShot(whoosh[Random.Range(0,whoosh.Length)], 0.5f);
	}

	public void PlayNegative() {
		GetComponent<AudioSource>().PlayOneShot(negative);
	}

	public void PlayDeath() {
		GetComponent<AudioSource>().PlayOneShot(Random.value > 0.5 ? death1 : death2);
	}

	public void PlayClick() {
		GetComponent<AudioSource>().PlayOneShot(click, 0.25f);
	}

	public void PlayArrowPull() {
		GetComponent<AudioSource>().PlayOneShot(arrowPull);
	}

	public void PlayArrowShoot() {
		GetComponent<AudioSource>().PlayOneShot(arrowShoot);
	}

	public void PlayArrowHit() {
		GetComponent<AudioSource>().PlayOneShot(arrowHit);
	}

}
