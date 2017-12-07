using UnityEngine;
using Assets.Scripts.Attacks;
using Assets.Scripts.Data;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Arrow property that will spawn an object
    /// </summary>
    public abstract class SpawnerProperty : ArrowProperty
    {
        /// <summary>
        /// Effect that will be spawned on impact
        /// </summary>
        protected GameObject spawnEffect;
		/// <summary>
		/// The spawned reference to update if need be
		/// </summary>
		protected GameObject spawnedReference;

        public override void Init()
        {
            base.Init();
            // Get the effect base on the type of the arrow
            spawnEffect = AttackManager.instance.GetEffect(type);
        }

        public override void Effect(PlayerID hitPlayer)
        {
            // If the prefab is not null
            if (spawnEffect != null)
            {
                GameObject g = (GameObject)Instantiate(spawnEffect, colInfo.HitPosition, colInfo.HitRotation);
				spawnedReference = g;
            }
            else Debug.Log("Arrow of type: " + type.ToString() + " could not load an effect");
        }
    }
}
