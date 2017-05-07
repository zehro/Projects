using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Logic;

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

        [Space(10)]
        [Header("Input Behavior")]
        public Logical input;
        public SplineInputBehavior inputBehavior;
        public float inputAccelerationTime;

        private float time = 0;
        private float timeSpeed = 1;

        void Start()
        {
            timeSpeed = Logic.SafeEvaluate(input, true) ? 1 : 0;
        }

        void FixedUpdate()
        {
            if (wrapper == null)
                return;

            // In play mode, make this object follow the spline as expected,
            // but just set it to the beginning at full scale when in the editor.

            float scale;
            SplineLerpResult result;

            if (Application.isPlaying)
            {
                // Find out how fast the follower should be moving

                float targSpeed = Logic.SafeEvaluate(input, true) ? 1 : 0;

                if (inputAccelerationTime == 0)
                    timeSpeed = targSpeed;
                else
                    timeSpeed = Mathf.MoveTowards(timeSpeed, targSpeed, Time.smoothDeltaTime / inputAccelerationTime);

                // Move the time variable accordingly

                if (inputBehavior == SplineInputBehavior.ControlSpeed)
                    time += Time.smoothDeltaTime * timeSpeed;
                else
                    time += Time.smoothDeltaTime * (timeSpeed * 2 - 1);
                if (inputBehavior == SplineInputBehavior.ControlDirectionClamped)
                    time = Mathf.Clamp(time, 0, movementDuration + movementPause);

                // Interpolate based on the time
                
                scale = FollowScale(time + movementSync);
                result = FollowLerp(time + movementSync);
            }

            else
            {
                scale = 1;
                result = FollowLerp(0);
            }
            
            transform.position = result.worldPosition;
            transform.rotation = Quaternion.Euler(result.worldRotation);
            transform.localScale = Vector3.one * scale;
        }

        void OnDrawGizmos()
        {
            if (wrapper == null)
                return;

            // When drawing gizmos, use the editor time (ignoring input behavior) when in the editor,
            // but make it properly match up when in play mode

            float t = (Application.isPlaying ? time : Time.realtimeSinceStartup) + movementSync;
            SplineLerpResult result = FollowLerp(t);

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(result.worldPosition, Quaternion.Euler(result.worldRotation), Vector3.one * FollowScale(t));
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 0.5f);
            Gizmos.matrix = Matrix4x4.identity;

            Logic.Visualize(transform, input);
        }

        void OnValidate()
        {
            movementDuration = Mathf.Max(0.1f, movementDuration);
            movementPause = Mathf.Max(0, movementPause);
            appearTime = Mathf.Max(0, appearTime);
            disappearTime = Mathf.Max(0, disappearTime);
        }

        // ======================================================================================================================== HELPERS

        SplineLerpResult FollowLerp(float time)
        {
            SplineLerpQuery query = new SplineLerpQuery();

            float duration = movementDuration + movementPause;
            float x = time * (movementType == SplineMovementType.PingPong ? 2 : 1);
            if (inputBehavior != SplineInputBehavior.ControlDirectionClamped || movementType == SplineMovementType.PingPong)
                x = Mathf.Repeat(x, duration);
            float t = Mathf.Clamp01(x / movementDuration);

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
    }
}