using UnityEngine;
using System.Collections;

// From Space Squids
public class ObjectRotator : MonoBehaviour
{
    public float RotateXSpeed;
    public float RotateYSpeed;
    public float RotateZSpeed;
	Vector3 rotation;

	void Awake()
	{
		rotation = new Vector3(RotateXSpeed, RotateYSpeed, RotateZSpeed);
	}

	void Update()
	{
		transform.Rotate(rotation);
	}
}