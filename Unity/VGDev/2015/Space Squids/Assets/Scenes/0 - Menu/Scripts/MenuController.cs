using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
	static bool hasLoadedOnce;
	static int[] lastSelected = new int[] { -1, 1, 3, 0, -1 };
	public Transform[] cameraTargets;

	Camera camera;
	GameObject UIPhases;
	CircleWipe circleWipe;
	AudioSource[] UISounds;

	bool inputLeft;
	bool inputRight;
	bool inputUp;
	bool inputDown;
	bool inputFire;
	float inputLRSign = 0;
	float inputUDSign = 0;
	float inputLRSignPrev = 0;
	float inputUDSignPrev = 0;

	int phase;
	int phasePrev;
	float phaseTime;
	float phaseTimeStart;
	float phaseTimeLength;
	bool phaseInitialized = false;
	ButtonController[] phaseButtons;

	int selected;

	void Awake()
	{
		camera = transform.Find("Camera").GetComponent<Camera>();
		UIPhases = transform.Find("UI Phases").gameObject;
		circleWipe = transform.Find("UI Overlay/CircleWipe").GetComponent<CircleWipe>();
		UISounds = transform.Find("UI Sounds").GetComponents<AudioSource>();
	}

	void Start()
	{
		phase = (hasLoadedOnce ? 2 : 1);
		phasePrev = (hasLoadedOnce ? 2 : 0);
		phaseTimeStart = Time.time;
	}
	
	void Update()
	{
		// Input logic

		float lr, ud;
		lr = Input.GetAxis("Move Horizontal 0");
		ud = Input.GetAxis("Move Vertical 0");
		inputLRSign = lr < 0 ? -1 : (lr > 0 ? 1 : 0);
		inputUDSign = ud < 0 ? -1 : (ud > 0 ? 1 : 0);

		inputLeft = (lr < 0 && (inputLRSign-inputLRSignPrev) == -1);
		inputRight = (lr > 0 && (inputLRSign-inputLRSignPrev) == 1);
		inputUp = (ud < 0 && (inputUDSign-inputUDSignPrev) == -1);
		inputDown = (ud > 0 && (inputUDSign-inputUDSignPrev) == 1);
		inputFire = Input.GetButtonDown("Fire 0");
		inputLRSignPrev = inputLRSign;
		inputUDSignPrev = inputUDSign;

		// Phase logic

		phaseTime = Mathf.Clamp01((Time.time - phaseTimeStart)/phaseTimeLength);
		phaseTimeLength = (phasePrev == 0 ? 3.5F : 2);

		if (phaseTime == 1)
		{
			if (!phaseInitialized)
			{
				phaseButtons = UIPhases.transform.GetChild(phase).GetComponentsInChildren<ButtonController>();

				selected = lastSelected[phase];
				if (selected != -1)
					phaseButtons[selected].select();
				
				phaseInitialized = true;
				UISounds[0].Play();
			}

			int prev;
			switch (phase)
			{
			case 0:			// Flying in or quitting game
			case 4:			// Exiting to a level
				break;
			case 1:			// Main menu screen
				prev = selected;
				if (inputLeft)
					selected--;
				if (inputRight)
					selected++;
				if (selected < 0)
					selected = 2;
				if (selected > 2)
					selected = 0;
				if (selected != prev)
				{
					UISounds[0].Play();
					phaseButtons[prev].deselect();
					phaseButtons[selected].select();
				}
				if (inputFire)
				{
					lastSelected[phase] = selected;
					UISounds[1].Play();

					if (selected == 0)
						toPhase(3);
					else if (selected == 1)
						toPhase(2);
					else if (selected == 2)
					{
						circleWipe.setSceneToLoad(-1);
						circleWipe.fadeOut();
						toPhase(0);
					}
				}
				break;
			case 2:			// Level selection screen
				prev = selected;
				if (selected < 3)
				{
					if (inputLeft)
						selected--;
					if (inputRight)
						selected++;
					if (selected < 0)
						selected = 2;
					if (selected > 2)
						selected = 0;
					if (inputUp || inputDown)
						selected = 3;
				}
				else if (selected == 3)
				{
					if (inputUp || inputDown)
						selected = 1;
				}
				if (selected != prev)
				{
					UISounds[0].Play();
					phaseButtons[prev].deselect();
					phaseButtons[selected].select();
				}
				if (inputFire)
				{
					lastSelected[phase] = selected;
					UISounds[1].Play();

					if (selected < 3)
					{
						circleWipe.setSceneToLoad(selected+1);
						circleWipe.fadeOut();
						toPhase(4);

						hasLoadedOnce = true;
					}
					else
						toPhase(1);
				}

				break;
			case 3:
				if (inputFire)
				{
					UISounds[1].Play();
					toPhase(1);
				}
				break;
			}
		}

		// Set camera position and rotation

		var rotHover = Quaternion.identity;
		rotHover *= Quaternion.Euler(Mathf.Sin(Time.time*2.0F)*0.15F,0,0);
		rotHover *= Quaternion.Euler(0,Mathf.Cos(Time.time*1.5F)*0.25F,0);
		rotHover *= Quaternion.Euler(0,0,Mathf.Sin(Time.time*1.0F)*0.6F);
		var posHover = Vector3.zero;
		posHover.x += Mathf.Cos(Time.time*1.5F)*0.03F;
		posHover.y += Mathf.Sin(Time.time*2.5F)*0.03F;
		posHover.z += Mathf.Cos(Time.time*2F)*0.03F;

		float bias = 0.8F;
		float t = phaseTime;
		t *= t*t*(t*(t*6-15)+10);
		t /= ((1/bias-2)*(1-t)+1);

		camera.transform.position = Vector3.Lerp(cameraTargets[phasePrev].position, cameraTargets[phase].position, t) + posHover;
		camera.transform.rotation = Quaternion.Slerp(cameraTargets[phasePrev].rotation, cameraTargets[phase].rotation, t) * rotHover;
	}

	void toPhase(int index)
	{
		phaseButtons[selected].deselect();

		phaseTime = 0;
		phaseTimeStart = Time.time;
		phaseInitialized = false;
		phasePrev = phase;
		phase = index;
	}
}