using UnityEngine;
using System.Collections;

public class GrabandDrop : MonoBehaviour {

    public float distance;
    public float speedFactor;

    PlayerController control;
    Transform player;
    GameObject grabbedObject;
    //float grabbedObjectSize;

    // Use this for initialization
    void Start () {
        player = transform;
        control = GetComponent<PlayerController>();
	}
   
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("e"))
        {
            if (grabbedObject == null)
                TryGrabObject(GetObjectInFrontOfPlayer(5));
            else
                DropObject();
        }
        if (grabbedObject != null)
        {
            Vector3 newPosition = gameObject.transform.position + player.forward * distance + player.up * 2;
            grabbedObject.transform.position = newPosition;
        }
    }

    GameObject GetObjectInFrontOfPlayer(float range)
    {
        Vector3 position = gameObject.transform.position;
        RaycastHit raycastHit;
        Vector3 target = position + player.forward * range;
        if(Physics.Linecast(position, target, out raycastHit))
        {
            return raycastHit.collider.gameObject;
        }
        return null;
    }

    void TryGrabObject(GameObject grabObject)
    {
        if (grabObject == null || !CanGrab(grabObject))
        {
            return;
        }
        grabbedObject = grabObject;
        control.moveForce = control.moveForce / speedFactor;
        //grabbedObjectSize = grabObject.GetComponent<Renderer>().bounds.size.magnitude;
    }

    bool CanGrab(GameObject candidate)
    {
        return candidate.GetComponent<Rigidbody>() != null;
    }

    void DropObject()
    {
        if (grabbedObject == null)
            return;

        if (grabbedObject.GetComponent<Rigidbody>() != null)
        {
            grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            grabbedObject.GetComponent<Rigidbody>().position = player.position + player.forward * distance;
        }
        grabbedObject = null;
        control.moveForce = speedFactor * control.moveForce;
    }

}
