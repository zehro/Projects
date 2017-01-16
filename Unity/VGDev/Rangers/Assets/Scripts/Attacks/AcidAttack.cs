using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Timers;

namespace Assets.Scripts.Attacks
{
    /// <summary>
    /// Attack that will deal damage over time to the player
    /// </summary>
    public class AcidAttack : SpawnAttack
    {
        // The timer to repeatedly damage the player
        private RepetitionTimer t;
        // Amount of damage to deal to the player each iteration
        new protected float damage = 5;
        // Amount of time to pass before repeating the damage
        private float damageInterval = 1f;
        // Number of times to repeat the damage
        private int numHits = 4;
        // Caching the controller of the hit player
        private Controller controller;

        void Start()
		{
			controller = GetComponent<Controller>();
            // Get all AcidAttacks already on the player (this component should have at least been added)
            AcidAttack[] currentAttacks = gameObject.GetComponents<AcidAttack>();
            // If this component is not the only AcidAttack on the player
            if(currentAttacks.Length > 1)
            {
                for(int i  = 0; i < currentAttacks.Length; i++)
                {
                    // Reset the timer if it is the original timer and not this timer
                    if(currentAttacks[i] != this)
					{
						if(currentAttacks[i].Timer != null)
						{
							currentAttacks[i].Timer.Reset();
						}
						else
						{
							InitializeTimer();
						}
					}
                }
                // Destroy this AcidAttack because it is not the original
                Destroy(this);
            }
            // This is the only AcidAttack on the player
            else
            {
                // Initialize the acid attack
				InitializeTimer();
            }
        }

		/// <summary>
		/// Initializes the acid timer.
		/// </summary>
		private void InitializeTimer() {
			t = gameObject.AddComponent<RepetitionTimer>();
			t.Initialize(damageInterval, "Acid Attack", numHits);
			t.TimeOut += new RepetitionTimer.TimerEvent(DamagePlayer);
			t.FinalTick += FinalHit;
		}

        // Target for the repeating timer
        private void DamagePlayer(RepetitionTimer t)
        {
            controller.LifeComponent.ModifyHealth(-damage, fromPlayer);
        }

        // Target for the timer's final timeout
        private void FinalHit(RepetitionTimer t)
        {
            Destroy(this);
        }

        #region C# Properties
        /// <summary>
        /// The timer that is running for this attack
        /// </summary>
        public RepetitionTimer Timer
        {
            get { return t; }
        }
        #endregion
    }
}