using UnityEngine;

namespace Assets.Scripts.Attacks
{
    /// <summary>
    /// Base attack class that is spawned in (or added to player as component)
    /// </summary>
    public class SpawnAttack : MonoBehaviour
    {
        // Base damage the attack does
        protected float damage = 10f;
        // Player ID info of who initiated the attack and who it hit
        protected PlayerID fromPlayer, hitPlayer;

        /// <summary>
        /// Update the player info about who shot the arrow that created the attack and who it hit if applicable.
        /// </summary>
        /// <param name="fromPlayer">Player that shot the arrow and created the attack.</param>
        /// <param name="hitPlayer">Player that was hit</param>
        public void UpdatePlayerInfo(PlayerID fromPlayer, PlayerID hitPlayer)
        {
            this.fromPlayer = fromPlayer;
            this.hitPlayer = hitPlayer;
        }

        #region C# Properties
        /// <summary>
        /// Base damage an attack will do
        /// </summary>
        public float Damage
        {
            get { return damage; }
        }
        /// <summary>
        /// Player that shot the arrow
        /// </summary>
        public PlayerID FromPlayer
        {
            get { return fromPlayer; }
        }
        /// <summary>
        /// Player hit by the arrow
        /// </summary>
        public PlayerID HitPlayer
        {
            get { return hitPlayer; }
        }
        #endregion
    }
}