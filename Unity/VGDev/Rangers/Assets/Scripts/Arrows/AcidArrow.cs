using Assets.Scripts.Attacks;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// This is the component added to the arrow if the acid token was picked up.
    /// It will add an acid effect to the player that deals damage over time.
    /// </summary>
    public class AcidArrow : ArrowProperty
    {
        /// <summary>
        /// Override the ArrowProperty effect to add the acid effect to the player that was hit
        /// </summary>
        /// <param name="hitPlayer">This is the player ID of the player that was hit or 0 if no player was hit</param>
        public override void Effect(PlayerID hitPlayer)
        {
            // If a player was hit
            if (hitPlayer != 0)
            {
                // Add the attack component to the player that was hit and let the effect know who it came from
                AcidAttack a = Data.GameManager.instance.AllPlayers.Find(x => x.ID.Equals(hitPlayer)).gameObject.AddComponent<AcidAttack>();
                a.UpdatePlayerInfo(fromPlayer, hitPlayer);
            }
        }
    } 
}
