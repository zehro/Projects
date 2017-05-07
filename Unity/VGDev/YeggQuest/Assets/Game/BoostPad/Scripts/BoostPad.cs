using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Logic;

namespace YeggQuest.NS_BoostPad
{
    [ExecuteInEditMode]
    public class BoostPad : CollisionReceiver
    {
        [Space(10)]
        [Header("Boosting Behavior")]
        [Range(1, 30)]
        public float boostTopSpeed = 10;
        [Range(0.1f, 5)]
        public float boostAcceleration = 1;

        [Space(10)]
        [Header("Input Behavior")]
        public Logical input;
        public bool onByDefault = true;

        [Space(10)]
        [Header("Internal References")]
        public BoxCollider trigger;
        public MeshRenderer screen;
        public AudioSource sound;

        private float triggerBorder = 0.25f;
        private float soundPitch = 1;

        private bool on;
        //private bool onPrev;

        private float strength;
        private float strengthNormal = 1.5f;
        private float strengthActive = 3.5f;
        private float strengthDrag = 0.1f;
        private float animTime;

        void Start()
        {
            if (Application.isPlaying)
            {
                soundPitch = Random.Range(1.03f, 1.05f);

                on = Logic.SafeEvaluate(input, onByDefault);
                //onPrev = on;

                strength = on ? strengthNormal : 0;

                screen.material.SetFloat("_Strength", strength);
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                // Figure out whether the pad should be on or off

                //onPrev = on;
                on = Logic.SafeEvaluate(input, onByDefault);

                // Play the animations based on that state

                float dt = Time.deltaTime * 60;
                float strengthTarg = on ? strengthNormal : 0;
                strength = Mathf.Lerp(strength, strengthTarg, strengthDrag * dt);
                animTime += Time.deltaTime * strength * boostTopSpeed / 6f;

                //float wobble = 1 + Mathf.Sin(animTime * 6) * 0.2f;

                screen.material.SetFloat("_Strength", strength);
                screen.material.SetFloat("_AnimTime", animTime);

                // Play sounds

                sound.pitch = strength * soundPitch;
                sound.volume = (strength / strengthActive) * (strength / strengthActive);

                // Make the trigger the correct size

                if (transform.localScale.x != 0
                 && transform.localScale.z != 0)
                {
                    Vector3 size = trigger.size;
                    size.x = 1 - triggerBorder / transform.localScale.x;
                    size.z = 1 - triggerBorder / transform.localScale.z;
                    trigger.size = size;
                }
            }

            // In the editor, just enforce the default Y scale
            // (scaling a BoostPad along Y doesn't make sense)

            else
            {
                Vector3 scale = transform.localScale;
                scale.y = 1;
                transform.localScale = scale;
            }
        }
        
        public override void OnReceivedTriggerStay(Collider other)
        {
            if (on)
            {
                strength = strengthActive;

                Bird bird = other.GetComponentInParent<Bird>();

                if (bird)
                    BoostBird(bird);
                else if (other.attachedRigidbody)
                    BoostRigidbody(other.attachedRigidbody);
            }
        }

        void OnDrawGizmos()
        {
            Logic.Visualize(transform, input);
        }

        // ======================================================================================================================== HELPERS

        private void BoostBird(Bird bird)
        {
            foreach (Rigidbody body in bird.GetComponentsInChildren<Rigidbody>())
                BoostRigidbody(body);
        }

        private void BoostRigidbody(Rigidbody body)
        {
            Vector3 lateralSpeed = Vector3.ProjectOnPlane(body.velocity, BoostNormal());

            Vector3 currForwardSpeed = Vector3.Project(body.velocity, BoostNormal());
            Vector3 fullForwardSpeed = boostTopSpeed * BoostNormal();
            Vector3 newForwardSpeed = Vector3.MoveTowards(currForwardSpeed, fullForwardSpeed, boostAcceleration);

            body.velocity = lateralSpeed + newForwardSpeed;
        }

        private Vector3 BoostNormal()
        {
            return transform.forward;
        }
    }
}