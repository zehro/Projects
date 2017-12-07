using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Cam
{
    public class CamStrategyRailLook : CamStrategy
    {
        public Vector3 startPos;
        public Vector3 endPos;
        public Vector3 offsetPos;
        public Transform target;
        [Range(0, 1)]
        public float targetFollow;
        [Range(0, 1)]
        public float targetLook;
        public float fov;

        private Vector3 anchor;
        private Vector3 lookAt;

        void OnDrawGizmosSelected()
        {
            Yutil.DrawArrow(startPos, endPos, Color.green);
            Yutil.DrawArrow(startPos, startPos + offsetPos, Color.blue);
        }

        public override CamStrategyResult Direct()
        {
            CamStrategyResult result = new CamStrategyResult();

            Vector3 project = startPos + Vector3.Project(target.position - startPos, endPos - startPos);
            Vector3 midpoint = Vector3.Lerp(startPos, endPos, 0.5f);
            
            if (Vector3.Distance(midpoint, project) > Vector3.Distance(startPos, endPos) / 2)
            {
                if (Vector3.Distance(startPos, project) < Vector3.Distance(endPos, project))
                    project = startPos;
                else
                    project = endPos;
            }

            float dt = Time.deltaTime * 60;
            float drag = 0.1f;
            anchor = Vector3.Lerp(anchor, Vector3.Lerp(project, target.position, targetFollow), dt * drag);
            lookAt = Vector3.Lerp(lookAt, Vector3.Lerp(project, target.position, targetLook), dt * drag);

            result.anchorPosition = anchor;
            result.offsetPosition = offsetPos;
            result.lookAtPosition = lookAt;
            result.roll = 0;
            result.fov = fov;

            return result;
        }
    }
}