using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// The BirdNavigator handles the bird's walking behavior. It uses a NavMeshAgent tied to
// Unity's navigation system, which gives many nice behaviors (detailed below) for no
// extra work. The BirdNavigator also reports various movement information, and ensures
// the bird can only switch back to walking where there is walkable ground.

namespace YeggQuest.NS_Bird
{
    public class BirdNavigator : MonoBehaviour
    {
        public Bird bird;                           // the bird

        private NavMeshAgent agent;                 // the NavMeshAgent controlling walking
        private bool activated = false;             // whether or not the agent is walking

        private bool validWalkPosition = false;     // whether or not the disabled agent should be able to return to walking

        private Vector3 pos;
        private Vector3 posPrev;
        private float rot;
        private float rotPrev;
        private float linearSpeed;
        private float angularSpeed;
        private Vector3 velocity;

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();

            pos = transform.position;
            rot = transform.rotation.eulerAngles.y;
            posPrev = pos;
            rotPrev = rot;
        }

        internal void CustomUpdate()
        {
            // When the navigator is activated, it uses Unity's navigation system to walk around
            // on a baked world NavMesh. This gives the bird several behaviors for free - he slows
            // down realistically when being told to walk into a wall, he cannot walk off edges,
            // and he does some simple (but intelligent-feeling) pathfinding around obstacles.
            // The bird walks less quickly when underwater.

            if (activated)
            {
                Vector3 move = bird.GetMovementInput();
                float speed = bird.physics.inWater ? 0.4f : 1;
                agent.SetDestination(agent.transform.position + move * agent.radius * speed);
            }

            // When the navigator is deactivated, it tries to find a valid positon on the baked
            // world NavMesh close to the bird's physics every frame. The bird can only transition
            // back to walking if the navigator finds a close enough valid position in that frame.

            else if (bird.GetState() == BirdState.Rolling)
            {
                NavMeshHit hit;
                validWalkPosition = false;

                if (NavMesh.SamplePosition(bird.physics.center.transform.position, out hit, agent.radius, 1))
                {
                    validWalkPosition = true;
                    Warp(hit.position);
                }

                float heading = Yutil.VectorYaw(velocity);
                float t = Mathf.Clamp01(Mathf.InverseLerp(0.1f, 4f, velocity.magnitude)) * 60 * Time.smoothDeltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, heading, 0), t * 0.1f);
            }

            // Calculate linear speed, angular speed, and velocity

            if (Time.deltaTime == 0)
                return;

            posPrev = pos;
            rotPrev = rot;
            pos = transform.position;
            rot = transform.rotation.eulerAngles.y;
            linearSpeed = (pos - posPrev).magnitude / Time.deltaTime;
            angularSpeed = Mathf.Lerp(angularSpeed, Mathf.DeltaAngle(rotPrev, rot) / Time.deltaTime, 0.15f * 60 * Time.smoothDeltaTime);
            velocity = (pos - posPrev) / Time.deltaTime;
        }

        // ======================================================================================================================== MESSAGES

        // Activate the navigator.

        internal void Activate()
        {
            activated = true;
        }

        // Deactivate the navigator, clearing the current path to
        // prevent Unity's navigation from remembering and using it.

        internal void Deactivate()
        {
            activated = false;
            agent.ResetPath();
        }

        // Immediately moves the navigator to a position.

        internal void Warp(Vector3 position)
        {
            agent.Warp(position);
        }

        // ======================================================================================================================== GETTERS

        // Gets the linear speed of the navigator in world units per second

        internal float GetLinearSpeed()
        {
            return linearSpeed;
        }

        // Gets the angular speed of the navigator in degrees per second

        internal float GetAngularSpeed()
        {
            return angularSpeed;
        }

        // Gets the velocity of the navigator in world units

        internal Vector3 GetVelocity()
        {
            return velocity;
        }

        // Returns whether or not the navigator can start walking, which is basically
        // just checking if there is ground to walk on (see validWalkPosition above.)

        internal bool CanStartWalking()
        {
            return validWalkPosition;
        }
    }
}