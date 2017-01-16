using UnityEngine;
using System.Collections;

public class SquidTransportShip : MonoBehaviour
{
    public float length;
    public float speed;

	void Start()
	{
		Destroy(gameObject, length);
	}

	void Update()
	{
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
	}
}