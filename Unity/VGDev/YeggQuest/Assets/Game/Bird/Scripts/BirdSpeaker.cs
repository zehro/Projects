using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: class description (when more of what this class does is actually written)

namespace YeggQuest.NS_Bird
{
    public class BirdSpeaker : MonoBehaviour
    {
        public Bird bird;
        public AudioClip[] footstepSounds;
        public AudioClip enterPipeSound;
        public AudioClip exitPipeSound;

        private AudioSource spawnAudio;
        private AudioSource jumpAudio;
        private AudioSource jiggleAudio;
        private AudioSource frictionAudio;
        private AudioSource pipeAudio;
        private AudioSource pipeOneShotAudio;
        private AudioSource swimAudio;

        void Awake()
        {
            spawnAudio = transform.Find("SpawnAudio").GetComponent<AudioSource>();
            jumpAudio = transform.Find("JumpAudio").GetComponent<AudioSource>();
            jiggleAudio = transform.Find("JiggleAudio").GetComponent<AudioSource>();
            frictionAudio = transform.Find("FrictionAudio").GetComponent<AudioSource>();
            pipeAudio = transform.Find("PipeAudio").GetComponent<AudioSource>();
            pipeOneShotAudio = transform.Find("PipeOneShotAudio").GetComponent<AudioSource>();
            swimAudio = transform.Find("SwimAudio").GetComponent<AudioSource>();
        }

        void Update()
        {
            transform.position = bird.animator.hips.position;

            float j = bird.animator.GetJiggle();
            jiggleAudio.pitch = Mathf.Clamp(1 + j, 0.2f, 1.8f);
            jiggleAudio.volume = Mathf.Pow(Mathf.Abs(jiggleAudio.pitch - 1), 2) * 10;

            float f = bird.physics.center.velocity.magnitude;
            frictionAudio.pitch = 0.7f + f * 0.05f + j;
            frictionAudio.volume = bird.physics.GetFrictionNoise() * Mathf.InverseLerp(0.05f, 4, f);

            pipeAudio.volume = bird.animator.GetPipeAmount();
            pipeAudio.pitch = pipeAudio.volume;
        }

        // ======================================================================================================================== MESSAGES

        // Plays a random footstep sound from the given foot audio source

        internal void PlayFootstepSound(AudioSource footAudio, float strength)
        {
            footAudio.pitch = Random.Range(0.75f, 0.8f) + strength * 0.4f;
            footAudio.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)], strength);
        }

        // Plays the spawn sound

        internal void PlaySpawnSound()
        {
            spawnAudio.Play();
        }

        // Plays the jump sound

        internal void PlayJumpSound()
        {
            jumpAudio.pitch = Random.Range(0.95f, 1.05f) - bird.physics.center.velocity.magnitude * 0.04f;
            jumpAudio.Play();
        }
        
        // Plays the swim sound
       
        internal void PlaySwimSound()
        {
            swimAudio.pitch = Random.Range(0.95f, 1.05f);
            swimAudio.Play();
        }

        // Plays the entering or exiting pipe sound

        internal void PlayPipeSound(bool entering)
        {
            pipeOneShotAudio.pitch = Random.Range(0.95f, 1.05f);
            pipeOneShotAudio.PlayOneShot(entering ? enterPipeSound : exitPipeSound);
        }
    }
}