using UnityEngine;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// Handles the animation component for events like pausing
    /// </summary>
	public class AnimationInfo : MonoBehaviour
	{
        // Speed of the animator to save
		private float speed = 0f;

        // Reference to the animator
        private Animator anim;

		void OnEnable()
		{
			//Data.GameManager.GamePause += PauseAnimator;
			//Data.GameManager.GameUnpause += UnpauseAnimator;
		}
		void OnDisable()
		{
			//Data.GameManager.GamePause -= PauseAnimator;
			//Data.GameManager.GameUnpause -= UnpauseAnimator;
		}

        // Initialize
        void Start()
        {
            anim = GetComponent<Animator>();
        }
		
        /// <summary>
        /// Pausing the anumator
        /// </summary>
		public void PauseAnimator()
		{
			speed = anim.speed;
			anim.speed = 0;
		}
		
        /// <summary>
        /// Unpausing the animator
        /// </summary>
		public void UnpauseAnimator()
		{
			anim.speed = speed;
		}
	}
}
