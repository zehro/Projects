using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Pipe;

// The Bird class is a coordinator, which manages and times many different subsystems that together constitute the
// entire bird character. There is a BirdNavigator run on Unity's navigation system which can walk around and
// intelligently respond to obstacles, walls, and ledges; a BirdPhysics run on Unity's physics which handles
// movement and collision when the bird is rolling; a BirdAnimator which does procedural animations to tie those
// two systems together; a BirdMesh which makes the skinned mesh match those procedural animations; a BirdPaint
// which handles all paint IO behavior; and a BirdSpeaker which plays sounds.

namespace YeggQuest.NS_Bird
{
    public class Bird : MonoBehaviour
    {
        public BirdAnimator animator;           // The animator  - procedurally moves its hierarchical skeleton in code
        public BirdMesh mesh;                   // The mesh      - copies animator into skinned skeleton
        public BirdNavigator navigator;         // The navigator - walks around with the aid of Unity's navigation
        public BirdPaint paint;                 // The paint     - handles giving and receiving paint to/from the world
        public BirdPhysics physics;             // The physics   - rolls around with the aid of Unity's physics
        public BirdSpeaker speaker;             // The speaker   - plays all sounds

        private BirdState state;                // the current state the bird is in

        void Update()
        {
            // Force-disable the bird if the game is paused. This is usually not necessary,
            // but is done to circumvent some moderately expensive operations the bird would
            // be doing for no reason while paused (foot raycasting, etc.) Worth noting that
            // the coroutines in BirdAnimator and BirdAnimatorLeg stay live, but simply do
            // nothing because they use deltaTime as their increment.

            if (Time.timeScale == 0)
                return;

            // The main state machine that controls the bird. This coordinates state changes
            // between the bird's various component systems.

            switch (state)
            {
                case BirdState.Walking:

                    if (Yinput.BirdSwap())
                    {
                        state = BirdState.Rolling;
                        navigator.Deactivate();
                        physics.Activate();
                    }

                    break;

                case BirdState.Rolling:
                    
                    if (Yinput.BirdSwap() && CanStartWalking())
                    {
                        state = BirdState.ToWalking;
                        physics.Deactivate();
                        animator.ToWalking();
                    }

                    break;
            }

            // Update the bird's subsystems right after the state is updated, to ensure there's
            // no 1-frame lag between state changes here and the behavior of the subsystems.

            animator.CustomUpdate();
            navigator.CustomUpdate();
            physics.CustomUpdate();
        }

        // ======================================================================================================================== MESSAGES

        // Tells the bird to spawn out of the given spawner.

        public void Spawn()
        {
            if (state == BirdState.Unspawned)
            {
                state = BirdState.ToWalking;
                animator.Spawn();
            }
        }

        // Tells the bird to despawn after the given amount of time.

        public void Despawn(float time)
        {
            if (state != BirdState.Unspawned)
            {
                animator.Despawn(time);
            }
        }

        // Tells the bird to enter the given pipe.

        public void EnterPipe(Pipe pipe, bool forwards)
        {
            if (state == BirdState.Rolling || state == BirdState.Walking)
            {
                state = BirdState.InPipe;
                animator.EnterPipe(pipe, forwards);
                navigator.Deactivate();
                physics.Deactivate();
            }
        }

        // Tells the bird to jiggle.

        public void Jiggle(float jiggle)
        {
            animator.AddJiggle(jiggle);
        }

        // This is a message sent by the bird animator after it completes the ToWalking animation,
        // which is the bouncy kip-up done between the Rolling and Walking states. This method just
        // initializes the walking state (activates the bird navigator.)

        internal void ToWalkingCompleted()
        {
            state = BirdState.Walking;
            navigator.Activate();
        }

        // This is a message sent by the bird animator after it completes the Despawn animation.
        // This method sets the state to Unspawned and resets the bird.

        internal void DespawnCompleted()
        {
            state = BirdState.Unspawned;
            navigator.Deactivate();
            paint.Clean();
            physics.Deactivate();
        }

        // This is a message sent by the bird animator after it completes the InPipe animation.
        // This method gives the control of the bird backs to the physics system.

        internal void InPipeCompleted()
        {
            state = BirdState.Rolling;
            physics.Activate();
        }

        // ======================================================================================================================== GETTERS

        // Gets the state of the bird. Used by the BirdAnimator in its state machine.

        internal BirdState GetState()
        {
            return state;
        }

        // Gets a vector representing how the bird is trying to move. Used by the BirdNavigator and BirdPhysics.

        internal Vector3 GetMovementInput()
        {
            float h = Yinput.MovementHorizontal();
            float v = Yinput.MovementVertical();
            Vector3 move = new Vector3(h, 0, v);
            return Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0) * Vector3.ClampMagnitude(move, 1);
        }

        // Gets the world-space position of the bird's center

        public Vector3 GetPosition()
        {
            return animator.hips.position;
        }

        // ======================================================================================================================== HELPERS

        // Queries the bird's subsystems to see if the bird can switch from rolling to walking.
        // Descriptions of the subsystems' criteria can be found in their own CanStartWalking() methods.

        private bool CanStartWalking()
        {
            return animator.CanStartWalking() && navigator.CanStartWalking() && physics.CanStartWalking();
        }
    }

    public enum BirdState
    {
        Unspawned, ToWalking, Walking, Rolling, InPipe
    }
}