using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Spline
{
    // The response of Spline.Lerp() - a SplineLerpResult struct which contains all the
    // interpolated information, including the resultant position and other custom data.

    internal struct SplineLerpResult
    {
        public Vector3 worldPosition;       // the interpolated world position on the spline
        public Vector3 worldRotation;       // the interpolated world rotation
        public Vector3 tangent;             // the interpolated tangent
        public float fieldOfView;           // the interpolated field of view (for cameras following splines)
        public int section;
        public int segment;
    }
}