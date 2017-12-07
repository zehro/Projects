using UnityEngine;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// Handles the particle system attached to the game object
    /// </summary>
	public class ParticleSystemInfo : MonoBehaviour
	{
        // Reference to the particle system
        private ParticleSystem system;

		void OnEnable()
		{
			//Data.GameManager.GamePause += PauseParticles;
			//Data.GameManager.GameUnpause += UnpauseParticles;
		}
		void OnDisable()
		{
			//Data.GameManager.GamePause -= PauseParticles;
			//Data.GameManager.GameUnpause -= UnpauseParticles;
		}

        // Initialize
        void Start()
        {
            system = GetComponent<ParticleSystem>();
        }

        /// <summary>
        /// Pauses the particle system
        /// </summary>
		public void PauseParticles()
		{
			system.Pause();
		}

        /// <summary>
        /// Unpauses the particle system
        /// </summary>
		public void UnpauseParticles()
		{
            system.Play();
		}
	}
}
