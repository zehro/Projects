using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The BirdPhysicsNode is a simple class attached to each of the bird's six blobby nodes,
// which can report to the BirdPhysics whether or not the bird should be able to jump.

namespace YeggQuest.NS_Bird
{
    public class BirdPhysicsNode : MonoBehaviour
    {
        public Rigidbody body;
        
        private int jumpTimer = 0;                  // if greater than 0, the bird can jump. decremented every physics tick back down to 0
        private int jumpTimerReset = 3;             // the number of physics ticks this node can be not touching anything but still allow jumping
        private float jumpNormalCutoff = 0.75f;     // the cutoff value for a surface being "jumpable" - its normal must point upwards this amount
        private float frictionNoise;                // how much this node should be contributing to the bird's friction sounds

        void Awake()
        {
            body = GetComponent<Rigidbody>();
        }
        
        void FixedUpdate()
        {
            if (jumpTimer > 0)
                jumpTimer--;
        }

        void Update()
        {
            float dt = Time.smoothDeltaTime * 60;
            frictionNoise *= 1 - (0.15f * dt);
        }

        void OnCollisionStay(Collision col)
        {
            if (!col.gameObject.CompareTag("NoJump"))
            {
                foreach (ContactPoint c in col.contacts)
                {
                    if (c.normal.y > jumpNormalCutoff)
                    {
                        jumpTimer = jumpTimerReset;
                        break;
                    }
                }
            }

            float f = col.relativeVelocity.sqrMagnitude;
            frictionNoise = Mathf.Clamp01(Mathf.InverseLerp(1, 5, f));
        }

        internal bool CanJump()
        {
            return (jumpTimer > 0);
        }

        internal float GetFrictionNoise()
        {
            return frictionNoise;
        }
    }
}