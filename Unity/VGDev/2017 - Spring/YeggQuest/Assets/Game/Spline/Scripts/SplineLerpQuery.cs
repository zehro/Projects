using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Spline
{
    // The parameter of Spline.Lerp() - a SplineLerpQuery struct which
    // contains all the information about how the spline should be interpolated.
    // This includes the parametrized value and some smoothing information.

    internal struct SplineLerpQuery
    {
        public float t;                                     // The parametrized time-position on the spline
        public SplineMovementSmoothing movementSmoothing;   // What kind of smoothing to apply to movement (between nodes, whole spline)
        public bool valueSmoothing;                         // Whether or not to smooth the other values (rotation, etc)

        public SplineLerpQuery(float t)
        {
            this.t = t;
            this.movementSmoothing = SplineMovementSmoothing.Off;
            this.valueSmoothing = false;
        }

        public SplineLerpQuery(float t, SplineMovementSmoothing movementSmoothing, bool valueSmoothing)
        {
            this.t = t;
            this.movementSmoothing = movementSmoothing;
            this.valueSmoothing = valueSmoothing;
        }
    }
}