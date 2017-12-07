using UnityEngine;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Arrow property that lets the arrow travel through objects.
    /// </summary>
    public class GhostArrow : ArrowProperty
    {
        public override void Init()
        {
            base.Init();
            // Turn the arrow into a trigger so it can travel through objects
			GetComponentInChildren<Collider>().isTrigger = true;
        }

        public override void Effect(PlayerID hitPlayer) { }
    } 
}
