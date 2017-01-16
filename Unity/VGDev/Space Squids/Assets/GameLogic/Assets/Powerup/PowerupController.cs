using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour
{
	public AudioClip redSound;
	public AudioClip blueSound;

	Vector3 initialPos;
	ParticleSystem fxParticles;
	ParticleSystem fxBurst;
	Light fxLight;
	AudioSource sound;
	GameObject follow;

	int state = 1;
	float stateSlide = 1;
	int stateDrag = 8;
	float cooldownStart;
	int cooldown = 2;
	
	void Awake()
	{
		initialPos = transform.position;
		fxParticles = GetComponent<ParticleSystem>();
		fxBurst = transform.Find("BurstFX").GetComponent<ParticleSystem>();
		sound = GetComponent<AudioSource>();
		fxLight = GetComponent<Light>();
	}

	void Update()
	{
		stateSlide += (state-stateSlide)/stateDrag;
		float s = stateSlide * stateSlide;
		transform.localScale = new Vector3(s, s, s);
		fxParticles.enableEmission = (state == 1 ? true : false);
		fxLight.range = stateSlide * 5;

		if (state == 0)
		{
			transform.position = Vector3.MoveTowards(transform.position, follow.transform.position,
			                                         Vector3.Distance(transform.position, follow.transform.position) * 0.4F);

			if (Time.time > cooldownStart + cooldown)
			{
				transform.position = initialPos;
				state = 1;
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (state == 1)
		{
			SquidController squid = other.GetComponent<SquidController>();
			if ((squid != null) && (squid.getPowerupPhase() == 0))
			{
				follow = squid.gameObject;
				squid.givePowerup();

				state = 0;
				cooldownStart = Time.time;
				fxBurst.Play();
				if (squid.playerIndex == 0)
					sound.PlayOneShot(redSound);
				else
					sound.PlayOneShot(blueSound);
			}
		}
	}
}