using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Spline;

namespace YeggQuest.NS_Cam
{
    public class CamCutscene : MonoBehaviour
    {
        public float transitionIn;
        public float transitionOut;
        public CamCutsceneShot[] shots;

        private bool isPlaying = false;

        private Vector3 anchorPos;
        private Vector3 lookAtPos;
        private float roll;
        private float fov;
        private float influence;

        public void Play()
        {
            StartCoroutine(PlayRoutine());
        }

        private IEnumerator PlayRoutine()
        {
            isPlaying = true;

            float totalTime = 0;
            float totalLength = GetLength();

            foreach (CamCutsceneShot shot in shots)
            {
                float length = shot.timeIn + shot.time + shot.timeOut;

                for (float f = 0; f < length; f += Time.deltaTime)
                {
                    float t = Mathf.Clamp01(Mathf.InverseLerp(shot.timeIn, shot.timeIn + shot.time, f));
                    SplineLerpQuery query = new SplineLerpQuery(t, shot.movementSmoothing, shot.valueSmoothing);
                    SplineLerpResult result = shot.spline.Lerp(query);

                    anchorPos = result.worldPosition;
                    lookAtPos = result.worldPosition + Quaternion.Euler(result.worldRotation) * Vector3.forward;
                    roll = result.worldRotation.z;
                    fov = result.fieldOfView;

                    float infIn = 1;
                    if (transitionIn > 0)
                        infIn = Mathf.Clamp01(totalTime / transitionIn);

                    float infOut = 1;
                    if (transitionOut > 0)
                        infOut = 1 - Mathf.Clamp01(Mathf.InverseLerp(totalLength - transitionOut, totalLength, totalTime));
                    
                    influence = Yutil.Smootherstep(Mathf.Min(infIn, infOut));

                    totalTime += Time.deltaTime;
                    yield return null;
                }
            }

            isPlaying = false;
            influence = 0;
        }

        public CamVolumeResult Direct()
        {
            // Store the cinematography in a strategy result.

            CamStrategyResult strategy = new CamStrategyResult();

            strategy.anchorPosition = anchorPos;
            strategy.offsetPosition = Vector3.zero;
            strategy.lookAtPosition = lookAtPos;
            strategy.roll = roll;
            strategy.fov = fov;

            // Wrap it in a volume result with the correct influence.

            CamVolumeResult volume = new CamVolumeResult();
            volume.influence = influence;
            volume.result = strategy;

            return volume;
        }

        public float GetLength()
        {
            float result = 0;

            for (int i = 0; i < shots.Length; i++)
            {
                result += shots[i].timeIn;
                result += shots[i].time;
                result += shots[i].timeOut;
            }

            return result;
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        void OnValidate()
        {
            transitionIn = Mathf.Max(0, transitionIn);
            transitionOut = Mathf.Max(0, transitionOut);
        }
    }

    [System.Serializable]
    public class CamCutsceneShot
    {
        public SplineWrapper spline;
        public float timeIn = 0;
        public float time = 1;
        public float timeOut = 0;
        public SplineMovementSmoothing movementSmoothing = SplineMovementSmoothing.WholeSpline;
        public bool valueSmoothing = true;
    }
}