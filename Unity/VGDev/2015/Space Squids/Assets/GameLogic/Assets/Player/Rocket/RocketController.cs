using UnityEngine;
using System.Collections;

public class RocketController : MonoBehaviour
{
	public GameObject ignore;
	public GameObject target;
	public bool homing;

	int state = 0;
	Quaternion targetRot;
	float targetRotDrag = 0.1F;
	float scale = 0;
	float scaleDrag = 8;
	float lifeBegin;
	float lifeTime = 8;
	float rocketSpeed = 0.5F;
	float rocketExplosionForce = 2500;
	float explosionTime;
	ParticleSystem[] particles;
	ParticleSystem[] explosion;
	TrailRenderer trail;
	Light blinkLight;
	AudioSource sonarSound;
	AudioSource exhaustSound;
	AudioSource explosionSound;

	void Awake()
	{
		particles = GetComponentsInChildren<ParticleSystem>();
		explosion = transform.Find("Explosion").gameObject.GetComponentsInChildren<ParticleSystem>();
		trail = GetComponentInChildren<TrailRenderer>();
		blinkLight = GetComponentInChildren<Light>();
		sonarSound = transform.Find("Light").GetComponent<AudioSource>();
		exhaustSound = GetComponent<AudioSource>();
		explosionSound = transform.Find("Explosion").gameObject.GetComponent<AudioSource>();
	}

	void Start()
	{
		transform.localScale = Vector3.zero;
		lifeBegin = Time.time;
		sonarPitch();
	}

	void FixedUpdate()
	{
		if (state == 0)
		{
			sonarPitch();

			if (target != null)
			{
				targetRot = Quaternion.LookRotation(target.transform.position-transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,(homing ? targetRotDrag : 0));
			}

			transform.position += transform.rotation * new Vector3(0,0,rocketSpeed);
			blinkLight.intensity = 1-((Time.time*4) % 1);
			blinkLight.range = 1-((Time.time*4) % 1)*0.5F;
			if (Time.time-lifeBegin > lifeTime)
				explode();
		}

		if (state == 1)
		{
			exhaustSound.volume = Mathf.Max(exhaustSound.volume-0.05F,0);
			if (Time.time > explosionTime+2)
				Destroy(gameObject);
		}

		scale += ((1-state)-scale)/scaleDrag;
		transform.localScale = new Vector3(scale,scale,scale);
	}

	void OnTriggerEnter(Collider other)
	{
		if (state == 0)
		{
			if (other.gameObject == target)
			{
				other.gameObject.GetComponent<SquidController>().spinOut();
				other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(rocketExplosionForce,transform.position,10);
			}
			if (other.gameObject != ignore)
			{
				RocketController possibleRocket = other.gameObject.GetComponent<RocketController>();
				if (possibleRocket == null)
					explode();
				else if (possibleRocket.target != target && possibleRocket.state == 0)
				{
					explode();
					possibleRocket.explode();
				}
			}
		}
	}

	void explode()
	{
		state = 1;
		explosionTime = Time.time;
		foreach (ParticleSystem p in particles)
			p.enableEmission = false;
		explosion[0].Emit(200);
		explosion[1].Emit(100);
		explosion[2].Emit(10);
		sonarSound.Stop();
		explosionSound.Play();
		trail.enabled = false;
		blinkLight.intensity = 0;
	}

	void sonarPitch()
	{
		float minDist = 2, maxDist = 32;
		float dist = Vector3.Magnitude(target.transform.position-transform.position);
		dist = Mathf.Clamp(dist,minDist,maxDist);
		dist = 1-(dist-minDist)/(maxDist-minDist);
		float life = Mathf.Clamp(Time.time-(lifeBegin+lifeTime-1),0,1);
		sonarSound.pitch = 1+Mathf.Max(dist,life)*3;
	}
}