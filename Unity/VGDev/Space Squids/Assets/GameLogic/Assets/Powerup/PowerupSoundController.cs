using UnityEngine;
using System.Collections;

public class PowerupSoundController : MonoBehaviour
{
	public AudioClip inkSound;
	public AudioClip boostSound;
	AudioSource source;
	SquidController squid;
	float fade = 0;
	float fadeTarg = 0;
	float fadeDrag = 8;

	void Awake()
	{
		source = GetComponent<AudioSource>();
		squid = transform.parent.gameObject.GetComponent<SquidController>();
	}
	
	void Update()
	{
		int powerup = squid.getPowerup();
		if (powerup == 0)
			source.clip = inkSound;
		if (powerup == 1)
			source.clip = boostSound;
		if (powerup == 2)
			source.Stop();
		else if (!source.isPlaying)
			source.Play();

		float fire = Input.GetAxis("Fire "+squid.playerIndex)*squid.getControl();
		fadeTarg = (squid.getPowerupPhase() == 2 && source.isPlaying ? fire : 0);
		fade += (fadeTarg-fade)/fadeDrag;
		source.volume = fade;
		source.pitch = 0.75F + fade/4;
	}
}