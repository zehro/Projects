using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Cam
{
    [DisallowMultipleComponent]
    public abstract class CamStrategy : MonoBehaviour
    {
        public abstract CamStrategyResult Direct();
    }

    public struct CamStrategyResult
    {
        public Vector3 anchorPosition;      // Where the camera should be anchored (in world space)
        public Vector3 offsetPosition;      // How the camera should be offset from the anchor position (in world space)
        public Vector3 lookAtPosition;      // Where the camera should be looking
        public float roll;                  // The roll of the camera
        public float fov;                   // The field of view the camera should have

        public CamStrategyResult(Vector3 anchorPosition, Vector3 offsetPosition, Vector3 lookAtPosition, float roll, float fov)
        {
            this.anchorPosition = anchorPosition;
            this.offsetPosition = offsetPosition;
            this.lookAtPosition = lookAtPosition;
            this.roll = roll;
            this.fov = fov;
        }
    }
}