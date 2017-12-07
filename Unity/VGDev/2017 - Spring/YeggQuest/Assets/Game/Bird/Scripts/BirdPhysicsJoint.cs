using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The BirdPhysicsJoint is a simple implementation of a springy slider joint.
// It is built on a ConfigurableJoint, initialized with two positional axes
// locked and one axis limited, with the limited axis being the one the
// spring force is applied on.

namespace YeggQuest.NS_Bird
{
    public class BirdPhysicsJoint : MonoBehaviour
    {
        public Rigidbody r1;
        public Rigidbody r2;

        private bool init;                          // whether or not the joint has been initialized
        private ConfigurableJoint joint;            // the ConfigurableJoint this joint is built on

        private float springRestingLength = 0.1f;   // the resting length of the spring joint
        private float springStrength = 300f;        // the strength of the spring joint

        internal void InitializeJoint(BirdPhysicsJointAxis freeAxis)
        {
            init = true;

            joint = r1.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = r2;
            joint.anchor = Vector3.zero;
            joint.connectedAnchor = Vector3.zero;
            joint.enableCollision = true;

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

            SoftJointLimit limit = new SoftJointLimit();
            limit.limit = 0.025f;
            joint.linearLimit = limit;

            if (freeAxis != BirdPhysicsJointAxis.X)
                joint.xMotion = ConfigurableJointMotion.Locked;
            if (freeAxis != BirdPhysicsJointAxis.Y)
                joint.yMotion = ConfigurableJointMotion.Locked;
            if (freeAxis != BirdPhysicsJointAxis.Z)
                joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }

        void FixedUpdate()
        {
            if (init)
            {
                Vector3 springForce = r2.position - r1.position;
                float displacement = springForce.magnitude - springRestingLength;
                springForce = springForce.normalized * displacement * springStrength;

                r1.AddForce(springForce);
                r2.AddForce(-springForce);
            }
        }
    }

    public enum BirdPhysicsJointAxis
    {
        X, Y, Z
    }
}