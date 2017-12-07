using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A PaintVolume sends PaintRequests of its color to all Paintable objects
// which inhabit its trigger volume (the PaintVolume is just meant to be a
// region of space which gives paint, not a solid object.) Because there is
// no raycast info involved or available, this sends the color-only PaintRequest.

namespace YeggQuest.NS_Paint
{
    public class PaintVolume : MonoBehaviour
    {
        private static int maxInhabitants = 64;         // how many things the trigger can affect per frame at maximum

        public Vector3 offset = Vector3.zero;           // the offset of this volume in world space
        public Vector3 halfExtents = Vector3.one;       // the size of this volume in world space
        public PaintColor color = PaintColor.Clear;     // the PaintColor this volume gives to inhabitants

        private Collider[] inhabitants;                 // what's currently inside the volume (stored so it's non-alloc)

        void Start()
        {
            inhabitants = new Collider[maxInhabitants];
        }

        void Update()
        {
            // Go through everything inhabiting the volume on this frame,
            // and if anything is paintable, send it a paint request.
            // Color only (no raycast information.)

            for (int i = 0; i < Physics.OverlapBoxNonAlloc(transform.position + offset, halfExtents, inhabitants); i++)
            {
                Paintable p = inhabitants[i].GetComponent<Paintable>();
                if (p != null)
                    p.Paint(new PaintRequest(color));
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = (color == PaintColor.Clear ? Color.white : PaintColors.ToColor(color));
            Gizmos.DrawWireCube(transform.position + offset, halfExtents * 2);
        }
    }
}