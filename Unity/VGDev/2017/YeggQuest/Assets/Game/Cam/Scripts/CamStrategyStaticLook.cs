using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Cam
{
    public class CamStrategyStaticLook : CamStrategy
    {
        public Vector3 position;
        public Vector3 rotation;
        public float fov;
        
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position + position, Quaternion.Euler(rotation), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, fov, 2.5f, 0.1f, 16f / 9f);
            Gizmos.DrawFrustum(Vector3.zero, fov, 10f, 0.1f, 16f / 9f);
        }

        public override CamStrategyResult Direct()
        {
            CamStrategyResult result = new CamStrategyResult();

            result.anchorPosition = transform.position;
            result.offsetPosition = position;
            result.lookAtPosition = transform.position + position + Quaternion.Euler(rotation) * Vector3.forward;
            result.roll = rotation.z;
            result.fov = fov;

            return result;
        }
    }
}