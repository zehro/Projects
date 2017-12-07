using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Pipe;
using YeggQuest.NS_Spawner;
using YeggQuest.NS_Spline;

// The BirdAnimator is a large, important class which is responsible for tying together the bird's various
// underlying movement systems (navigation and physics) into a cohesive, smooth, and visually appealing whole.
// Because this is probably the most complicated and magic-numbery task under the hood, it offloads this task
// partially into several subcomponents: BirdAnimatorHead, BirdAnimatorLeg, and BirdAnimatorNode. However, the
// BirdAnimator itself controls the root movement, rotation, and scaling, and also coordinates leg planting.

namespace YeggQuest.NS_Bird
{
    public class BirdAnimator : MonoBehaviour
    {
        public GameCoordinator coordinator;         // the game coordinator

        public Bird bird;                           // the bird
        public Transform hips;                      // the hips
        public BirdAnimatorLeg legL;                // the left leg
        public BirdAnimatorLeg legR;                // the right leg

        private Quaternion footRot;                 // the slope rotation, which is part of the base rotation of the animator
        private BirdAnimatorLeg activeLeg;          // which leg (L or R) is currently being planted

        private float walkingHipHeight = 0.40f;     // how far from the root position the hips are when the bird is walking
        private float rollingHipHeight = 0.25f;     // how far from the root position the hips are when the bird is rolling

        private float jiggle = 0;                   // the bird's squash-and-stretch (+ is stretch, - is squash)
        private float jiggleBound = 0.75f;          // the positive and negative limit of the jiggle value
        private float jiggleVel = 0;                // how quickly the jiggle value is currently changing
        private float jiggleAccel = 0.1f;           // how quickly the jiggle value accelerates towards 0
        private float jiggleDrag = 0.1f;            // the drag of the jiggle value

        private float size = 0.0001f;               // the bird's overall size multiplier (used to grow and shrink when spawning / despawning)
        private float sizeUnspawned = 0.0001f;      // the size when the bird is unspawned (not 0 to prevent scale errors)

        private bool isToWalking = false;           // is the bird currently playing the ToWalking animation?
        private bool isDespawning = false;          // is the bird currently playing the despawning animation?
        private bool isBalled = true;               // should the bird look like it's walking, or rolling in a ball?
        private float ballAmount = 1;               // how balled up the bird is (interpolates between 0 and 1)
        private float pipeAmount = 0;               // how inside a pipe the bird is visually (used to play traversal sound)

        void Start()
        {
            footRot = Quaternion.identity;
            Plant(legL);
        }

        internal void CustomUpdate()
        {
            // This is a big one: the animation update.

            // First, set the root rotation of the animator. This comes first because it affects almost every other calculation.
            // The root rotation is set to a combination of both the foot rotation and the rotation of the bird navigator (where
            // the "foot rotation" is a quaternion that continuously lerps to the average of the two slopes the feet are planted on.)
            // We also calculate a tidbit, "dt", to use as our delta time factor (which gets used many more times in this method.)

            float dt = 60 * Time.smoothDeltaTime;
            footRot = Quaternion.Slerp(footRot, Quaternion.Slerp(legL.GetFootRotation(), legR.GetFootRotation(), 0.5f), 0.25f * dt);
            transform.rotation = footRot * Quaternion.Euler(-90, 0, bird.navigator.transform.rotation.eulerAngles.y);

            // Now that we've set the root rotation, we need to set the root position, the hip position, and the hip rotation. 
            // These three depend on the bird state, so we run a semi-complete state machine (note that ToWalking and InPipe
            // are both missing, because they're handled by coroutines further down):

            switch (bird.GetState())
            {
                // When the bird is walking, the root position continuously lerps to the position of the bird navigator.
                // The hip position and rotation are done locally, and do a little procedural bouncing and swaying using
                // information from the legs about how they're planting, as well as the bird navigator's angular speed.

                case BirdState.Walking:

                    isBalled = false;

                    transform.position = Vector3.Lerp(transform.position, bird.navigator.transform.position, 0.1f * dt);

                    float turn = bird.navigator.GetAngularSpeed() * 0.025f;
                    float sway = legL.GetFootPosition().y - legR.GetFootPosition().y;
                    float bob = Mathf.Sin(Time.time * 3) * 0.005f;
                    if (activeLeg != null)
                        bob -= Mathf.Sin(activeLeg.GetPlantProgress() * 2 * Mathf.PI)
                        * 0.025f * (legL.GetPlantDistance() + legR.GetPlantDistance());
                    hips.localPosition = new Vector3(sway * 0.15f, 0, walkingHipHeight + bob);
                    hips.localRotation = Quaternion.Euler(Mathf.Cos(Time.time * 3), turn + sway * 25, 0);

                    break;

                // When the bird is rolling, the root position is set directly underneath the center of the bird physics
                // ("underneath" depends on the foot rotation). The hip position and rotation are done in world space,
                // and are simply set to the position and rotation of the center of the bird physics.

                case BirdState.Rolling:

                    isBalled = true;

                    float hipHeight = Mathf.Lerp(walkingHipHeight, rollingHipHeight, ballAmount);
                    transform.position = bird.physics.center.transform.position - footRot * Vector3.up * hipHeight;

                    hips.position = bird.physics.center.transform.position;
                    hips.rotation = bird.physics.center.transform.rotation * Quaternion.Euler(-90, 0, 0);

                    break;

                // When the bird is unspawned, the root position is set to the position of the active spawner.
                // The hip position and rotation are reset.

                case BirdState.Unspawned:

                    isBalled = true;
                    Spawner spawner = coordinator.GetActiveSpawner();
                    if (spawner)
                        transform.position = spawner.transform.position;
                    hips.localPosition = Vector3.zero;
                    hips.localRotation = Quaternion.identity;

                    break;
            }

            // Now that the positions and rotations have been set for the root and hips, we can start to update the
            // other parts of the bird's state. First, the "ball amount", an interpolant value for how balled up the
            // bird is, which affects the visual behavior of the legs and nodes.

            ballAmount = Mathf.Clamp01(ballAmount + (isBalled ? 1 : -1) * 0.0625f * dt);

            // Update the bird's squash-and-stretch. This is done by funneling velocity changes from the bird's physics
            // handler into forces affecting a springy "jiggle" value, which affects the root scale of the animator.

            Vector3 jiggleDelta = bird.GetState() == BirdState.Unspawned ? Vector3.zero : bird.physics.GetVelocityDelta();
            jiggleVel -= jiggleDelta.y * 0.016f;
            jiggleVel += Mathf.Abs(jiggleDelta.x) * 0.008f;
            jiggleVel += Mathf.Abs(jiggleDelta.z) * 0.008f;

            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;
            jiggle = Mathf.Clamp(jiggle, -jiggleBound, jiggleBound);

            // Using the bird's size multiplier (which is what increases and decreases when the bird spawns and unspawns),
            // set the final scale of the bird as a combination of jiggle and size.
            
            transform.localScale = new Vector3(1 - jiggle, 1 - jiggle, 1 + jiggle) * Mathf.Sqrt(size);

            // Finally, update the legs. The legs are given their plant target and rotation every frame (even while
            // planting) because it allows them to be in the most optimal place when they do hit the ground. Note
            // that the legs are always planting, even when the character is not walking or even moving. This is
            // because the foot rotation with regard to slopes is used regardless of what state the bird is in.

            legL.SetPlantTarget(bird.navigator.transform.position + bird.navigator.transform.right * -0.2f);
            legR.SetPlantTarget(bird.navigator.transform.position + bird.navigator.transform.right * +0.2f);
            legL.SetFootHeading(bird.navigator.transform.rotation.eulerAngles.y - 15);
            legR.SetFootHeading(bird.navigator.transform.rotation.eulerAngles.y + 15);
            legL.CustomUpdate();
            legR.CustomUpdate();
        }

        // ======================================================================================================================== MESSAGES

        // This is (basically) a callback function, called by the bird leg animators whenever they're
        // done with the planting maneuver. It plays a footstep sound from the given foot audio source
        // with the given strength as long as the bird isn't balled, and then begins planting the other leg.
        // If that leg isn't being told to move anywhere, plant it immediately (so that when the bird actually
        // begins moving, it starts an uninterrupted step.)

        internal void LegPlanted(AudioSource footAudio, float strength)
        {
            if (!isBalled && strength > 0)
                bird.speaker.PlayFootstepSound(footAudio, strength);

            if (activeLeg == legL)
                Plant(legR);
            else if (activeLeg == legR)
                Plant(legL);

            if (activeLeg.GetPlantDistance() == 0)
                activeLeg.PlantImmediately();
        }

        // A message the bird calls to begin the ToWalkingRoutine, which handles the playing of the
        // transition animation between the Rolling and Walking states.

        internal void ToWalking()
        {
            StartCoroutine(ToWalkingRoutine(0.5f, 2.5f, 1));
        }

        // A message the bird calls to begin the spawn animation - which is actually just the
        // ToWalkingRoutine with a custom animation time and arc height.

        internal void Spawn()
        {
            Spawner spawner = coordinator.GetActiveSpawner();
            
            bird.navigator.Warp(spawner.transform.position + spawner.spawnOffset);
            bird.navigator.transform.rotation = Quaternion.Euler(0, spawner.spawnRotation, 0);
            bird.speaker.PlaySpawnSound();

            StartCoroutine(ToWalkingRoutine(spawner.spawnTime, spawner.spawnArc, 2));
        }
        
        // A message the bird calls to do the despawn animation.

        internal void Despawn(float time)
        {
            if (CanDespawn())
            {
                pipeAmount = 0;
                StopAllCoroutines();
                StartCoroutine(DespawnRoutine(time));
            }
        }

        // A message the bird calls to begin the InPipeRoutine, which handles the
        // animation of the bird going through a pipe.

        internal void EnterPipe(Pipe pipe, bool forwards)
        {
            if (!isDespawning)
                StartCoroutine(InPipeRoutine(pipe, forwards));
        }

        // ======================================================================================================================== GETTERS

        // Gets the float parameter (0-1) of how balled up the bird is. This is used by the
        // bird animator legs and nodes so that they can smoothly interpolate between the
        // Rolling and Walking states.

        internal float GetBallAmount()
        {
            return ballAmount;
        }
        
        // Gets the float parameter (0-1) of how inside a pipe the bird is. This is used by the
        // bird speaker to play the pipe traversal sound.

        internal float GetPipeAmount()
        {
            return pipeAmount;
        }

        // Gets the float parameter of the bird's squash and stretch. This is used by the
        // bird speaker to play the jiggle audio.

        internal float GetJiggle()
        {
            return jiggle;
        }

        // Sets the float parameter of the bird's squash and stretch. This is used when
        // external objects want to compress or elongate the bird.

        internal void AddJiggle(float amount)
        {
            jiggleVel += amount;
        }

        // Gets the direction the bird is facing (which is from the navigator)

        internal float GetHeading()
        {
            return bird.navigator.transform.eulerAngles.y;
        }

        // Returns whether or not the animator can start walking,
        // which just checks if the bird is fully balled up

        internal bool CanStartWalking()
        {
            return (ballAmount == 1);
        }

        // Returns whether or not the bird can despawn

        internal bool CanDespawn()
        {
            return !isToWalking && !isDespawning;
        }

        // ======================================================================================================================== HELPERS

        // A private helper function that sends the "begin planting" message to the given leg
        // (and also designates it here as the active leg.) The planting time given to the leg
        // decreases with both how quickly the bird is moving and how quickly it is turning.

        private void Plant(BirdAnimatorLeg leg)
        {
            float plantTime = 0.325f;
            plantTime -= bird.navigator.GetLinearSpeed() * 0.05f;
            plantTime -= Mathf.Abs(bird.navigator.GetAngularSpeed()) * 0.000125f;

            activeLeg = leg;
            activeLeg.Plant(plantTime);
        }

        // A private coroutine that plays the special animation that occurs between the Rolling
        // and Walking states (the bird does a bouncy kip-up.) Also used as a spawn animation.

        private IEnumerator ToWalkingRoutine(float animationTime, float arcHeight, int flips)
        {
            isToWalking = true;

            Vector3 startPos = transform.position;
            Quaternion startRot = hips.localRotation;

            legL.PlantImmediately();
            legR.PlantImmediately();

            for (float f = 0; f < animationTime; f += Time.deltaTime)
            {
                float t = f / animationTime;
                size = Mathf.Max(size, Mathf.Clamp(t * 3, sizeUnspawned, 1));

                float hipHeight = Mathf.Lerp(walkingHipHeight, rollingHipHeight, ballAmount);
                float height = Mathf.Lerp(hipHeight, walkingHipHeight, t);
                height += (t * (1 - t)) * arcHeight;

                transform.position = Vector3.Lerp(startPos, bird.navigator.transform.position, t);
                hips.localPosition = Vector3.forward * height;
                hips.localRotation = Quaternion.Slerp(startRot, Quaternion.identity, t);
                hips.localRotation *= Quaternion.Euler(Mathf.SmoothStep(0, 1, t) * 360 * flips, 0, 0);

                isBalled = (f < animationTime - 0.25f);

                yield return null;
            }

            isToWalking = false;

            size = 1;
            jiggleVel = -0.08f;
            bird.ToWalkingCompleted();
            bird.speaker.PlayFootstepSound(legL.GetFootAudio(), 1);
            bird.speaker.PlayFootstepSound(legR.GetFootAudio(), 1);
        }

        // A private coroutine that plays the "despawning" animation, which waits until the
        // bird has completely shrunk and then disables everything.

        private IEnumerator DespawnRoutine(float animationTime)
        {
            isDespawning = true;

            for (float f = 0; f < animationTime; f += Time.deltaTime)
            {
                float t = f / animationTime;
                size = Mathf.Max(1 - t, sizeUnspawned);

                yield return null;
            }

            isDespawning = false;

            size = sizeUnspawned;
            bird.DespawnCompleted();
        }

        // A private coroutine that plays the "going through a pipe" animation. There's a brief
        // "getting sucked in" at the beginning, followed by the traversal through the pipe.

        private IEnumerator InPipeRoutine(Pipe pipe, bool forwards)
        {
            isBalled = true;
            bird.speaker.PlayPipeSound(true);

            // Entering

            Vector3 p0 = transform.position;
            Vector3 p1 = pipe.GetPoint(forwards ? 0 : 1);
            Vector3 m0 = bird.physics.center.velocity;
            Vector3 m1 = (forwards ? pipe.GetEntrance().forward : -pipe.GetExit().forward) * 15;
            Vector3 hipsLocalPos = hips.localPosition;
            Quaternion hipsLocalRot = hips.localRotation;

            for (float f = 0; f < pipe.enteringTime; f += Time.deltaTime)
            {
                float t = f / pipe.enteringTime;
                pipeAmount = t * t;

                p1 = pipe.GetPoint(forwards ? 0 : 1);

                transform.position = Spline.CalculateSpline(p0, p1, m0, m1, t);
                hips.localPosition = hipsLocalPos * (1 - t);
                hips.localRotation = Quaternion.Slerp(hipsLocalRot, Quaternion.identity, t);
                hips.localRotation *= Quaternion.Euler(t * t * 720, 0, 0);

                yield return null;
            }

            pipeAmount = 1;

            // Traversal

            for (float f = 0; f < pipe.traversalTime; f += Time.deltaTime)
            {
                float t = f / pipe.traversalTime;

                transform.position = pipe.GetPoint(forwards ? (t) : (1 - t));

                yield return null;
            }

            pipeAmount = 0;

            // Exiting

            float exitSpeed = pipe.GetLength() / pipe.traversalTime;
            Vector3 exitForce = (forwards ? pipe.GetExit().forward : -pipe.GetEntrance().forward) * exitSpeed;
            exitForce += (forwards ? pipe.GetExitVelocity() : pipe.GetEntranceVelocity());

            jiggleVel = -0.12f;
            bird.InPipeCompleted();
            bird.physics.SetVelocity(exitForce);
            bird.speaker.PlayPipeSound(false);
        }
    }
}