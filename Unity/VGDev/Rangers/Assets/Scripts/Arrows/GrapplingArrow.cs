
using Assets.Scripts.Data;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Arrows
{
    public class GrapplingArrow : ArrowProperty
    {
        public static float maxGrappleDistance = 20f;

        public override void Effect(PlayerID hitPlayer) { }

        void OnCollisionEnter(Collision col)
        {
            Controller player = GameManager.instance.GetPlayer(fromPlayer);
            Vector3 playerPos = player.transform.position + new Vector3(0, 1, 0);
            GameObject obj = col.gameObject;
            if ((obj.tag == "Ground" || (obj.transform.parent != null && obj.transform.parent.tag == "Ground")) &&
                !player.ParkourComponent.Grappling && Vector3.Distance(playerPos, transform.position) <= maxGrappleDistance)
            {
                Grapple grapple = player.gameObject.AddComponent<Grapple>();
                grapple.Ground = col.gameObject;
                grapple.Position = transform.position - col.gameObject.transform.position;
                player.ParkourComponent.Grappling = true;
            }
        }
    }
}