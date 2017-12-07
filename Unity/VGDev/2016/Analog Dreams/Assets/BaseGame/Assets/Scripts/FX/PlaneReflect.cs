using UnityEngine;
using System.Collections;

public class PlaneReflect : MonoBehaviour
{
    public Transform follow;
    public Vector3 origin;
    public Vector3 normal;

	void Update()
    {
        transform.position = Vector3.Reflect(follow.position - origin, normal);
	}
}