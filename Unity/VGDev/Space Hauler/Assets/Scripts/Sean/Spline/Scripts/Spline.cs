using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A Spline is a mathematical construct at the core of many elements in YeggQuest, like pipe traversal,
// mesh generation for pipes and wires, and camera cutscene movement. Given a list of SplineNodes, a precision,
// and an optional gap between each of the nodes, a Spline calculates a single curved path which connects them,
// where the rotation of each node controls the direction of the curve and its torsion controls the curvature.
// Internally, this involves calculating vertices, tangents, normals, lengths, and a total length.

namespace YeggQuest.NS_Spline
{
    public class Spline
    {
        public SplineNode[] nodes;      // the SplineNodes that make up this spline

        public int sectionCount;        // how many total sections the whole spline is made up of (each is one cubic spline)
        public int segmentCount;        // how many segments each section is split up into (based on precision)
        public Vector3[,] vertices;     // every center vertex, organized by [section, segment]
        public Vector3[,] tangents;     // every tangent, organized by [section, segment]
        public Vector3[,] normals;      // every normal, organized by [section, segment]
        public float[,] speeds;         // every speed, organized by [section, segment] with optional interpolation
        public float[,] lengths;        // every length, organized by [section, segment] and cumulative within sections
        public float totalLength;       // the length of all sections added together (sum of last column of lengths[,])

        // Constructor which creates a new Spline and defines all the public data

        // nodes            the (n) SplineNodes with positions, rotations, and torsions defining (n-1) sections
        // precision        how many segments make up a section
        // gap              the physical gap separating sections on both sides

        public Spline(SplineNode[] nodes, int precision, float gap)
        {
            this.nodes = nodes;
            sectionCount = nodes.Length - 1;
            segmentCount = precision + 2;
            vertices = new Vector3[sectionCount, segmentCount];
            tangents = new Vector3[sectionCount, segmentCount];
            normals = new Vector3[sectionCount, segmentCount];
            speeds = new float[sectionCount, segmentCount];
            lengths = new float[sectionCount, segmentCount];

            Quaternion inv = Quaternion.Inverse(nodes[0].transform.parent.rotation);

            for (int i = 0; i < sectionCount; i++)
            {
                Transform sPos = this.nodes[i].transform;
                Transform ePos = this.nodes[i + 1].transform;
                Vector3 p0 = sPos.localPosition + inv * sPos.forward * gap;
                Vector3 p1 = ePos.localPosition - inv * ePos.forward * gap;
                Vector3 m0 = inv * sPos.forward * this.nodes[i].torsion;
                Vector3 m1 = inv * ePos.forward * this.nodes[i + 1].torsion;

                for (int j = 0; j < segmentCount; j++)
                {
                    float t = Mathf.Clamp01(Mathf.InverseLerp(1, precision, j));

                    if (j == 0)
                        vertices[i, j] = sPos.localPosition;
                    else if (j == segmentCount - 1)
                        vertices[i, j] = ePos.localPosition;
                    else
                        vertices[i, j] = CalculateSpline(p0, p1, m0, m1, t);

                    tangents[i, j] = CalculateSplineDerivative(p0, p1, m0, m1, t);
                    normals[i, j] = -Vector3.Cross(tangents[i, j], Vector3.Lerp(inv * sPos.right, inv * ePos.right, t)).normalized;
                    speeds[i, j] = Mathf.Lerp(nodes[i].speed, nodes[i + 1].speed, t);
                    if (j > 0)
                        lengths[i, j] = lengths[i, j - 1] + Vector3.Distance(vertices[i, j], vertices[i, j - 1]) / speeds[i, j];
                }

                totalLength += lengths[i, segmentCount - 1];
            }
        }

        // ======================================================================================================================== GETTERS

        // This is the crucial function for splines: the one which lets outside objects query for interpolated data.
        // To do so, they pass in a root transform to use as the owner of the spline, as well as a SplineLerpQuery,
        // which contains the desired parametrized time [0-1] on the spline and some additional interpolation settings.
        // This function then returns a SplineLerpResult which contains all of the interpolated information.

        internal SplineLerpResult Lerp(Transform root, SplineLerpQuery query)
        {
            // Failsafe - if this spline is degenerate, just return the first point and don't do any other interpolation

            if (totalLength == 0)
            {
                SplineLerpResult degenerateResult = new SplineLerpResult();
                degenerateResult.worldPosition = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale).MultiplyPoint3x4(vertices[0, 0]);
                return degenerateResult;
            }
            
            // First, we need to find the section on the spline that contains the parametrized time being queried for.
            // Convert the parametrized time given into a time-position within the total length of the spline

            float p = Mathf.Clamp01(query.t) * totalLength;

            // If the query says to smooth along the entire spline, do that now

            if (query.movementSmoothing == SplineMovementSmoothing.WholeSpline)
                p = Mathf.SmoothStep(0, totalLength, p / totalLength);

            // Now, find the section of the spline that corresponds to that time-position, by using the total lengths
            // of each section. In doing so, make the time-position p local to that section

            int section;
            for (section = 0; section < sectionCount - 1; section++)
            {
                if (p > lengths[section, segmentCount - 1])
                    p -= lengths[section, segmentCount - 1];
                else
                    break;
            }

            // Now that p is located within a section (between two SplineNodes), we need to do more parameter calculation.
            // v is the [0-1] parameter which will lerp the non-positional values between the nodes, with optional value smoothing if the query says to
            // p is also updated so that it is smoothed between the nodes if the query says to

            float v = p / lengths[section, segmentCount - 1];
            if (query.movementSmoothing == SplineMovementSmoothing.BetweenNodes)
                p = Mathf.SmoothStep(0, lengths[section, segmentCount - 1], v);
            if (query.valueSmoothing)
                v = Mathf.SmoothStep(0, 1, v);

            // Finally, find the segment within the found section that corresponds to p, by using the
            // cumulative segment lengths. In doing so, make p local to that segment

            int segment;
            for (segment = 0; segment < segmentCount - 1; segment++)
            {
                float l1 = lengths[section, segment];
                float l2 = lengths[section, segment + 1];
                float r = (p - l1) / (l2 - l1);

                if (r >= 0 && r <= 1 || segment == segmentCount - 2)
                {
                    p = r;
                    break;
                }
            }

            // Now that we finally know where we are in terms of sections and segments, we can do the interpolation.

            SplineLerpResult result = new SplineLerpResult();

            // Position

            Vector3 localPos = Vector3.Lerp(vertices[section, segment], vertices[section, segment + 1], p);
            Matrix4x4 m = Matrix4x4.TRS(root.position, root.rotation, root.lossyScale);
            result.worldPosition = m.MultiplyPoint3x4(localPos);

            // Values
            SplineNode s = nodes[section];
            SplineNode e = nodes[section + 1];
            result.worldRotation = Vector3.Lerp(s.worldOrientation, e.worldOrientation, v);
            result.fieldOfView = Mathf.Lerp(s.fieldOfView, e.fieldOfView, v);
            result.tangent =  (Tangent(vertices, section, segment, segmentCount, 0)
                + Tangent(vertices, section, segment, segmentCount, 1)) / 2;
            result.section = section;
            result.segment = segment;

            return result;
        }

        // Tangent helper function
        public static Vector3 Tangent(Vector3[,] vertices, int section, int segment, int segmentCount, int offset)
        {
            return vertices[section, Mathf.Clamp(segment + 1 + offset, 0, segmentCount)]
                - vertices[section, Mathf.Clamp(segment - 1 + offset, 0, segmentCount)];
        }


        // Gets the entrance into the spline.

        internal Transform GetEntrance()
        {
            return nodes[0].transform;
        }

        // Gets the exit out of the spline.

        internal Transform GetExit()
        {
            return nodes[nodes.Length - 1].transform;
        }

        // Gets the total length of the spline.

        internal float GetLength()
        {
            return totalLength;
        }

        // ======================================================================================================================== MESSAGES

        // A method that lets an object that owns a spline draw it in the editor.

        internal void Draw(Transform root)
        {
            Gizmos.color = Color.white;
            Gizmos.matrix = Matrix4x4.TRS(root.position, root.rotation, root.localScale);

            for (int i = 0; i < sectionCount; i++)
            {
                for (int j = 0; j < segmentCount - 1; j++)
                {
                    Gizmos.color = SpeedColor(speeds[i, j]);
                    Gizmos.DrawLine(vertices[i, j], vertices[i, j + 1]);
                }
            }

            Gizmos.matrix = Matrix4x4.identity;
        }

        // ======================================================================================================================== HELPERS

        // Core math function which evaluates the given cubic spline
        // (defined by two endpoints and their tangents) at time t.

        public static Vector3 CalculateSpline(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, float t)
        {
            t = Mathf.Clamp01(t);
            float t3 = t * t * t;
            float t2 = t * t;

            return (2 * t3 - 3 * t2 + 1) * p0
                 + (t3 - 2 * t2 + t) * m0
                 + (-2 * t3 + 3 * t2) * p1
                 + (t3 - t2) * m1;
        }

        // Core math function which evaluates the derivative of the given cubic spline
        // (defined by two endpoints and their tangents) at time t. This is useful
        // because the spline derivative is the tangent at time t.

        public static Vector3 CalculateSplineDerivative(Vector3 p0, Vector3 p1, Vector3 m0, Vector3 m1, float t)
        {
            t = Mathf.Clamp01(t);
            float t2 = t * t;

            return (6 * t2 - 6 * t) * p0
                 + (3 * t2 - 4 * t + 1) * m0
                 + (-6 * t2 + 6 * t) * p1
                 + (3 * t2 - 2 * t) * m1;
        }

        // Returns the color associated with the given speed factor.

        public static Color SpeedColor(float speed)
        {
            if (speed < 1)
                return Color.Lerp(Color.blue, Color.white, Mathf.InverseLerp(0.2f, 1f, speed));
            else
                return Color.Lerp(Color.white, Color.red, Mathf.InverseLerp(1f, 5f, speed));
        }
    }
}