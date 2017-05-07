using UnityEngine;
using YeggQuest.NS_Bird;
using YeggQuest.NS_Paint;

namespace YeggQuest.NS_Water
{

    /// <summary>
    /// Makes the bird move slower when inside of it.
    /// </summary>
    public class Water : MonoBehaviour
    {

        /// <summary> Splash particles when the bird enters the water. </summary>
        private ParticleSystem particles;
        /// <summary> Settings used to configure particle emission. </summary>
        private ParticleSystem.MainModule particleSettings;
        /// <summary> The audio source to play splash sounds with. </summary>
        private AudioSource audioSource;

        [Header("Effects")]
        /// <summary> The number of particles to emit when splashing. </summary>
        [SerializeField]
        [Tooltip("The number of particles to emit when splashing.")]
        private int numParticles;
        /// <summary> The water-entering speed that will cause the maximum splash volume. </summary>
        [SerializeField]
        [Tooltip("The water-entering speed that will cause the maximum splash volume.")]
        private float maxSplashSpeed;
        /// <summary> The sound to play when entering water. </summary>
        [SerializeField]
        [Tooltip("The sound to play when entering water.")]
        private AudioClip splashSound;
        /// <summary> The sound to play when leaving water. </summary>
        [SerializeField]
        [Tooltip("The sound to play when leaving water.")]
        private AudioClip leaveSound;

        /// <summary>
        /// Finds the splash particles to emit.
        /// </summary>
        private void Start()
        {
            particles = GetComponentInChildren<ParticleSystem>();
            particleSettings = particles.main;
            audioSource = GetComponentInChildren<AudioSource>();
        }

        /// <summary>
        /// Makes splash efffects when the bird enters the water.
        /// </summary>
        /// <param name="collider">The collider of the bird that hit the water.</param>
        private void OnTriggerEnter(Collider collider)
        {
            InteractWater(collider, -1, splashSound);
        }

        /// <summary>
        /// Marks an object as in water.
        /// </summary>
        /// <param name="collider">The collider of the object that is in the water.</param>
        private void OnTriggerStay(Collider collider)
        {
            SetInWater(collider, true);
            Paintable paintable = collider.GetComponent<Paintable>();
            if (paintable != null) {
                paintable.Paint(new PaintRequest(PaintColor.Clear));
            }
        }

        /// <summary>
        /// Marks when an object leaves the water.
        /// </summary>
        /// <param name="collider">The collider of the object that left the water.</param>
        private void OnTriggerExit(Collider collider)
        {
            SetInWater(collider, false);
            InteractWater(collider, 1, leaveSound);
        }

        /// <summary>
        /// Plays effects when the water is interacted with.
        /// </summary>
        /// <param name="collider">Collider.</param>
        /// <param name="particleSpeedMultiplier">Particle speed multiplier.</param>
        /// <param name="sound">Sound.</param>
        private void InteractWater(Collider collider, float particleSpeedMultiplier, AudioClip sound)
        {
            particles.transform.position = collider.transform.position;
            Rigidbody body = collider.attachedRigidbody;

            if (body != null)
            {
                Vector3 particleVector = body.velocity * particleSpeedMultiplier;
                // Vary particle velocity based on velocity that object entered water at.
                particleSettings.startSpeedMultiplier = Vector3.Magnitude(particleVector);
                if (particleVector != Vector3.zero)
                {
                    particles.transform.forward = particleVector;
                }
                particles.Emit(numParticles);

                if (!audioSource.isPlaying)
                {
                    float interactSpeed = Vector3.Magnitude(body.velocity);
                    audioSource.volume = interactSpeed / maxSplashSpeed;
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(sound);
                }
            }
        }

        /// <summary>
        /// Sets the inWater field on the object.
        /// </summary>
        /// <param name="collider">The object's collider.</param>
        /// <param name="inWater">The new value of inWater.</param>
        private void SetInWater(Collider collider, bool inWater)
        {
            Submergeable submergeable = collider.GetComponentInParent<Submergeable>();
            if (submergeable != null)
            {
                submergeable.inWater = inWater;
            }
        }
    }
}