using UnityEngine;
using Assets.Scripts.Util;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Property to add to an arrow.
    /// </summary>
    public abstract class ArrowProperty : MonoBehaviour
    {
        /// Type of arrow this component is
        protected Enums.Arrows type;
        /// The collision info for the children to use
        protected CollisionInfo colInfo;
        /// The player who the ID belongs to and can hit
        protected PlayerID fromPlayer, hitPlayer;

        /// Runs at start
        public virtual void Init()
        {
            colInfo = GetComponent<CollisionInfo>();
        }
        /// Runs when arrow hits or passes through something as applicable
        public abstract void Effect(PlayerID hitPlayer);

        #region C# Properties
        /// <summary>
        /// Type of arrow this component is
        /// </summary>
        public Enums.Arrows Type
        {
            get { return type; }
            set { type = value; }
        }
        /// <summary>
        /// The ID of the player who shot the arrow
        /// </summary>
        public PlayerID FromPlayer
        {
            get { return fromPlayer; }
            set { fromPlayer = value; }
        }
        #endregion
    }
}
