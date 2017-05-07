using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The two BirdAnimatorLegs are objects which each handle the placement and replanting of one leg and foot.
// Through the BirdAnimator, they receive positions (SetPlantTarget) and orientations (SetPlantHeading)
// to plant their feet and internally carry out replanting and IK routines to make their legs bend
// properly. They're tied directly to the BirdAnimator because that's the only information needed.

namespace YeggQuest.NS_Bird
{
    public class BirdAnimatorLeg : MonoBehaviour
    {
        public BirdAnimator animator;       // the bird animator
        public Transform thigh;             // the thigh
        public Transform calf;              // the calf
        public Transform foot;              // the foot
        public LayerMask plantMask;         // what the foot can plant on

        private Vector3 footPos;            // where the foot currently is in world space
        private Quaternion footRot;         // the rotation representing the normal the foot is on
        private AudioSource footAudio;      // the audio source connected to the foot (for playing footstep sounds)
        private float footHeading;          // the direction the foot is facing in world space

        private Vector3 plantTarg;          // where the foot should attempt to plant next (continually updated, even while planting)
        private Vector3 plantPrev;          // the last place the foot finished planting (set once when planting begins, for interpolation)
        private Quaternion plantRotTarg;    // the normal rotation of the surface the foot is attempting to plant on next
        private Quaternion plantRotPrev;    // the normal rotation of the last place the foot finished planting
        private float plantProgress = 1;    // the progress of the replanting maneuver (where 1 == done and ready to start again)
        private bool plantOverride = false; // whether or not the replanting maneuver should immediately skip to the end

        private float ikLength;             // how long the leg is at its full physical extension
        private float ikSoftDist;           // the distance before the full extension at which the IK solver starts to become stretchy

        private Vector3 retractedPos;       // where the foot is (in local space) when the bird animator is in ball form
        private Quaternion retractedRot;    // the rotation the foot has (in local space) when the bird animator is in ball form

        void Start()
        {
            footPos = foot.position;
            footRot = Quaternion.identity;
            footAudio = foot.GetComponent<AudioSource>();

            plantTarg = footPos;
            plantPrev = footPos;
            plantRotTarg = Quaternion.identity;
            plantRotPrev = Quaternion.identity;

            ikLength = new Vector2(0.1f, 0.4f).magnitude;
            ikSoftDist = ikLength * 0.05f;

            retractedPos = new Vector3(foot.localPosition.x < 0 ? -0.19f : 0.19f, 0, -0.2f);
            retractedRot = Quaternion.Euler(0, foot.localPosition.x < 0 ? 30 : -30, 0);
        }

        internal void CustomUpdate()
        {
            // Every frame, we move the foot to the foot position (which could be being updated by
            // the replanting coroutine below if it's running), and also apply the scale of the
            // bird animator (so the feet move their position properly when the bird jiggles)

            foot.position = Yutil.ApplyScale(animator.transform, footPos);

            // The foot rotation is based on the slope of the surface (footRot), the direction
            // the foot should be facing (footHeading) and the pitch based on how far the foot
            // is through the replanting animation (footPitch)

            float footPitch = plantProgress * plantProgress;
            footPitch = Mathf.Sin(footPitch * 2 * Mathf.PI) * GetPlantDistance() * 100;
            foot.rotation = footRot * Quaternion.Euler(-90 + footPitch, footHeading, 0);

            // After these values are set, the foot lerps into its retracted local position and
            // rotation based on how much the bird animator is balled up for rolling

            float t = Mathf.SmoothStep(0, 1, animator.GetBallAmount());
            foot.localPosition = Vector3.Lerp(foot.localPosition, retractedPos, t);
            foot.localRotation = Quaternion.Slerp(foot.localRotation, retractedRot, t);

            // Now that the foot is in the right place, run the leg IK solving routine, which
            // rotates and stretches the thigh and calf so that they point nicely at the foot

            SolveIK();
        }

        // ======================================================================================================================== SETTERS

        // Sets what world-space direction the foot should face

        internal void SetFootHeading(float heading)
        {
            footHeading = heading;
        }

        // Sets where the foot should plant
        // This function works by casting rays downwards from 25 points offset in a grid
        // from the given target point. The actual foot target is then set to the surface
        // point which causes the least error in comparison to the original target, which
        // (somewhat) circumvents the popping artifacts that occur when the wanted target
        // suddenly goes over a height discontinuity. If literally no surfaces are found,
        // the raw target point is used as a fallback.

        internal void SetPlantTarget(Vector3 target)
        {
            if (plantProgress != 1 && plantProgress > 0.8f)
                return;

            plantTarg = target;
            plantRotTarg = Quaternion.identity;

            float rayBest = Mathf.Infinity;
            RaycastHit rayHit;

            for (float x = -0.2f; x <= 0.2f; x += 0.1f)
            {
                for (float y = -0.2f; y <= 0.2f; y += 0.1f)
                {
                    Vector3 offset = new Vector3(x, 0.5f, y);

                    if (Physics.Raycast(target + offset, Vector3.down, out rayHit, 1f, plantMask.value))
                    {
                        float diff = Vector3.Distance(target, rayHit.point);
                        if (diff < rayBest)
                        {
                            rayBest = diff;
                            plantTarg = rayHit.point;
                            plantRotTarg = Quaternion.FromToRotation(Vector3.up, rayHit.normal);
                        }
                    }
                }
            }
        }

        // ======================================================================================================================== MESSAGES

        // Begins the foot replanting maneuver as long as the leg is ready to
        // The given time parameter is how long the maneuver will take to complete

        internal void Plant(float plantTime)
        {
            if (plantProgress == 1)
                StartCoroutine("ReplantRoutine", plantTime);
        }

        // Forces the foot to the target position and rotation immediately
        // This is used when the bird animator does the ToWalking animation

        internal void PlantImmediately()
        {
            plantOverride = true;
        }

        // ======================================================================================================================== GETTERS

        // Gets the plant progress

        internal float GetPlantProgress()
        {
            return plantProgress;
        }

        // Gets the plant distance (how far the foot is from its target currently)

        internal float GetPlantDistance()
        {
            return Vector3.Distance(footPos, plantTarg);
        }

        // Gets the foot position

        internal Vector3 GetFootPosition()
        {
            return footPos;
        }

        // Gets the foot rotation (just the slope influence)

        internal Quaternion GetFootRotation()
        {
            return footRot;
        }

        // Gets the foot audio source

        internal AudioSource GetFootAudio()
        {
            return footAudio;
        }

        // ======================================================================================================================== HELPERS

        // A private coroutine which actually carries out the replanting maneuver over the given time (moves the foot)
        // The height peak of the foot's arc depends on how far the foot was asked to move when the coroutine began
        // After the coroutine finishes, it sends the bird animator the LegPlanted() message with some information

        private IEnumerator ReplantRoutine(float plantTime)
        {
            plantTime = Mathf.Clamp(plantTime, 0.01f, 1f);
            float heightPeak = GetPlantDistance();

            plantPrev = footPos;
            plantRotPrev = footRot;

            for (float f = 0; f < plantTime; f += Time.deltaTime)
            {
                plantProgress = f / plantTime;

                footPos = Vector3.Lerp(plantPrev, plantTarg, plantProgress);
                footPos.y += plantProgress * (1 - plantProgress) * heightPeak;
                footRot = Quaternion.Slerp(plantRotPrev, plantRotTarg, plantProgress);

                if (plantOverride)
                    break;
                else
                    yield return null;
            }

            plantOverride = false;
            plantProgress = 1;
            footPos = plantTarg;
            footRot = plantRotTarg;

            animator.LegPlanted(footAudio, heightPeak);
        }

        // A private helper function which solves IK for the leg

        private void SolveIK()
        {
            // First, what actual length does the leg need to cover? (from hip to ankle)
            // Based on this, calculate a "soft length", which matches the actual length
            // until a soft distance (ikSoftDist) before the leg's full length (ikLength),
            // at which point it interpolates asymptotically to the full length

            float actualLength = Vector3.Distance(thigh.localPosition, foot.localPosition);
            float softLength = actualLength;

            float softCutoff = ikLength - ikSoftDist;
            if (actualLength > softCutoff)
                softLength = softCutoff + ikSoftDist * (1 - Mathf.Exp((softCutoff - softLength) / ikSoftDist));

            // Now, rotate the thigh and calf so they extend to the soft length in the proper direction.
            // This is done by first pointing the thigh and calf at the foot, then bending at the "knee"
            // so that the ankle (end of calf) is the soft length away from the hip (start of thigh)
            // This bend gets negated when the bird animator is in ball form

            thigh.LookAt(foot, thigh.parent.up);
            thigh.Rotate(0, 180, 0);
            calf.rotation = thigh.rotation;

            float bend = Mathf.Acos(softLength / ikLength) * Mathf.Rad2Deg;
            bend *= Mathf.SmoothStep(0, 1, 1 - animator.GetBallAmount());
            thigh.Rotate(-bend, 0, 0);
            calf.Rotate(bend * 2, 0, 0);

            // Finally, stretch the thigh and the calf so that the ankle actually lines up with the foot

            float limbLength = (actualLength * 0.5f) / Mathf.Cos(bend * Mathf.Deg2Rad);
            limbLength -= animator.GetBallAmount() * 0.05f;
            calf.localPosition = Vector3.back * limbLength;
        }
    }
}