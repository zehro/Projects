using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Cam
{
    public class CamStrategyTargetLook : CamStrategy
    {
        public Vector3 position;
        public Transform target;
        public float fov;

        void OnDrawGizmosSelected()
        {
            if (target == null)
                return;

            Vector3 p = transform.position + position;
            Gizmos.matrix = Matrix4x4.TRS(p, Quaternion.LookRotation(target.position - p), Vector3.one);
            Gizmos.DrawFrustum(Vector3.zero, fov, 2.5f, 0.1f, 16f / 9f);
            Gizmos.DrawFrustum(Vector3.zero, fov, 10f, 0.1f, 16f / 9f);
        }

        public override CamStrategyResult Direct()
        {
            CamStrategyResult result = new CamStrategyResult();

            result.anchorPosition = transform.position;
            result.offsetPosition = position;
            result.lookAtPosition = target.position;
            result.roll = 0;
            result.fov = fov;

            return result;
        }
    }
}