using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour 
{

	public enum Axis 
	{
		X = 0,
		Y = 1,
		Z = 2,
	}

	public float speed = 5f;
	public Axis rotationAxis = Axis.X;
	public bool worldSpace = true;
	public float pause = 0f;

	private float pauseTimer = 0f;
	private bool isPaused = false;

	// Use this for initialization
	void Start () 
	{
		pauseTimer = pause;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(pauseTimer > 0f) 
		{
			pauseTimer -= Time.deltaTime;
		}
		else
		{
			if(rotationAxis == Axis.X) 
			{
				if(worldSpace) 
				{
					transform.Rotate(new Vector3(speed, 0f, 0f), Space.World);
					if(transform.eulerAngles.x == 0) pauseTimer = pause;
				} 
				else 
				{
					transform.Rotate(new Vector3(speed, 0f, 0f), Space.Self);
					if(transform.localEulerAngles.x == 0) pauseTimer = pause;
				}
			} 
			else if(rotationAxis == Axis.Y)
			{
				if(worldSpace)
				{
					transform.Rotate(new Vector3(0f, speed, 0f), Space.World);
					if(transform.eulerAngles.y == 0) pauseTimer = pause;
				}
				else
				{
					transform.Rotate(new Vector3(0f, speed, 0f), Space.Self);
					if(transform.localEulerAngles.y == 0) pauseTimer = pause;
				}
			} 
			else if(rotationAxis == Axis.Z) 
			{
				if(worldSpace)
				{
					transform.Rotate(new Vector3(0f, 0f, speed), Space.World);
					if(transform.eulerAngles.z == 0) pauseTimer = pause;
				} 
				else 
				{
					transform.Rotate(new Vector3(0f, 0f, speed), Space.Self);
					if(transform.localEulerAngles.z == 0) pauseTimer = pause;
				}
			}
		}
	}
}
