using UnityEngine;
using System.Collections;
using Assets.Scripts.Data;

public class CameraFollow : MonoBehaviour 
{

	public float speed;

	private Vector3 startingPos;
	private int numPlayers;
	private float greatestDistance;

	private Camera blurCam;

	private Vector3 targetPos;

	public Vector3 TargetPos {
		get { return targetPos;}
		private set {targetPos = value;}
	}

	// Use this for initialization
	void Start ()
	{
		startingPos = transform.position;
		targetPos = startingPos;
		if(transform.FindChild("BlurCam")) {
			blurCam = transform.FindChild("BlurCam").GetComponent<Camera>();
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		if(!GameManager.instance.GameFinished) {
			Vector3 averagePosition = Vector3.zero;
			numPlayers = 0;
			for(int i = 0; i < GameManager.instance.AllPlayers.Count; i++) 
			{
				if(GameManager.instance.AllPlayers[i].LifeComponent.HealthPercentage > 0) {
					numPlayers++;
					averagePosition += GameManager.instance.AllPlayers[i].transform.position + (Vector3.up*2f);
				}
			}
			if (numPlayers == 0) {
				return;
			}

			averagePosition /= numPlayers;
			greatestDistance = 0;

			for(int i = 0; i < numPlayers; i++) 
			{
				float tempDist = Vector3.Distance(GameManager.instance.AllPlayers[i].transform.position, averagePosition);
				if(tempDist > greatestDistance) 
				{
					greatestDistance = tempDist;
				}
			}
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(averagePosition.x, averagePosition.y, (-1.3f)*(greatestDistance+4)), Time.deltaTime*speed);
			targetPos = transform.position;

			if(blurCam) blurCam.fieldOfView = Mathf.MoveTowards(blurCam.fieldOfView,60f,Time.deltaTime*5f);
			GetComponent<Camera>().fieldOfView = Mathf.MoveTowards(GetComponent<Camera>().fieldOfView,60f,Time.deltaTime*5f);
		} else {
			transform.position = Vector3.MoveTowards(transform.position, startingPos, Time.deltaTime*5f);
		}

	}
}