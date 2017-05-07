using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Spline;

// TODO: description

namespace YeggQuest.NS_Pipe
{
    [ExecuteInEditMode]
    public class Pipe : MonoBehaviour
    {
        [Range(0.25f, 1)]
        public float enteringTime = 0.75f;
        [Range(0.1f, 20)]
        public float traversalTime = 5;
        [Range(1, 3)]
        public float cooldownTime = 2;
        public PipeCap start;
        public PipeCap end;

        private SplineMeshWrapper wrapper;
        private Bird bird;

        void Awake()
        {
            wrapper = GetComponentInChildren<SplineMeshWrapper>();
            bird = FindObjectOfType<Bird>();
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                Transform s = wrapper.GetEntrance();
                start.transform.position = s.position;
                start.transform.rotation = s.rotation * Quaternion.Euler(0, 180, 0);
                start.transform.localScale = Vector3.one;

                Transform e = wrapper.GetExit();
                end.transform.position = e.position;
                end.transform.rotation = e.rotation;
                end.transform.localScale = Vector3.one;
            }

            else
            {
                bool s = start.HasBird(bird);
                bool e = end.HasBird(bird);

                if (s || e)
                {
                    if (s)
                    {
                        bird.EnterPipe(this, true);
                        StartCoroutine(CapRoutine(end));
                    }

                    else
                    {
                        bird.EnterPipe(this, false);
                        StartCoroutine(CapRoutine(start));
                    }
                }
            }
        }

        // ======================================================================================================================== GETTERS

        internal Vector3 GetPoint(float t)
        {
            SplineLerpQuery query = new SplineLerpQuery(t);
            return wrapper.Lerp(query).worldPosition;
        }

        internal Transform GetEntrance()
        {
            return wrapper.GetEntrance();
        }

        internal Transform GetExit()
        {
            return wrapper.GetExit();
        }

        internal Vector3 GetEntranceVelocity()
        {
            return start.GetVelocity();
        }

        internal Vector3 GetExitVelocity()
        {
            return end.GetVelocity();
        }

        internal float GetLength()
        {
            return wrapper.GetLength();
        }

        // ======================================================================================================================== HELPERS

        // Private coroutine that handles how the pipe caps behave when the bird enters the pipe.
        // The cap passed to this routine (the one the bird is going to exit out of) is forced open
        // for a brief window of buffer time right before and after the bird exits, and after that, both
        // caps are disabled for the cooldown time.

        private IEnumerator CapRoutine(PipeCap cap)
        {
            float bufferTime = 0.5f;
            yield return new WaitForSeconds(enteringTime + traversalTime - bufferTime);

            cap.ForceOpen(bufferTime * 2);
            start.DisableSuction(cooldownTime);
            end.DisableSuction(cooldownTime);
        }
    }
}