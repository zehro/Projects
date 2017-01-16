
using UnityEngine;

namespace Assets.Scripts.Arrows
{
    public class TrackingArrow : ArrowProperty
    {
        /// <summary>
        /// Time an arrow can follow a target before stopping
        /// </summary>
        public static float trackingTime = 4f;

        public override void Init()
        {
            base.Init();
            GetComponent<Rigidbody>().useGravity = false;
        }

        public override void Effect(PlayerID hitPlayer) { }
    }
}
