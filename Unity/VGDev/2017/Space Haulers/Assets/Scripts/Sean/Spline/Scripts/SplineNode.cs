using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Spline is constructed of many SplineNodes, which are attached as children
// to a SplineWrapper in the scene.
// TODO: comments

namespace YeggQuest.NS_Spline
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class SplineNode : MonoBehaviour
    {
        [Range(0, 30)]
        public float torsion = 3;
        [Range(0.2f, 5f)]
        public float speed = 1;
        [Range(10, 100)]
        public float fieldOfView = 50;
        public Vector3 worldOrientation;
        
        private Vector3 posPrev;
        private Quaternion rotPrev;
        private float torPrev;
        private float spdPrev;

        void OnEnable()
        {
            posPrev = transform.localPosition;
            rotPrev = transform.localRotation;
            torPrev = torsion;
            spdPrev = speed;
        }

        public bool CustomUpdate()
        {
            transform.localScale = Vector3.one;

            if (posPrev != transform.localPosition
            || rotPrev != transform.localRotation
            || torPrev != torsion
            || spdPrev != speed)
            {
                posPrev = transform.localPosition;
                rotPrev = transform.localRotation;
                torPrev = torsion;
                spdPrev = speed;

                return true;
            }

            return false;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Spline.SpeedColor(speed);
            Gizmos.DrawSphere(transform.position, 0.25f);

            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.Euler(worldOrientation), Vector3.one * 0.2f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero, Vector3.right);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, Vector3.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Vector3.zero, Vector3.forward);
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}