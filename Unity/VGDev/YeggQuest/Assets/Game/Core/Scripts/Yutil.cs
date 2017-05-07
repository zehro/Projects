using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Obligatory static class containing globally
// useful functions and calculations.

namespace YeggQuest
{
    public static class Yutil
    {
        // A function which takes in a world-space point, converts it
        // to the local space of the given transform, applies the given
        // transform's local scale, and returns the result in world space

        public static Vector3 ApplyScale(Transform transform, Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            localPoint.Scale(transform.localScale);
            return transform.TransformPoint(localPoint);
        }

        // A function which takes in a world-space point, converts it
        // to the local space of the given transform, removes the given
        // transform's local scale, and returns the result in world space

        public static Vector3 RemoveScale(Transform transform, Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            localPoint.x /= transform.localScale.x;
            localPoint.y /= transform.localScale.y;
            localPoint.z /= transform.localScale.z;
            return transform.TransformPoint(localPoint);
        }

        // Returns the yaw a vector has (Y rotation)

        public static float VectorYaw(Vector3 ray)
        {
            return Mathf.Atan2(ray.x, ray.z) * Mathf.Rad2Deg;
        }

        // Takes in a coordinate space (as defined by the given space transform and orientation) and a forward vector,
        // and returns both the pitch and the yaw of that vector's rotation from the coordinate space's forward.
        // Essentially, a specialized LookAt that is parametrized in a way useful for axially clamped look behavior.
        // (Note the result is returned in the float[] result variable, to avoid unnecessary memory allocation.)

        public static void CalculatePitchYaw(Transform space, Quaternion spaceOrientation, Vector3 forward, float[] result)
        {
            space.rotation *= spaceOrientation;

            Vector3 xz = Vector3.ProjectOnPlane(forward, space.up);
            Vector3 y = Vector3.ProjectOnPlane(forward, space.right);
            y *= Mathf.Sign(Vector3.Dot(y, space.forward));

            result[0] = Vector3.Angle(xz, forward) * Mathf.Sign(Vector3.Dot(forward, space.up)) * -1;
            result[1] = Vector3.Angle(y, forward) * Mathf.Sign(Vector3.Dot(forward, space.right));

            space.rotation *= Quaternion.Inverse(spaceOrientation);
        }

        // Ken Perlin's smootherstep.

        public static float Smootherstep(float t)
        {
            float x = Mathf.Clamp01(t);
            return x * x * x * (x * (x * 6 - 15) + 10);
        }

        // Draw a gizmo arrow from point A to point B, using the given color and an optional gap.

        public static void DrawArrow(Vector3 start, Vector3 end, Color color, float gap = 0)
        {
            float a = Mathf.PingPong(Time.realtimeSinceStartup, 1);
            a = Mathf.SmoothStep(0, 1, a) * 0.15f;

            start = Vector3.MoveTowards(start, end, gap + a);
            end = Vector3.MoveTowards(end, start, gap + a);

            if (start != end)
            {
                Vector3 es = Vector3.Normalize(start - end);
                Vector3 left = Vector3.Cross(es, es.x == 0 ? Vector3.right : Vector3.up);
                Vector3 leftDiagonal = Vector3.Normalize(es + left / 2f) / 5f;
                Vector3 rightDiagonal = Vector3.Normalize(es - left / 2f) / 5f;

                Gizmos.color = color;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawLine(end, end + leftDiagonal);
                Gizmos.DrawLine(end, end + rightDiagonal);
            }
        }
    }
}