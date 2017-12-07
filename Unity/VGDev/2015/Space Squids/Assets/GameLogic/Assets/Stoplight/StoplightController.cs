using UnityEngine;
using System.Collections;

public class StoplightController : MonoBehaviour
{
	public AudioClip appearSound;
	public AudioClip countSound;

	float time = 0;
	float timeEnter = 5.5F;
	float timeCount = 7F;
	float timeMusic = 11.25F;

	bool isCounting = false;
	bool hasPlayed = false;
	bool hasBoosted = false;

	Light[] lights;
	AudioSource sound;
	SquidController[] squids;

	float xzScale = 0;
	float yScale = 0;
	float xzScaleVel = 0;
	float yScaleVel = 0;
	float scaleVelForce = 0.025F;
	float scaleVelDrag = 0.925F;

	float xRot = -90;
	float zRot = 30;
	float xRotVel = 0;
	float zRotVel = 0;
	float rotVelForce = 0.015F;
	float rotVelDrag = 0.9F;

	void Awake()
	{
		lights = transform.Find("Lights").gameObject.GetComponentsInChildren<Light>();
		foreach (Light light in lights)
			light.gameObject.SetActive(false);

		sound = GetComponent<AudioSource>();
		squids = transform.parent.GetComponentsInChildren<SquidController>();
	}

	void Start()
	{
		transform.localScale = Vector3.zero;
	}

	void Update()
	{
		time += Time.deltaTime;

		if (time > timeEnter)
		{
			if (transform.localScale == Vector3.zero)
				sound.PlayOneShot(appearSound);

			xRotVel += -xRot*rotVelForce*Time.deltaTime*60;
			zRotVel += -zRot*rotVelForce*1.5F*Time.deltaTime*60;
			xRotVel *= rotVelDrag;
			zRotVel *= rotVelDrag;
			xRot += xRotVel;
			zRot += zRotVel;

			xzScaleVel += (1-xzScale)*scaleVelForce*Time.deltaTime*60;
			yScaleVel += (1-yScale)*scaleVelForce*1.5F*Time.deltaTime*60;
			xzScaleVel *= scaleVelDrag;
			yScaleVel *= scaleVelDrag;
			xzScale += xzScaleVel;
			yScale += yScaleVel;

			transform.rotation = Quaternion.Euler(xRot,90+zRot*0.3F,zRot);
			transform.localScale = new Vector3(xzScale,yScale,xzScale);
		}

		if (time > timeCount)
		{
			if (!isCounting)
				sound.PlayOneShot(countSound);
			isCounting = true;

			float rumble = 0;
			if (time < timeCount+3)
				rumble = 1-(timeCount+3-time)/3F;
			else
				rumble = Mathf.Max(1+(timeCount+3-time)*2,0);
			foreach (SquidController squid in squids)
				squid.setRumble(rumble*rumble*rumble);

			if (time > timeCount+3)
			{
				if (!hasBoosted)
				{
					foreach (SquidController squid in squids)
					{
						squid.setControl(1);
						squid.boost();
					}
				}
				hasBoosted = true;

				float rise = time-(timeCount+3);
				rise = Mathf.Pow(rise,3)*24F;

				transform.localPosition = new Vector3(-6,2.5F+rise,0);
			}

			for (int i = 0; i < 4; i += 1)
				lights[i].gameObject.SetActive((time-timeCount) > i);
			if ((((time-timeCount) % 1) < 0.5) && (time < timeCount+3))
			{
				xzScale = 1.05F;
				yScale = 1.05F;
			}
		}

		if (time > timeMusic)
		{
			if (!hasPlayed)
				transform.parent.parent.Find("AudioLogic").gameObject.GetComponent<AudioController>().playLevelMusic();
			hasPlayed = true;

			if (time > timeMusic+2)
				enabled = false;
		}
	}
}