using UnityEngine;
using System.Collections;

public class PowerupRotate : MonoBehaviour
{
	void Update()
	{
		float t = Time.time;
		float s = transform.position.x;
		s += transform.position.y;
		s += transform.position.z;
		transform.localRotation = Quaternion.Euler(s+t*67,s+t*101,s+t*73);
	}
}