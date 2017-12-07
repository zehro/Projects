using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Logic;

// Every Pipe has two PipeCaps, at the entrance and the exit respectively. They are closely coupled: the PipeCaps are reponsible
// for reporting to the owning Pipe when the bird is within their range, as well as playing their own animations and audio. The
// owning Pipe also has controls over the caps, telling them to disable their suction (when the pipe is in its cooldown period),
// as well as force open (when the bird needs to pass through an end that is currently closed.) PipeCaps also have logical input,
// so they can be opened and closed dynamically as a part of gameplay.

namespace YeggQuest.NS_Pipe
{
    [ExecuteInEditMode]
    public class PipeCap : MonoBehaviour
    {
        public Vector3 triggerSize = new Vector3(1.25f, 1.25f, 2f);
        public bool openByDefault = true;
        public Logical input;

        [Space(10)]
        [Header("Internal References")]
        public Transform cover;                                             // The actual cover GameObject that visually and physically prevents the bird from entering
        public AudioSource coverAudio;                                      // The audio source for the cover sounds (opening and closing)
        public AudioClip[] coverSounds;                                     
        public ParticleSystem suctionParticles;                             // The particle
        public AudioSource suctionAudio;                                    // The audio the suction makes
        public AudioSource hingeAudio;                                      // The audio the pipe cover hinge makes

        private Vector3 pos;
        private Vector3 posPrev;
        private Vector3 velocity;
        
        private bool open;                                                  // Whether or not this cap is open (directly tied to logical input)
        private bool openPrev;                                              // Whether or not this cap was open in the last frame (used for playing sounds)
        private bool openForced;                                            // Whether or not this cap is being forced open (used by the owning Pipe when the bird exits through a closed end)
        private float angle = 0;
        private float angleVel = 0;
        private float angleAccel = 0.05f;
        private float angleDrag = 0.15f;
        private float angleMax = -120f;

        private bool suction;                                               // Whether or not this cap has suction to accept the bird (disabled by the owning Pipe during its cooldown period)
        private float suctionPitch = 1;                                     // A randomized suction pitch for each pipe
        private float jiggle = 0;
        private float jiggleDrag = 0.03f;
        private float jiggleMax = 0.02f;

        void Awake()
        {
            pos = transform.position;
            posPrev = pos;

            // Initialize control relating to pipe openness

            open = Logic.SafeEvaluate(input, openByDefault) || openForced;
            openPrev = open;
            openForced = false;
            angle = open ? angleMax : 0;

            // Initialize control relating to pipe suction

            suction = true;
            if (Application.isPlaying)
                suctionPitch = Random.Range(0.9f, 1.1f);

            if (open && suction)
            {
                jiggle = jiggleMax;

                suctionParticles.Play();
                suctionAudio.pitch = suctionPitch;
                suctionAudio.volume = 1;
            }
        }

        void Update()
        {
            // Should the cover be open or closed?
            // This is directly tied to the logical input, but can also be forced by the owning Pipe.

            openPrev = open;
            open = Logic.SafeEvaluate(input, openByDefault) || openForced;

            // When in editor mode, the pipe cover immediately opens and closes to show the
            // default state the pipe will be in when the game begins. The variables tied
            // to suction are not calculated.

            if (!Application.isPlaying)
            {
                angle = open ? angleMax : 0;
                jiggle = 0;
            }

            // When in play mode, the pipe cover swivels in and out with the open state,
            // and the pipe cover's scale, audio, and particles are tied to the suction state.

            else
            {
                // Control relating to open state

                float dt = Time.deltaTime * 60;
                float angleTarg = open ? angleMax : 0;
                angleVel += (angleTarg - angle) * angleAccel * dt;
                angleVel *= 1 - (angleDrag * dt);
                angle += angleVel * dt;

                if (angle > 0)
                {
                    angle = 0;
                    angleVel *= -0.5f;
                }

                hingeAudio.pitch = (angleVel > 0 ? 0.6f : 0.8f);
                hingeAudio.volume = Mathf.Abs(angleVel) * 0.1f;

                // Control relating to suction state

                float jiggleTarg = open && suction ? jiggleMax : 0;
                jiggle = Mathf.Lerp(jiggle, jiggleTarg, jiggleDrag * dt);

                if (open && suction)
                {
                    if (!suctionParticles.isPlaying)
                        suctionParticles.Play();
                }

                else
                    suctionParticles.Stop();

                suctionAudio.pitch = jiggle / jiggleMax * suctionPitch;
                suctionAudio.volume = jiggle / jiggleMax * 0.5f + 0.5f;

                // Play the sounds for the cover opening and closing

                if (!openPrev && open)
                {
                    coverAudio.pitch = Random.Range(0.95f, 1.05f);
                    coverAudio.PlayOneShot(coverSounds[0]);
                }

                if (openPrev && !open)
                {
                    coverAudio.pitch = Random.Range(0.95f, 1.05f);
                    coverAudio.PlayOneShot(coverSounds[1]);
                }
            }

            // Animate the pipe cap and the cover in the game and the editor,
            // using the openness and suction control variables.

            float t = Mathf.Sin(Time.time * 50) * jiggle;
            transform.localScale = new Vector3(1 + t, 1 + t, 1);
            cover.transform.localRotation = Quaternion.Euler(angle + t * angle * -0.5f, 0, 0);

            // Calculate the velocity of the end of the pipe, for launching the bird at
            // the proper relative speed if this pipe is moving

            if (Time.deltaTime == 0 || !Application.isPlaying)
                return;

            posPrev = pos;
            pos = transform.position;
            velocity = (pos - posPrev) / Time.deltaTime;
        }

        void OnDrawGizmos()
        {
            Logic.Visualize(transform, input);
            Gizmos.color = open ? Color.yellow : Color.black;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.forward * triggerSize.z * 0.5f, triggerSize);
        }

        // ======================================================================================================================== GETTERS

        // Checks whether or not this pipe cap has the bird within its range.
        // This will always return false if this end of the pipe is closed or is not sucking.

        internal bool HasBird(Bird bird)
        {
            if (!open || !suction)
                return false;

            Vector3 world = bird.GetPosition();
            Vector3 local = transform.InverseTransformPoint(world);

            return (Mathf.Abs(local.x) < triggerSize.x
                 && Mathf.Abs(local.y) < triggerSize.y
                 && local.z > 0 && local.z < triggerSize.z);
        }

        // Gets the velocity at the end of this pipe

        internal Vector3 GetVelocity()
        {
            return velocity;
        }

        // ======================================================================================================================== MESSAGES
        
        // A message called by the owning Pipe when the bird enters it, which disables the suction of the pipe.

        internal void DisableSuction(float seconds)
        {
            StartCoroutine(DisableSuctionRoutine(seconds));
        }

        // A message called by the owning Pipe when the bird needs to exit through this end, forcing it open.

        internal void ForceOpen(float seconds)
        {
            StartCoroutine(ForceOpenRoutine(seconds));
        }
        
        // ======================================================================================================================== HELPERS

        // Private coroutine which disables the pipe's suction for the given amount of time, then turns it back on.

        private IEnumerator DisableSuctionRoutine(float seconds)
        {
            suction = false;
            yield return new WaitForSeconds(seconds);
            suction = true;
        }

        // Private coroutine which forces this pipe cap open for the given amount of time, then (possibly) closes it again.

        private IEnumerator ForceOpenRoutine(float seconds)
        {
            openForced = true;
            yield return new WaitForSeconds(seconds);
            openForced = false;
        }
    }
}