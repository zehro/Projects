using UnityEngine;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// When it hits a player, applies more knockback - outward from point of impact
    /// </summary>

    public class HeavyKnockbackArrow : ArrowProperty
    {
        // Force of the knockback
        private const float DELTA_V = 12f;

        public override void Effect(PlayerID hitPlayer)
        {
            //applies knockback force iff a player was hit
            if (hitPlayer != 0)
            {
                Player.Controller hitPlayerController = Data.GameManager.instance.GetPlayer(hitPlayer);
                //hitPlayerController.GetComponent<Rigidbody>().AddExplosionForce(HeavyKnockbackForce, transform.position, HeavyKnockbackRadius);
				hitPlayerController.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(transform.forward) * DELTA_V, ForceMode.VelocityChange);
            }
        }
    }
}