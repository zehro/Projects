using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The BirdAnimatorHead (unsurprisingly) controls the bird's head. While it carries out the
// exact functionality of a BirdAnimatorNode, it also has a more special task: being able to
// "look" both the head and the eyes at a controlled target position.

namespace YeggQuest.NS_Bird
{
    public class BirdAnimatorHead : MonoBehaviour
    {
        public BirdAnimator animator;               // the bird animator
        public Rigidbody physicsNode;               // the rigidbody physics node that this node is paired to
        public Transform lookAt;                    // the transform for what to look at (controlled in here)
        public Transform eyeL;                      // the left eye
        public Transform eyeR;                      // the right eye

        private Vector3 restingPos;                 // the default local position the head should return to when the BirdAnimator is walking
        private BirdLookTarget curLookTarget;       // the current BirdLookTarget the head should be looking at

        private float[] lookRot = new float[2];
        private float headPitch = 0;
        private float headYaw = 0;
        private float headDrag = 0.2f;
        private float eyeLPitch = 0;
        private float eyeLYaw = 0;
        private float eyeRPitch = 0;
        private float eyeRYaw = 0;
        private float eyeDrag = 0.8f;

        private const float MAX_EYE_MULT = 1.8f;    // Cap on the multiplier for eye movement. Prevents low frame rate from causing overflow errors.

        void Start()
        {
            restingPos = transform.localPosition;
        }

        void LateUpdate()
        {
            // The head acts mostly like a BirdAnimatorNode, but with a few extra stipulations:
            // it pulls in and squishes into the body when the BirdAnimator is balled up.

            float t = Mathf.SmoothStep(0, 1, animator.GetBallAmount());
            Vector3 ballOffset = physicsNode.transform.up * t * 0.06f;
            float ballScale = t * 0.4f;

            transform.position = Yutil.ApplyScale(animator.transform, physicsNode.transform.position + ballOffset);
            transform.localPosition = Vector3.Lerp(transform.localPosition, restingPos, 1 - t);
            transform.localScale = new Vector3(1 + ballScale * 0.5f, 1, 1 - ballScale);

            // Control headlook. First, we have to find out if there's any BirdLookTargets we should
            // be looking at in the scene. Get the closest activated one within range:

            curLookTarget = null;
            float closest = Mathf.Infinity;

            foreach (BirdLookTarget target in FindObjectsOfType<BirdLookTarget>())
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (target.enabled && dist < target.radius && dist < closest)
                {
                    closest = dist;
                    curLookTarget = target;
                }
            }

            // If there is a target to look at, look at it:

            if (curLookTarget != null)
                lookAt.position = curLookTarget.transform.position;

            // Otherwise, make the bird look with turning and normal movement:

            else
            {
                float angle = animator.bird.navigator.GetAngularSpeed();
                lookAt.localPosition = new Vector3(angle * 0.005f, -1 + Mathf.Abs(angle * 0.005f), 0.225f);
            }

            // Now do the headlook programmatically TODO: clean this it's still poopy

            Vector3 lookVec;
            float dt = Time.smoothDeltaTime * 60;

            // Head

            lookVec = lookAt.position - transform.position;
            Yutil.CalculatePitchYaw(animator.hips, Quaternion.Euler(90, 0, 0), lookVec, lookRot);
            headPitch += (Mathf.Clamp(lookRot[0], -30, 15) - headPitch) * headDrag * dt;
            headYaw += (Mathf.Clamp(lookRot[1], -45, 45) - headYaw) * headDrag * dt;
            transform.localRotation = Quaternion.Euler(headPitch * (1 - t) + t * -10f, animator.bird.navigator.GetAngularSpeed() * 0.05f, headYaw * (1 - t));

            // EyeL

            float eyeMult = Mathf.Min(eyeDrag * dt, MAX_EYE_MULT);

            lookVec = lookAt.position + Vector3.up * 0.2f - eyeL.position;
            Yutil.CalculatePitchYaw(eyeL.parent, Quaternion.identity, lookVec, lookRot);
            eyeLPitch += (Mathf.Clamp(lookRot[0], -25, 15) - eyeLPitch) * eyeMult;
            eyeLYaw += (Mathf.Clamp(lookRot[1], -45, 25) - eyeLYaw) * eyeMult;
            eyeL.localRotation = Quaternion.Euler(eyeLPitch, eyeLYaw, 0);

            // EyeR

            lookVec = lookAt.position + Vector3.up * 0.2f - eyeR.position;
            Yutil.CalculatePitchYaw(eyeR.parent, Quaternion.identity, lookVec, lookRot);
            eyeRPitch += (Mathf.Clamp(lookRot[0], -25, 15) - eyeRPitch) * eyeMult;
            eyeRYaw += (Mathf.Clamp(lookRot[1], -25, 45) - eyeRYaw) * eyeMult;
            eyeR.localRotation = Quaternion.Euler(eyeRPitch, eyeRYaw, 0);
        }
    }
}