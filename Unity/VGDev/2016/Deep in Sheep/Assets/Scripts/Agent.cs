using UnityEngine;
using System.Collections;

/// <summary>
/// Basic Nav AI for sheep
/// </summary>
public class Agent : MonoBehaviour {
    NavMeshAgent agent;
    public float range;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Direct the sheep to a certain location
    public void moveToLocation(Vector3 destination) {
        agent.SetDestination(destination);
    }

    // Move the sheep to a random location close to original location
    public void randomLocation() {
        //GetComponent<NavMeshAgent>().enabled = true;
        Vector3 random = new Vector3(transform.position.x + Random.Range(-range, range), transform.position.y, transform.position.z + Random.Range(-range, range));
        moveToLocation(random);
    }

    public NavMeshAgent getAgent() {
        return agent;
    }

    public void disable() {
        agent.enabled = false;
    }

    public void enable() {
        agent.enabled = true;
    }
}
