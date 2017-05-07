using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YeggQuest.NS_ProcBlock;
using YeggQuest.NS_Water;

// The BirdPhysics is responsible for setting up and coordinating the six BirdPhysicsNodes that
// constitute the bird's physical state. They are arranged orthogonally around a center rigidbody
// and connected to it each with one BirdPhysicsJoint. 

namespace YeggQuest.NS_Bird
{
    public class BirdPhysics : MonoBehaviour, Submergeable
    {
        public Bird bird;
        public Rigidbody center;
        public BirdPhysicsNode[] nodes;

        private bool activated = false;

        private Vector3[] nodePositions = new Vector3[6];
        private Quaternion[] nodeRotations = new Quaternion[6];
        private BirdPhysicsJoint[] joints = new BirdPhysicsJoint[6];

        private Vector3 velocityPrev;
        private Vector3 velocityDelta;
        
        private float frictionNoise = 0;
        private float frictionNoiseFalloff = 0.2f;

        private int activatedLayer;
        private int deactivatedLayer;

        private float moveForce = 10f;
        private float maxLateralSpeed = 6f;
        private float jumpForce = 300f;
        private float jumpCooldown = 0;
        private float jumpCooldownTime = 0.3f;

        private float walkingSpeedLimit = 5f;
        
        public bool inWater
        {
            get;
            set;
        }

        private float waterDrag = 4;
        private float waterBuoyancy = 80;

        void Start()
        {
            for (int i = 0; i < 6; i++)
            {
                joints[i] = gameObject.AddComponent<BirdPhysicsJoint>();
                joints[i].r1 = center;
                joints[i].r2 = nodes[i].body;
                joints[i].InitializeJoint((BirdPhysicsJointAxis) (i % 3));

                nodePositions[i] = nodes[i].transform.localPosition;
                nodeRotations[i] = nodes[i].transform.localRotation;
            }

            velocityPrev = center.velocity;

            activatedLayer = LayerMask.NameToLayer("Bird (Activated)");
            deactivatedLayer = LayerMask.NameToLayer("Bird (Deactivated)");
        }

        internal void CustomUpdate()
        {
            // First off, calculate the delta time factor.

            float dt = 60 * Time.smoothDeltaTime;

            // When the physics is activated, the player controls
            // the bird by applying movement and jumping forces.

            if (activated)
            {
                // In order to control the bird, the center node applies forces directly from
                // player input, which emerges into a rolling behavior in the physics simulation.
                // The player can only apply movement forces either if the resultant speed would be less
                // than the max lateral speed, or the force they're applying would reduce the magnitude.

                Vector3 move = bird.GetMovementInput();
                
                Vector3 lateral = Vector3.Scale(center.velocity, new Vector3(1, 0, 1));
                if ((lateral + move).magnitude < maxLateralSpeed || (lateral + move).magnitude < lateral.magnitude)     // (acceleration || deceleration)
                    center.AddForce(move * moveForce * dt, ForceMode.Acceleration);
                else
                {
                    Vector3 perp = Vector3.Cross(lateral, new Vector3(0, 1, 0));
                    move = Vector3.Project(move, perp);

                    center.AddForce(move * moveForce * dt, ForceMode.Acceleration);
                }

                // The player can jump as long as they haven't jumped in jumpCooldownReset seconds,
                // and at least one BirdPhysicsNode is reporting that it's on a jumpable surface.
                // They can also always jump in water (twice as strong and plays a different sound.)

                jumpCooldown = Mathf.Max(0, jumpCooldown - Time.deltaTime);

                if (Yinput.BirdJump() && jumpCooldown == 0)
                {
                    if (inWater)
                    {
                        center.AddForce(Vector3.up * jumpForce * 2, ForceMode.Acceleration);
                        jumpCooldown = jumpCooldownTime;
                        bird.speaker.PlaySwimSound();
                    }

                    else for (int i = 0; i < 6; i++)
                    {
                        if (nodes[i].CanJump())
                        {
                            center.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                            jumpCooldown = jumpCooldownTime;
                            bird.speaker.PlayJumpSound();
                            break;
                        }
                    }
                }

                // Add a constant buoyant force when in water.

                if (inWater)
                    center.AddForce(Vector3.up * waterBuoyancy * dt);
            }

            // When the physics is deactivated, everything under and including BirdPhysics is
            // reset to default positions and orientations (crucially, not root scale) so that
            // everything is in the exact right position for physics reinitialization.

            else
            {
                transform.position = Yutil.RemoveScale(bird.animator.transform, bird.animator.hips.transform.position);
                transform.rotation = bird.animator.hips.transform.rotation * Quaternion.Euler(90, 0, 0);
                center.transform.position = transform.position;
                center.transform.rotation = transform.rotation;

                for (int i = 0; i < 6; i++)
                {
                    nodes[i].transform.localPosition = nodePositions[i];
                    nodes[i].transform.localRotation = nodeRotations[i];
                }
            }

            // Increase drag if in water

            center.drag = inWater ? waterDrag : 0;

            // Calculate acceleration (velocity changes)

            velocityDelta = (center.velocity - velocityPrev);
            velocityPrev = center.velocity;

            // Calculate friction noise amount

            float f = 0;
            foreach (BirdPhysicsNode n in nodes)
                f += n.GetFrictionNoise() * 0.5f;
            f = Mathf.Clamp01(f);

            if (f > frictionNoise)
                frictionNoise = f;
            frictionNoise *= 1 - (frictionNoiseFalloff * dt);
        }

        // ======================================================================================================================== MESSAGES

        // Activate the physics. The center and all nodes become non-kinematic,
        // are put into the solid layer, and are given the velocity the navigator
        // had (for a smooth-looking transition.)

        internal void Activate()
        {
            activated = true;
            SetKinematic(false);
            SetLayer(activatedLayer);
            SetVelocity(bird.navigator.GetVelocity());
        }

        // Deactivate the physics. The center and all nodes become kinematic
        // and are put into the non-solid layer where they began.

        internal void Deactivate()
        {
            activated = false;
            SetKinematic(true);
            SetLayer(deactivatedLayer);
        }

        // ======================================================================================================================== GETTERS

        // Returns the derivative (delta) of the physics center's velocity.
        // Used by the BirdAnimator to control the jiggle behavior.

        internal Vector3 GetVelocityDelta()
        {
            return velocityDelta;
        }

        // Returns whether or not the physics can start walking, which just
        // checks if the physics isn't moving faster than the walkingSpeedLimit.

        internal bool CanStartWalking()
        {
            return (center.velocity.magnitude < walkingSpeedLimit);
        }
        
        // Returns how loud the friction noises for the bird should be (the sounds
        // made when rolling.) This is used by the BirdSpeaker.

        internal float GetFrictionNoise()
        {
            return frictionNoise;
        }

        // ======================================================================================================================== HELPERS

        // Private helper which changes the center and
        // all nodes to be kinematic or not.

        private void SetKinematic(bool isKinematic)
        {
            center.isKinematic = isKinematic;
            for (int i = 0; i < 6; i++)
                nodes[i].body.isKinematic = isKinematic;
        }

        // Public helper which sets the velocity of
        // the center and all nodes.

        public void SetVelocity(Vector3 velocity)
        {
            center.velocity = velocity;
            for (int i = 0; i < 6; i++)
                nodes[i].body.velocity = velocity;
        }

        // Private helper which sets the layer of
        // the center and all nodes.

        private void SetLayer(LayerMask layer)
        {
            gameObject.layer = layer;
            center.gameObject.layer = layer;
            for (int i = 0; i < 6; i++)
                nodes[i].gameObject.layer = layer;
        }
    }
}