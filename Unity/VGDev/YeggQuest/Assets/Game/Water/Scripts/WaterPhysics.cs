using UnityEngine;

namespace YeggQuest.NS_Water
{
    // Applies forces to objects in water, simulating inertia.
    
    [RequireComponent(typeof(BoxCollider))]
    public class WaterPhysics : MonoBehaviour
    {
        private static int maxInhabitants = 64;         // How many things the water can move per frame at maximum

        private BoxCollider waterCollider;              // The collider attached to the water.
        private Vector3 posPrev;                        // The location of the water on the previous frame.

        private Collider[] inhabitants;                 // What's currently inside the volume (stored so it's non-alloc)

        private void Start()
        {
            waterCollider = GetComponent<BoxCollider>();
            posPrev = transform.position;

            inhabitants = new Collider[maxInhabitants];
        }
        
        private void FixedUpdate()
        {
            Vector3 delta = transform.position - posPrev;
            posPrev = transform.position;

            Vector3 halfExtents = waterCollider.bounds.extents;

            for (int i = 0; i < Physics.OverlapBoxNonAlloc(transform.position, halfExtents, inhabitants); i++)
            {
                Submergeable submergeable = inhabitants[i].GetComponentInParent<Submergeable>();
                if (submergeable == null)
                    continue;

                Rigidbody[] bodies = inhabitants[i].GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody body in bodies)
                    body.AddForce(delta / Time.fixedDeltaTime * 0.06f, ForceMode.VelocityChange);
            }
        }
    }
}