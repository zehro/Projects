using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {
    public float strength;
    public float deathDistance = 50f;
    private float force;
    private Vector3 pull;
    private Vector3 dir;

	// Use this for initialization
	void Start () {
        GetComponent<Collider>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player")
            && other.gameObject.GetComponent<TruckController>().getState() == 0)
        {
            pull = transform.position - other.GetComponent<Rigidbody>().position;
            if (pull.magnitude < deathDistance)
            {
                LevelManager.instance.gameOver();
                return;
            }
            force = strength / pull.magnitude;
            dir = pull.normalized;
            other.gameObject.GetComponent<Rigidbody>().AddRelativeForce(force * dir, ForceMode.Force);
        }
    }
}
