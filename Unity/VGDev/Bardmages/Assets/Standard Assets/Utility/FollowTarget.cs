using System;
using UnityEngine;


namespace UnityStandardAssets.Utility
{
    public class FollowTarget : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0f, 7.5f, 0f);


        private void LateUpdate()
        {
			if(target) {
				if(target.GetComponent<Rigidbody>()) {
					transform.position = target.position + offset + target.GetComponent<Rigidbody>().velocity.normalized;
				} else {
					transform.position = target.position + offset;
				}
			}
        }
    }
}
