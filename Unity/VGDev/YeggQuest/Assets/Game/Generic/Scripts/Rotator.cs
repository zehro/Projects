using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Logic;

// Rotates this object around the given axis at the given speed.
// If a rigidbody is attached to this object, it rotates it in the correct way.

namespace YeggQuest.NS_Generic
{
    [ExecuteInEditMode]
    public class Rotator : MonoBehaviour
    {
        [Header("Rotation Behavior")]
        public Vector3 axis = Vector3.up;
        public float degreesPerSecond = 30;

        [Space(10)]
        [Header("Input Behavior")]
        public Logical input;
        public RotatorInputBehavior inputBehavior;
        public float inputAccelerationTime;

        private Rigidbody body;
        private Quaternion initRot;
        private float time;
        private float timeSpeed;

        void Awake()
        {
            body = GetComponent<Rigidbody>();

            if (body != null)
            {
                body.collisionDetectionMode = CollisionDetectionMode.Continuous;
                body.interpolation = RigidbodyInterpolation.Interpolate;
                body.isKinematic = true;
            }

            initRot = transform.rotation;
        }

        void Update()
        {
            if (!Application.isPlaying)
                initRot = transform.rotation;

            else
            {
                // Find out how fast the rotator should be spinning

                float targSpeed = Logic.SafeEvaluate(input, true) ? 1 : 0;

                if (inputAccelerationTime == 0)
                    timeSpeed = targSpeed;
                else
                    timeSpeed = Mathf.MoveTowards(timeSpeed, targSpeed, Time.smoothDeltaTime / inputAccelerationTime);

                // Move the time variable accordingly

                if (inputBehavior == RotatorInputBehavior.ControlSpeed)
                    time += Time.smoothDeltaTime * timeSpeed;
                else
                    time += Time.smoothDeltaTime * (timeSpeed * 2 - 1);

                // Rotate based on the time

                Quaternion rot = GetRotation(time);
                
                if (body == null)
                    transform.rotation = rot;
                else
                    body.MoveRotation(rot);
            }
        }

        void OnDrawGizmos()
        {
            // When drawing gizmos, use the editor time (ignoring input behavior) when in the editor,
            // but make it properly match up when in play mode

            Vector3 a = axis.normalized;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position - a, transform.position + a);
            Gizmos.DrawSphere(transform.position - a, 0.05f);
            Gizmos.DrawSphere(transform.position + a, 0.05f);

            float t = (Application.isPlaying ? time : Time.realtimeSinceStartup);

            Gizmos.matrix = Matrix4x4.TRS(transform.position, GetRotation(t), Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.5f);
            Gizmos.matrix = Matrix4x4.identity;

            Logic.Visualize(transform, input);
        }

        void OnValidate()
        {
            if (axis == Vector3.zero)
                axis = Vector3.up;
        }

        Quaternion GetRotation(float t)
        {
            return Quaternion.AngleAxis(degreesPerSecond * t, axis) * initRot;
        }
    }

    public enum RotatorInputBehavior
    {
        ControlSpeed, ControlDirection
    }
}