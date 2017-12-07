using Assets.Scripts.Tokens;
using Assets.Scripts.Timers;
using Assets.Scripts.Data;
using Assets.Scripts.Attacks;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Handles all the components related to in game Life
    /// such as lives, health, respawning, etc
    /// </summary>
    public class Life : ControllerObject
	{
        /// <summary>
        /// The max health a player can have
        /// </summary>
        public const float MAX_HEALTH = 100f;

        private float health = MAX_HEALTH, lives = Mathf.Infinity, deaths = 0;

        // CHANGE FOR LATER
        public int kills;

        // Last player to hit this player
        private PlayerID lastAttacker = PlayerID.None;

        /// <summary>
        /// Modifies a player's heath
        /// </summary>
        /// <param name="delta">The amound to change (should be negative for damage)</param>
        /// <param name="id">The player who dealt the damage</param>
        public void ModifyHealth(float delta, PlayerID id = PlayerID.None)
        {
			
			for(int i = 0; i < controller.playerMats.Count; i++) {
				controller.playerMats[i].SetColor("_EmissionColor", Color.red);
			}
			controller.justDamaged = true;

			if(id != PlayerID.None) {
				lastAttacker = id;
				StatisticManager.instance.statistics[id].arrowsHit++;
			}
            if (delta < 0)
                Camera.main.GetComponent<PerlinShake>().PlayShake();
            if (health > 0)
            {
                health = Mathf.Clamp((health + delta), 0, MAX_HEALTH);
				if (health <= 0) Die();
            }
        }

        // Handles when players die
        private void Die()
		{
			SFXManager.instance.PlayDeath();
            controller.Disable();
            if (--lives > 0)
            {
                // Tell GameManager to setup respawn
                GameManager.instance.Respawn(controller.ID);
				deaths++;
            }
            GameManager.instance.PlayerKilled(controller.ID, lastAttacker);
        }

        /// <summary>
        /// Respawns the player and clears all previous effects
        /// </summary>
        public void Respawn()
        {
            SpawnAttack[] currentAttacks = GetComponents<SpawnAttack>();
            for(int i = 0; i < currentAttacks.Length; i++)
            {
                Destroy(currentAttacks[i]);
            }

            Timer[] timers = GetComponents<Timer>();
            for(int i = 0; i < timers.Length; i++)
            {
                if (timers[i].ID.EndsWith("Attack")) Destroy(timers[i]);
            }

            controller.ArcheryComponent.ClearAllTokens();

            health = MAX_HEALTH;
            controller.Enable();
        }

        /// <summary>
        /// Overriding the collect token method from player controller object
        /// </summary>
        /// <param name="token">The token that was collected</param>
        public override void CollectToken(Token token)
        {
            // Handle what type of token was collected
            if (token.GetType().Equals(typeof(HealthToken)))
            {
                // Add health back to the player
               ModifyHealth(((HealthToken)token).Health);
            }
        }

        #region C# Properties
        /// <summary>
        /// Health of the player
        /// </summary>
        public float Health
        {
            get { return health; }
            set { health = value; }
        }

		/// <summary>
		/// Health divided by max health of the player
		/// </summary>
		public float HealthPercentage
		{
			get { return health/MAX_HEALTH; }
		}

        /// <summary>
        /// Number of lives the player has
        /// </summary>
        public float Lives
        {
            get { return lives; }
            set { lives = value; }
        }

		public float Deaths
		{
			get { return deaths; }
		}
        #endregion
    }
}
