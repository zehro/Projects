using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Cam;

namespace YeggQuest.NS_Spawner
{
    public class SpawnerCheckpoint : CollisionReceiver
    {
        public Spawner spawner;                 // the spawner this checkpoint contains
        public CamVolume volume;                // the "opening shot" for this checkpoint
        public ParticleSystem burst;            // the particle burst
        public ParticleSystem flagpoleBurst;    // the particle burst for the flagpole
        public Transform flagpole;              // the flagpole
        public Transform flag;                  // the flag
        public Transform[] wedges;              // the wedges of the aperture
        public Light glow;                      // the glow from inside the aperture
        public AudioSource spawnAudio;          // the audio for spawning
        public AudioSource activeAudio;         // the audio for checkpoint activation
        public AudioSource apertureAudio;       // the audio for aperture behavior
        public AudioClip apertureOpen;          // the opening sound
        public AudioClip apertureClose;         // the closing sound

        private GameCoordinator coordinator;    // the game coordinator

        private bool isJiggling;
        private float jiggle = 0;
        private float jiggleVel = 0;
        private float jiggleAccel = 0.1f;
        private float jiggleDrag = 0.1f;

        private bool isUp;
        private float up = 0;
        private float upVel = 0;
        private float upAccel = 0.05f;
        private float upDrag = 0.05f;

        private bool isOpen;
        private float open = 0;
        private float openVel = 0;
        private float openAccel = 0.2f;
        private float openDrag = 0.15f;

        private float startTimer = 0;
        private float startTimerReset = 0.5f;

        void Awake()
        {
            coordinator = FindObjectOfType<GameCoordinator>();
        }

        void Update()
        {
            // Update the jiggliness of the entire checkpoint

            float dt = Time.deltaTime * 60;
            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;

            transform.localScale = new Vector3(1 - jiggle, 1 - jiggle, 1 + jiggle * 4);

            // Update the flag being up / down

            bool newUp = coordinator.GetActiveSpawner() == spawner;

            if (!isUp && newUp)
            {
                activeAudio.Play();
                flagpoleBurst.Play();
            }

            isUp = newUp;

            float upTarg = isUp ? 1 : 0;
            upVel += (upTarg - up) * upAccel * dt;
            upVel *= 1 - (upDrag * dt);
            up += upVel * dt;

            if (up < 0 || up > 1)
            {
                up = Mathf.Clamp01(up);
                jiggleVel += upVel * 0.1f;
                upVel *= -0.25f;
            }

            float flap = 1 + Mathf.Sin(Time.time * 35) * 0.01f;

            flagpole.localPosition = new Vector3(0, 0, up * 2);
            flag.localScale = new Vector3(1, up * flap, 1);

            // Update the aperture (the wedges and the glow)

            float openTarg = isOpen ? 1 : 0;
            openVel += (openTarg - open) * openAccel * dt;
            openVel *= 1 - (openDrag * dt);
            open += openVel * dt;
            
            if (open < 0)
            {
                open = 0;
                openVel *= -0.5f;
            }

            if (open > 1)
            {
                open = 1;
                openVel = 0;
            }
            
            foreach (Transform w in wedges)
                w.localRotation = Quaternion.Euler(0, 0, open * 55);
            glow.intensity = open * 8;

            // Update the camera volume
            
            startTimer = Mathf.Max(0, startTimer - Time.deltaTime);

            if (startTimer > 0)
                volume.SetInfluence(1);
        }

        // When the bird touches the checkpoint trigger, this spawner becomes the
        // active spawner in the scene.

        public override void OnReceivedTriggerEnter(Collider other)
        {
            Bird bird = other.GetComponentInParent<Bird>();
            if (bird)
                coordinator.SetActiveSpawner(spawner);
        }

        // Called by the spawner when the bird shoots out of it.

        public void Animate()
        {
            burst.Play();
            spawnAudio.Play();
            jiggleVel = 0.05f;
            StartCoroutine(Aperture());
            startTimer = startTimerReset;
        }

        private IEnumerator Aperture()
        {
            isOpen = true;
            apertureAudio.PlayOneShot(apertureOpen);
            yield return new WaitForSeconds(0.75f);
            isOpen = false;
            apertureAudio.PlayOneShot(apertureClose);
        }
    }
}