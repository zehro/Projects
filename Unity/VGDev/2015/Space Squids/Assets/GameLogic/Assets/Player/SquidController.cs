using UnityEngine;
using System.Collections;

public class SquidController : MonoBehaviour
{
	public int playerIndex = 0;
	public GameObject rocketPrefab;
	float control = 0;
	float rumble = 0;

	float turn = 0;
	float turnTarg = 0;
	float turnDrag = 0.15F;

	float roll = 0;
	float rollDrag = 0.1F;
	float rollStrength = 1/4F;

	public float moveForce = 75;
	float moveBoost = 1;
	float moveBoostMax = 3;
	float moveBoostDrag = 1/32F;
	float moveBoostRoll = 0;
	float fovBase = 65;
	float fovDrag = 1/8F;
	
	float lookSpeed = 150;
	Quaternion lookTarg = Quaternion.identity;
	float lookDrag = 0.15F;
	Quaternion shipLook = Quaternion.identity;
	float shipLookSpeed = 20;
	float shipLookDrag = 0.1F;

	int powerup = 0;			// 0 = ink, 1 = boost, 2 = missiles
	int powerupPhase = 0;		// 0 = no powerup, 1 = roulette warmup, 2 = has powerup, 3 = cooldown
	float powerupTime = 0;
	float powerupWarmCool = 1;
	float powerupAmount = 0;
	float powerupFirePrev = 1;
	bool powerupAimed = false;

	Vector3 spinOutVector = Vector3.zero;
	float spinOutFactor = 0;

	Rigidbody body;
	Camera cam;
	Transform model;
	Quaternion modelRot;
	Transform[] armature;
	Transform armatureBase;
	Quaternion[] armatureRots;
	ParticleSystem inkFX;
	ParticleSystem boostFX;
	ParticleSystem superBoostFX;
	TrailRenderer trailFX;
	AudioSource engineSound;
	ArrowController arrow;
	GameController game;
	GameObject otherSquid;

	Vector3 camPos;
	Quaternion camRot;
	Vector3 camTargPos = new Vector3(3,1,0);
	Quaternion camTargRot = Quaternion.Euler(0,270,0);
	float camDrag = 1.1F;

	void Awake()
	{
		body = GetComponent<Rigidbody>();
		cam = transform.Find("Camera").gameObject.GetComponent<Camera>();
		model = transform.Find("Model");
		modelRot = model.localRotation;
		armature = transform.Find("Model/Armature/spine").gameObject.GetComponentsInChildren<Transform>();
		armatureBase = transform.Find("Model/Armature");
		armatureRots = new Quaternion[armature.Length];
		for (int i = 1; i < armature.Length; i++)
		{
			armatureRots[i] = armature[i].rotation;
		}

		inkFX = transform.Find("Model/InkFX").gameObject.GetComponent<ParticleSystem>();
		boostFX = transform.Find("Model/BoostFX").gameObject.GetComponent<ParticleSystem>();
		superBoostFX = transform.Find("Model/SuperBoostFX").gameObject.GetComponent<ParticleSystem>();
		trailFX = transform.Find("Model/Trail").gameObject.GetComponent<TrailRenderer>();
		engineSound = GetComponent<AudioSource>();
		arrow = transform.Find("Arrow").gameObject.GetComponent<ArrowController>();
		game = transform.parent.parent.gameObject.GetComponent<GameController>();
		otherSquid = transform.parent.GetChild(1-playerIndex).gameObject;
	}

	void Start()
	{
		camPos = new Vector3(100,40,(playerIndex-0.5F)*50F);
		camRot = Quaternion.LookRotation(new Vector3(0,1,0)-camPos);
		cam.transform.localPosition = camPos;
		cam.transform.localRotation = camRot;
	}

	void FixedUpdate()
	{
		// Get movement input, strength, and direction

		var dx = Input.GetAxis("Move Horizontal "+playerIndex)*control;
		var dy = Input.GetAxis("Move Vertical "+playerIndex)*control;
		var len = Mathf.Min(Mathf.Sqrt(dx*dx+dy*dy),1F);
		var dir = Mathf.Atan2(dy,dx)*Mathf.Rad2Deg+90;

		// Calculate the turn of the ship
		// The ship's turn target changes when the user is pushing hard enough in a direction, and the
		// turn angle continuously approaches its target by moving a turnDrag of their signed difference

		if (len > 0.25)
			turnTarg = dir;
		turn += Mathf.DeltaAngle(turn,turnTarg)*turnDrag;

		// Calculate the roll of the ship
		// The ship's roll target is the signed difference between its current turn and its turn target,
		// which makes the ship "bank" into directional changes with increasing strength for sharper turns
		// Like the turn angle, the roll angle continuously approaches its target with rollDrag

		var rollTarg = Mathf.DeltaAngle(turn,turnTarg)*rollStrength;
		roll += (rollTarg-roll)*rollDrag;

		// Add force in the direction the ship is facing, and expand the FOV
		// of the camera with the resultant speed (makes things look "faster")
		// The FOV also has a target and a drag to smooth it out

		var facingVec = body.rotation*Quaternion.Euler(0,turn,0)*-Vector3.right;
		moveBoost += (1-moveBoost)*moveBoostDrag;
		moveBoostRoll = (moveBoost-1)*(moveBoost-1)*-90;
		body.AddForce(facingVec*len*moveForce*moveBoost);

		var fovTarg = fovBase+body.velocity.magnitude*(4F+moveBoost)/5F-rumble*30-spinOutFactor;
		cam.fieldOfView += (fovTarg-cam.fieldOfView)*fovDrag;

		// Make the trail widen with the boost, make the boost particle fade,
		// and make the pitch of the engine sound relate to the speed

		trailFX.startWidth = moveBoost-0.5F;
		float s = Mathf.Max(0,moveBoost-1.5F);
		boostFX.startColor = new Color(1F,1F,1F,1-(1-s)*(1-s));
		engineSound.pitch = 1+body.velocity.magnitude*0.4F;

		// Handle powerups
		
		switch (powerupPhase)
		{
		case 0:
			powerupAmount = 1;
			break;
		case 1:
		case 3:
			if ((Time.time - powerupTime) > powerupWarmCool)
				powerupPhase = (powerupPhase+1) % 4;
			break;
		case 2:
			float fire = Input.GetAxis("Fire "+playerIndex)*control;
			switch (powerup)
			{
			case 0:
				powerupAmount -= fire * 1/120F;
				if (fire == 1)
					inkFX.Emit(1);
				break;
			case 1:
				powerupAmount -= fire * 1/90F;
				if (fire == 1)
				{
					superBoostFX.Emit(3);
					gentleBoost();
				}
				break;
			case 2:
				if (fire-powerupFirePrev == 1)
				{
					powerupAmount -= 0.334F;
					fireRocket();
				}
				break;
			}
			powerupFirePrev = fire;
			if (powerupAmount <= 0)
			{
				powerupPhase = 3;
				powerupTime = Time.time;
				powerupFirePrev = 1;
			}
			break;
		}
		
		Vector3 p1 = cam.WorldToScreenPoint(otherSquid.transform.position);
		Vector3 p2 = new Vector3(Screen.width*0.5F, Screen.height*(0.75F - playerIndex*0.5F),0);
		powerupAimed = (Mathf.Abs(p1.x-p2.x) < 300 && Mathf.Abs(p1.y-p2.y) < 150 && p1.z > 0);
	}

	public void setControl(int input)
	{
		control = input;
		//if (input == 1)
			arrow.expand();
	}

	public float getControl()
	{
		return control;
	}

	public void setRumble(float input)
	{
		rumble = input;
	}

	public void boost()
	{
		boostFX.Play();
		moveBoost = moveBoostMax;
	}

	public void gentleBoost()
	{
		boostFX.Play();
		moveBoost = 2;
	}

	public void fireRocket()
	{
		Quaternion rot = transform.localRotation * Quaternion.Euler(0,270,0);
		GameObject rocket = (GameObject) Instantiate(rocketPrefab, transform.position, rot);
		RocketController controller = rocket.GetComponent<RocketController>();
		controller.ignore = gameObject;
		controller.target = otherSquid;
		controller.homing = powerupAimed;
	}

	public void givePowerup()
	{
		// The powerup received depends on this player's standing
		// If this squid is ahead:	80% chance for ink, 10% for boost, 10% for missiles
		// If this squid is behind: 10% chance for ink, 50% for boost, 40% for missiles
		// If the squids are even:  33% chance for ink, 33% for boost, 33% for missiles

		int lead = game.getLeader();
		if (lead == playerIndex)
		{
			int seed = Random.Range(0,10);
			if (seed <= 7)
				powerup = 0;
			else if (seed == 8)
				powerup = 1;
			else
				powerup = 2;
		}
		else if (lead == 1-playerIndex)
		{
			int seed = Random.Range(0,10);
			if (seed == 0)
				powerup = 0;
			else if (seed <= 4)
				powerup = 1;
			else
				powerup = 2;
		}
		else
			powerup = Random.Range(0,3);

		powerupPhase = 1;
		powerupTime = Time.time;
		powerupAmount = 1;
	}

	public int getPowerup()
	{
		return powerup;
	}

	public int getPowerupPhase()
	{
		return powerupPhase;
	}

	public void setPowerupAmount(float input)
	{
		powerupAmount = input;
	}

	public float getPowerupAmount()
	{
		return powerupAmount;
	}

	public bool getPowerupAimed()
	{
		return powerupAimed;
	}

	public void spinOut()
	{
		spinOutVector = Random.onUnitSphere;
		spinOutFactor = 12F;
	}

	void Update()
	{
		// Get look input

		var dx = Input.GetAxis("Look Horizontal "+playerIndex)*control;
		var dy = Input.GetAxis("Look Vertical "+playerIndex)*control;
		var dz = Input.GetAxis("Look Roll "+playerIndex)*control;
		dx *= dx*Mathf.Sign(dx);
		dy *= dy*Mathf.Sign(dy);
		dz *= dz*Mathf.Sign(dz);
		dx += spinOutVector.x * spinOutFactor;
		dy += spinOutVector.y * spinOutFactor;
		dz += spinOutVector.z * spinOutFactor;
		spinOutFactor = Mathf.Max(spinOutFactor - 0.25F, 0);

		// Change the target look orientation based on the user's camera control input
		// and slowly interpolate this parent object's rotation to it based on the lookDrag

		var s = lookSpeed*Time.deltaTime*(2F+moveBoost)/3F;
		lookTarg *= Quaternion.AngleAxis(dx*s,-Vector3.up);
		lookTarg *= Quaternion.AngleAxis(dy*s,-Vector3.forward);
		lookTarg *= Quaternion.AngleAxis(dz*s,-Vector3.right);
		transform.localRotation = Quaternion.Slerp(transform.localRotation,lookTarg,lookDrag);

		// Change the ship's look orientation as well

		s = shipLookSpeed;

		var shipLookTarg = Quaternion.identity;
		shipLookTarg *= Quaternion.AngleAxis(dx*s,-Vector3.up);
		shipLookTarg *= Quaternion.AngleAxis(dy*s,-Vector3.forward);
		shipLookTarg *= Quaternion.AngleAxis(dz*s,-Vector3.right);
		shipLook = Quaternion.Slerp(shipLook,shipLookTarg,shipLookDrag);

		// Finally, set the position and rotation of the ship model
		// This includes a little bit of permutation for when the ship is idle

		s = Mathf.Max(0,1-body.velocity.magnitude*0.05F);

		float hOff = playerIndex*5F;
		var rotHover = Quaternion.identity;
		rotHover *= Quaternion.Euler(Mathf.Sin(Time.time*2F+hOff)*2*s,0,0);
		rotHover *= Quaternion.Euler(0,Mathf.Cos(Time.time*1.5F+hOff)*3*s,0);
		rotHover *= Quaternion.Euler(0,0,Mathf.Sin(Time.time*2.5F+hOff)*s);
		var posHover = Vector3.zero;
		posHover.x += Mathf.Cos(Time.time*1.5F+hOff)*0.03F*s;
		posHover.y += Mathf.Sin(Time.time*2.5F+hOff)*0.03F*s;
		posHover.z += Mathf.Cos(Time.time*2F+hOff)*0.015F*s;
		model.localPosition = posHover;

		model.rotation = body.rotation;
		model.rotation *= shipLook;
		model.rotation *= Quaternion.Euler(0,turn,0);
		model.rotation *= Quaternion.Euler(roll+moveBoostRoll,0,0);
		model.rotation *= modelRot;
		model.rotation *= rotHover;
		model.rotation *= Quaternion.Euler(spinOutFactor*30,spinOutFactor*30,spinOutFactor*30);

		// Slowly center the camera over time

		camPos = Vector3.Lerp(camPos,camTargPos,camDrag*Time.deltaTime);
		camRot = Quaternion.Slerp(camRot,camTargRot,camDrag*Time.deltaTime);
		cam.transform.localPosition = camPos + new Vector3(rumble*2F,-rumble*0.25F,0);
		Vector3 jitter = Random.insideUnitSphere;
		jitter.Scale(new Vector3(rumble*0.075F,rumble*0.075F,rumble*0.075F));
		cam.transform.localPosition += jitter;
		cam.transform.localRotation = camRot;

		// Rotate the armature

		armatureBase.rotation = Quaternion.identity;
		
		armature[0].rotation = model.rotation * Quaternion.Euler(270,180,0);
		for (int i = 1; i < armature.Length; i++)
		{
			float speed = 1 + body.velocity.magnitude * 0.2F;
			Quaternion permutation = Quaternion.Euler(Mathf.Sin(Time.time * 3F + i * 2) * speed,
			                                          Mathf.Cos(Time.time * 4F + i * 5) * speed * 2,
			                                          Mathf.Sin(Time.time * 5F + i * 7) * speed * 2);
			if (armature[i].name.Contains("_"))
			{
				permutation *= Quaternion.Euler(0,dy*-20,dx*-20);
				permutation *= Quaternion.Euler(0,0,Mathf.DeltaAngle(turn,turnTarg));
			}
			armature[i].rotation = model.rotation * armatureRots[i] * permutation;
		}

		// Point the arrow

		arrow.pointAt(game.getCheckpointPos(playerIndex), transform.localRotation * Vector3.up);
	}
}