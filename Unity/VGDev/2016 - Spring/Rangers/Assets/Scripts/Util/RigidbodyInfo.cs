using UnityEngine;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// Information about an object's rigidbody
    /// Used for pausing and unpausing
    /// </summary>
    public class RigidbodyInfo : MonoBehaviour
    {
        // Is rigidbody kinematic?
        private  bool kinematic;

        // Is rigidbody fixed angle?
        private bool freezeRotation;

        // Reference to old velocity
        private Vector3 vel = Vector3.zero;
        // Reference to spinning velocity
        private Vector3 angVel = Vector3.zero;
        // Reference to the rigidbody
        private Rigidbody body;

        // Initialize
        void Start()
        {
            body = GetComponent<Rigidbody>();
        }

		// Setting up events
		void OnEnable()
		{
			//Data.GameManager.GamePause += PauseMotion;
			//Data.GameManager.GameUnpause += UnpauseMotion;
		}

		void OnDisable()
		{
			//Data.GameManager.GamePause -= PauseMotion;
			//Data.GameManager.GameUnpause -= UnpauseMotion;
		}

        /// <summary>
        /// Pause rigidbody
        /// </summary>
        public void PauseMotion()
        {
            kinematic = body.isKinematic;
            freezeRotation = body.freezeRotation;
            if (!kinematic && !freezeRotation)
            {
                // Save velocity
                vel = body.velocity;
                // Save angular velocity
                angVel = body.angularVelocity;

                // Set fixed angle
                body.freezeRotation = true;
                // Set to kinematic to pause
                body.isKinematic = true;
            }
            else if (kinematic)
            {
                // Save velocity
                vel = body.velocity;
                // Set to kinematic to pause
                body.isKinematic = true;
            }
        }

        /// <summary>
        /// Unpause rigidbody
        /// </summary>
        public void UnpauseMotion()
        {
            if (!kinematic && !freezeRotation)
            {
                // Set to not kinematic to unpause
                body.isKinematic = false;

                // Set to not fixed angle
                body.freezeRotation = false;
                // Reapply angular velocity
                body.angularVelocity = angVel;
                // Reapply velocity
                body.velocity = vel;

                // Reset reference
                angVel = Vector3.zero;
                // Reset reference
                vel = Vector3.zero;
            }
            else if (!kinematic)
            {
                // Set to not kinematic to unpause
                body.isKinematic = false;
                // Reapply velocity
                body.velocity = vel;
                // Reset reference
                vel = Vector3.zero;
            }
        }
    }
}
