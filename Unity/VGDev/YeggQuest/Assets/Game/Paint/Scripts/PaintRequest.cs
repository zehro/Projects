using UnityEngine;

// A PaintRequest is a data object accepted by scripts that are Paintable in their Paint() method.
// It's a struct which can be initialized in two ways to convey two slightly different requests,
// as objects can be painted on both by entering volumes (like paint pools) and by being on the
// receiving ends of raycasts (like those cast by the bird on collisions.) This object essentially
// serves as an encompassing abstraction for these different behaviors. It's worth noting that
// different implementations handle the request very differently - e.g. the BirdPaint couldn't
// care less about what raycast hit it where, but PaintableFull can't function without that info.

namespace YeggQuest.NS_Paint
{
    public struct PaintRequest
    {
        public bool useRaycast;     // Whether or not raycast information exists and can be used
        public RaycastHit raycast;  // Raycast information (used for world position / lightmap coordinates)
        public PaintColor color;    // What color to paint this object
        public float strength;      // What strength to paint with (used by PaintableFull)

        // One way to send a paint request is based on a raycast. Both PaintableSimple
        // and PaintableFull need raycast information (world point / lightmap coordinates)
        // to act completely as expected. PaintableSimple CAN deal with just a color, but
        // PaintableFull cannot.

        public PaintRequest(RaycastHit raycast, PaintColor color, float strength)
        {
            useRaycast = true;
            this.raycast = raycast;
            this.color = color;
            this.strength = strength;
        }

        // Just send a color and not any raycast information (used in cases where
        // there isn't any to send, such as when an object enters a paint volume.)
        // The BirdPaint and PaintableSimples can accept this.

        public PaintRequest(PaintColor color)
        {
            useRaycast = false;
            raycast = new RaycastHit();
            this.color = color;
            strength = 1;
        }
    }
}