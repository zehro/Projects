using UnityEngine;

namespace YeggQuest.NS_Bird
{
    // Makes the bird blow bubbles when in water.
    
    public class BirdBubbles : MonoBehaviour
    {
        public Bird bird;                               // the bird
        private ParticleSystem particles;               // the particle system
        
        private void Start()
        {
            particles = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            transform.position = bird.GetPosition();

            if (bird.physics.inWater)
            {
                if (particles.isStopped)
                    particles.Play();
            }

            else if (!particles.isStopped)
            {
                particles.Stop();
            }
        }
    }
}