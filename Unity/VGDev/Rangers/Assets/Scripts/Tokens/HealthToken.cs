using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Util;

namespace Assets.Scripts.Tokens
{
    /// <summary>
    /// Functions like a health pickup. Adds health to character who collides with it
    /// </summary>
    public class HealthToken : Token
    {
        [SerializeField]
        private float health;

        /// <summary>
        /// Override the TokenCollected method and tell the Life component to collect the token
        /// </summary>
        /// <param name="controller">Controller that is collecting the token</param>
        public override void TokenCollected(Controller controller)
        {
            controller.LifeComponent.CollectToken(this);
            // Set inactive since we are pooling
            gameObject.SetActive(false);
        }

        #region C# Properties
        /// <summary>
        /// Amount of health to add to the player
        /// </summary>
        public float Health
        {
            get { return health; }
        }
        #endregion
    }
}
