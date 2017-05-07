using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;

public class CameraRaycaster : MonoBehaviour {
    GameObject o;
    
	void FixedUpdate () {
		if (InputManager.GetButton("Submit") && Cast())
        {
            o.GetComponent<CameraRaycastTarget>().Trigger();
        }
	}

    bool Cast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, 2f))
        {
            o = hit.transform.gameObject;
            if (o != null)
            {
                return true;
            }
        }
        return false;
    }
}
