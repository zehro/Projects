using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Logic;

namespace YeggQuest.NS_Spring
{
    public class Spring : CollisionReceiver
    {
        [Space(10)]
        [Header("Spring Behavior")]
        [Range(1, 50)]
        public float bounceHeight = 5;
        public bool removeVelocity = false;

        [Space(10)]
        [Header("Input Behavior")]
        public bool openByDefault = true;
        public Logical input;

        [Space(10)]
        [Header("Internal References")]
        public Transform spring;
        public Transform bumper;
        public AudioSource bumperAudio;
        public Transform cap1;
        public Transform cap2;
        public MeshCollider capCollision;
        public AudioSource capAudio;
        public AudioClip[] capSounds;
        public ParticleSystem particles;

        private Vector3 initialScale;

        private bool open;
        private bool openPrev;

        private float timer;
        private float timerCooldown = 0.5f;

        private float jiggle;
        private float jiggleVel;
        private float jiggleAccel = 0.4f;
        private float jiggleDrag = 0.2f;
        private float jiggleBound = 0.9f;

        private float extend;
        private float extendMin = 0.01f;
        private float extendMax;
        private float extendVel;
        private float extendAccel;
        private float extendDrag = 0.1f;
        
        private float angle = 0;
        private float angleVel = 0;
        private float angleAccel = 0.05f;
        private float angleDrag = 0.15f;
        private float angleMax = 55f;

        void Start()
        {
            initialScale = transform.localScale;

            open = Logic.SafeEvaluate(input, openByDefault);
            openPrev = open;

            extend = extendMin;
            extendMax = Mathf.Min(bounceHeight * 0.25f, 3f);

            angle = open ? angleMax : 0;
        }

        void Update()
        {
            // Figure out whether the spring should be open or closed, playing sounds accordingly
            // and toggling the collider that represents the closed cap

            openPrev = open;
            open = Logic.SafeEvaluate(input, openByDefault);

            if (!openPrev && open)
            {
                capAudio.pitch = Random.Range(0.95f, 1.05f);
                capAudio.PlayOneShot(capSounds[0]);
            }

            if (openPrev && !open)
            {
                capAudio.pitch = Random.Range(0.95f, 1.05f);
                capAudio.PlayOneShot(capSounds[1]);
            }

            capCollision.enabled = !open;

            // Tick down the bounce cooldown timer

            timer = Mathf.Max(timer - Time.deltaTime, 0);

            // Animate the jiggle, extension, and angle variables

            float dt = Time.deltaTime * 60;
            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;
            jiggle = Mathf.Clamp(jiggle, -jiggleBound, jiggleBound);
            
            float t = 1 - Mathf.InverseLerp(timerCooldown - 0.3f, timerCooldown, timer);
            extendAccel = Mathf.Lerp(0, 0.15f, t);
            float extendTarg = (timer > 0 && open ? extendMax : extendMin);
            extendVel += (extendTarg - extend) * extendAccel * dt;
            extendVel *= 1 - (extendDrag * dt);
            extend += extendVel * dt;

            if (extend < extendMin)
            {
                extend = extendMin;
                extendVel = 0;
                jiggleVel = -0.05f;
                particles.Play();
            }

            float angleTarg = open ? angleMax : 0;
            angleVel += (angleTarg - angle) * angleAccel * dt;
            angleVel *= 1 - (angleDrag * dt);
            angle += angleVel * dt;

            if (angle < 0)
            {
                angle = 0;
                angleVel *= -0.5f;
            }

            if (angle > angleMax)
            {
                angle = angleMax;
                angleVel = 0;
            }

            // Use them to animate the transforms

            transform.localScale = Vector3.Scale(initialScale, new Vector3(1 - jiggle, 1 - jiggle, 1 + jiggle));
            spring.localScale = new Vector3(1, 1, extend);
            bumper.localPosition = new Vector3(0, 0, 0.02f + extend);
            cap1.localRotation = Quaternion.Euler(0, -angle, 0);
            cap2.localRotation = Quaternion.Euler(0, +angle, 0);
        }

        // When the bumper sends a message to this object saying that it found something to bounce,
        // the spring checks to make sure that its cooldown period is over and it's open before bouncing.
        // This applies forces and plays sounds.

        public override void OnReceivedTriggerEnter(Collider other)
        {
            if (timer == 0 && open)
            {
                jiggleVel = -0.05f;

                float r = Random.Range(0.9f, 1.1f);
                timer = timerCooldown / r;
                bumperAudio.pitch = r;
                bumperAudio.Play();

                Bird bird = other.GetComponentInParent<Bird>();

                if (bird)
                    BounceBird(bird);
                else
                    BounceRigidbody(other.attachedRigidbody);
            }
        }

        void OnDrawGizmos()
        {
            Yutil.DrawArrow(transform.position, transform.position + BounceNormal() * bounceHeight, Color.white);
            Logic.Visualize(transform, input);
        }
        
        // ======================================================================================================================== HELPERS

        private void BounceBird(Bird bird)
        {
            foreach (Rigidbody body in bird.GetComponentsInChildren<Rigidbody>())
                BounceRigidbody(body);
        }

        private void BounceRigidbody(Rigidbody body)
        {
            Vector3 baseVelocity = Vector3.zero;

            if (!removeVelocity)
                baseVelocity = Vector3.ProjectOnPlane(body.velocity, BounceNormal());

            body.velocity = baseVelocity + BounceNormal() * BounceForce();
        }

        private float BounceForce()
        {
            return Mathf.Sqrt(2 * Physics.gravity.magnitude * bounceHeight);
        }

        private Vector3 BounceNormal()
        {
            return transform.forward;
        }
    }
}