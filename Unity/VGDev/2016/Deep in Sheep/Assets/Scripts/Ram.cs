using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class Ram : MonoBehaviour {
	
	private enum State {WANDER, ANGRY, CHARGING, STOPPED};

	Animation anim;
	private Rigidbody body;
	private AudioSource audioSource;
	private NavMeshAgent agent;
	private ParticleSystem.EmissionModule rageEmitter;
	float rageLevel = 0; //Current rage level
	public float ragePerSecond = 15; //Percent the rage ticks up each second when a player is near in wander mode.
	public float ragePlayerRadius = 12; //Distance a player has to be to anger the sheep.
	public float ragePlayerDoubleRadius = 6; //Double rage gain if a player is inside this radius.
	public float rageCooldownPerSecond = -3; //Reduce rage by this amount while nobody is near.
	public float chargeDelay = 5; //Time until starting a charge after enraged.
	public float chargeTime = 3; //Time to maintain a charge.
	public float recoverDelay = 6; //TIme until returning to wander mode after a charge.
	private float chargeTimer = 0; //Timer for charging or stopping.
	public float wanderRadius = 12;
	public float wanderSpeed = 6;
	public float chargeSpeed = 18;
	public Color idleColor = Color.white;
	public Color angryColor = Color.red;
	Vector3 corralLoc;
	State state;

	void Start () {
		body = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource> ();
		state = State.WANDER;
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponentInChildren<Animation>();
		rageEmitter = GetComponentInChildren<ParticleSystem>().emission;
		rageEmitter.enabled = false;
	}

	void FixedUpdate () {
		updateState();
	}

	void baa() {
		if (!audioSource.isPlaying) {
			audioSource.pitch = 0.7f;
			audioSource.Play ();
		}
	}

	void updateState() {
		switch(state) {
		case State.WANDER:
			if(body.velocity.magnitude > 1f)
				anim.GetClip("SheepJump").wrapMode = WrapMode.Loop;
			else
				anim.GetClip("SheepJump").wrapMode = WrapMode.Once;
			anim.Play();
			if (Random.Range(1, 100) <= 2) {
				moveRandom();
			}
			//Debug.Log(rageLevel);
			GameObject p = nearbyPlayer(this.ragePlayerRadius);
			if(p != null)
			{
				float dist = Vector3.Distance(p.transform.position, this.transform.position);
				if(dist < this.ragePlayerDoubleRadius)
				{
					this.rageLevel += this.ragePerSecond * 2 * Time.fixedDeltaTime;
					rageEmitter.enabled = true;
				}
				else
				{
					this.rageLevel += this.ragePerSecond * Time.fixedDeltaTime;
					rageEmitter.enabled = false;
				}
				if (Random.Range (0, 1000) > 994) {
					baa();
				}
			}
			else
				this.rageLevel += this.rageCooldownPerSecond * Time.fixedDeltaTime;
			if(this.rageLevel < 0)
				this.rageLevel = 0;
			this.GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(this.idleColor, this.angryColor, this.rageLevel/100f);
			if(this.rageLevel >= 100)
			{
				this.GetComponentInChildren<MeshRenderer>().material.color = this.angryColor;
				rageEmitter.enabled = true;
				this.rageLevel = 0;
				agent.updatePosition = false;
				agent.updateRotation = false;
				agent.Stop();
				this.chargeTimer = Time.fixedTime;
				this.state = State.ANGRY;
			}
			break;
		case State.ANGRY:
			anim.GetClip("SheepJump").wrapMode = WrapMode.Once;
			//Debug.Log(">:(");
			GameObject target = nearbyPlayer(this.ragePlayerRadius * 1.5f);
			agent.speed = 0;
			if(target != null)
			{
				Vector3 targetDir = (target.transform.position - this.transform.position);
				targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up).normalized;
				Quaternion newRotation = Quaternion.LookRotation(targetDir);
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, 120 * Time.fixedDeltaTime);
				//this.transform.Rotate(Vector3.RotateTowards(this.transform.rotation.eulerAngles, targetDir, Mathf.Deg2Rad * 120 * Time.fixedDeltaTime, 1));
			}
			if(Time.fixedTime - this.chargeTimer > this.chargeDelay)
			{
				this.chargeTimer = 0;
				//agent.Stop();
				agent.speed = this.chargeSpeed;
				this.state = State.CHARGING;
				rageEmitter.enabled = false;
				this.chargeTimer = Time.fixedTime;
				baa();
				return;
			}
			break;
		case State.CHARGING:
			anim.Play("SheepRun");
			if(Time.fixedTime - this.chargeTimer > this.chargeTime)
			{
				this.state = State.STOPPED;
				this.chargeTimer = Time.fixedTime;
				agent.speed = 0;
				return;
			}
			GameObject chargeTarget = nearbyPlayer(this.ragePlayerRadius * 1.5f);
			if(chargeTarget != null)
			{
				Vector3 targetDir = (chargeTarget.transform.position - this.transform.position);
				targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up).normalized;
				Quaternion newRotation = Quaternion.LookRotation(targetDir);
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, newRotation, 50 * Time.fixedDeltaTime);
			}
			Vector3 desiredVelocity = transform.forward * agent.speed;
			Vector3 addVec = desiredVelocity - body.velocity;
			float mag = addVec.magnitude;
			mag = Mathf.Max(mag, agent.acceleration * Time.fixedDeltaTime);
			addVec.y = 0;
			addVec = addVec.normalized * mag;
			body.velocity += addVec;
			body.velocity += Physics.gravity * Time.fixedDeltaTime * 1;
			break;
		case State.STOPPED:
			anim.Stop();
			this.GetComponentInChildren<MeshRenderer>().material.color = Color.Lerp(this.angryColor, this.idleColor, (Time.fixedTime - this.chargeTimer) / this.recoverDelay);
			if(Time.fixedTime - this.chargeTimer > this.recoverDelay)
			{
				this.GetComponentInChildren<MeshRenderer>().material.color = this.idleColor;
				state = State.WANDER;
				agent.speed = this.wanderSpeed;
				agent.Warp(transform.position);
				agent.updatePosition = true;
				agent.updateRotation = true;
				this.moveRandom();
				this.chargeTimer = 0;
			}
			break;
		default:
			GetComponent<NavMeshAgent>().enabled = true;
			state = State.WANDER;
			agent.speed = this.wanderSpeed;
			break;
		}
	}

	void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag == "Player")
		{
			Vector3 awayVec = Vector3.ProjectOnPlane((col.transform.position - this.transform.position).normalized, Vector3.up);
			if(this.state != State.CHARGING)
			{
				col.rigidbody.velocity += awayVec * 8 + Vector3.up * 6;
				col.gameObject.GetComponent<PlayerController>().setGroundedTimeout();
			}
			else
			{
				col.rigidbody.velocity += awayVec * 24 + Vector3.up * 35;
				col.gameObject.GetComponent<PlayerController>().setGroundedTimeout();
			}
			baa();
		}
		else if(col.gameObject.tag == "Sheep")
		{
			Vector3 awayVec = Vector3.ProjectOnPlane((col.transform.position - this.transform.position).normalized, Vector3.up);
			if(this.state == State.CHARGING)
			{
				col.gameObject.GetComponent<Sheep>().toggleSheepMovement();
				col.rigidbody.velocity += awayVec * 24 + Vector3.up * 30;
			}
		}
	}

	GameObject nearbyPlayer(float dist)
	{
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		GameObject closest = null;
		foreach(GameObject p in players)
		{
			float d = Vector3.Distance(p.transform.position, this.transform.position);
			if(d < dist)
			{
				dist = d;
				closest = p;
			}
		}
		return closest;
	}

	void moveRandom()
	{
		Vector3 destination = new Vector3(transform.position.x + Random.Range(-wanderRadius, wanderRadius), transform.position.y, transform.position.z + Random.Range(-wanderRadius, wanderRadius));
		agent.SetDestination(destination);
	}

}
