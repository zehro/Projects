using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: comments

namespace YeggQuest.NS_Spline
{
    [ExecuteInEditMode]
    public class SplineFollower : MonoBehaviour
    {
        public SplineWrapper wrapper;

        [Space(10)]
        [Header("Movement Behavior")]
        public float movementDuration = 1f;
        public float movementPause = 0f;
        public float movementSync = 0f;
        public SplineMovementType movementType;

        [Space(10)]
        [Header("Scaling Behavior")]
        public float appearTime = 0;
        public float disappearTime = 0;

        [Space(10)]
        [Header("Smoothing Behavior")]
        public SplineMovementSmoothing movementSmoothing;
        public bool rotationSmoothing = true;

        bool playing = false;
        SplineLerpResult result;

        public float t;
        public int currenSection;
        public int currentSegment;

        void FixedUpdate()
        {
            if (wrapper == null)
                return;

            if (!playing)
                return;

            t += Time.deltaTime;
            float scale = (Application.isPlaying ? FollowScale(t) : 1);
            result = FollowLerp(t);
            currenSection = result.section;
            currentSegment = result.segment;
            transform.position = result.worldPosition;
            transform.rotation = Quaternion.Euler(result.worldRotation);
            transform.localScale = Vector3.one * scale;
        }

        void OnDrawGizmos()
        {
            if (wrapper == null)
                return;

            if (!playing)
                return;

            float t = (Application.isPlaying ? Time.time : Time.realtimeSinceStartup) + movementSync;
            SplineLerpResult result = FollowLerp(t);

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(result.worldPosition, Quaternion.Euler(result.worldRotation), Vector3.one * FollowScale(t));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.5f);
            Gizmos.matrix = Matrix4x4.identity;
        }

        void OnValidate()
        {
            if (!playing)
                return;
            movementDuration = Mathf.Max(0.1f, movementDuration);
            movementPause = Mathf.Max(0, movementPause);
            appearTime = Mathf.Max(0, appearTime);
            disappearTime = Mathf.Max(0, disappearTime);
        }

        // ======================================================================================================================== HELPERS

        SplineLerpResult FollowLerp(float time)
        {
            if (!playing)
                return new SplineLerpResult();
            SplineLerpQuery query = new SplineLerpQuery();

            float duration = movementDuration + movementPause;
            float x = time * (movementType == SplineMovementType.PingPong ? 2 : 1);
            float t = Mathf.Clamp01(Mathf.Repeat(x, duration) / movementDuration);

            switch (movementType)
            {
                case SplineMovementType.Forward:
                    query.t = t;
                    break;

                case SplineMovementType.Backward:
                    query.t = 1 - t;
                    break;

                case SplineMovementType.PingPong:
                    if (Mathf.Repeat(time, duration) / duration < 0.5f)
                        query.t = t;
                    else
                        query.t = 1 - t;
                    break;
            }

            query.movementSmoothing = movementSmoothing;
            query.valueSmoothing = rotationSmoothing;

            return wrapper.Lerp(query);
        }

        float FollowScale(float time)
        {
            if (!playing)
                return 0;

            float duration = movementDuration + movementPause;
            float t = Mathf.Repeat(time, duration);

            if (movementType == SplineMovementType.PingPong)
                duration -= movementPause / 2;

            float scale = 1;
            if (appearTime > 0)
                scale = Mathf.Min(scale, (t) / appearTime);
            if (disappearTime > 0)
                scale = Mathf.Min(scale, (duration - t) / disappearTime);
            scale = Mathf.Max(scale, 0);

            return Mathf.SmoothStep(0, 1, scale);
        }

        public void Playing(bool play)
        {
            playing = play;
        }

        /// <summary>
        /// Method to get the tangent of the most
        /// recent SplineLerpResult in PlayMode
        /// </summary>
        public Vector3 getCurrentTangent()
        {
            return result.tangent;
        }
    }
}