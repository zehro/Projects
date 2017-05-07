using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves, rotates, and scales this object to follow the given transform as smoothly as possible.
// If a rigidbody is attached to this object, it moves it in the correct way.

namespace YeggQuest.NS_Generic
{
    [ExecuteInEditMode]
    public class Follower : MonoBehaviour
    {
        public Transform follow;

        private Rigidbody body;
        private Vector3 initScale;

        void Awake()
        {
            body = GetComponent<Rigidbody>();

            if (body != null)
            {
                body.collisionDetectionMode = CollisionDetectionMode.Continuous;
                //body.interpolation = RigidbodyInterpolation.Interpolate;
                body.isKinematic = true;
            }

            initScale = transform.localScale;
        }

        void LateUpdate()
        {
            if (follow == null)
                return;

            // If the game isn't playing, if there's no rigidbody, or if the following
            // scale isn't 1 (which messes up rigidbody interpolation), just move the
            // object by moving its position and rotation normally

            if (!Application.isPlaying || body == null || follow.localScale != Vector3.one)
            {
                transform.position = follow.position;
                transform.rotation = follow.rotation;
            }

            if (Application.isPlaying)
                transform.localScale = Vector3.Scale(initScale, follow.localScale);
        }

        void FixedUpdate()
        {
            // Otherwise, use MovePosition and MoveRotation so that objects on top
            // of the moving rigidbody display proper inertial behavior

            if (Application.isPlaying && body != null && follow.localScale == Vector3.one)
            {
                body.MovePosition(follow.position);
                body.MoveRotation(follow.rotation);
            }
        }
    }
}