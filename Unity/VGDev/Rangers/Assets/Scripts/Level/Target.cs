using UnityEngine;
using Assets.Scripts.Data;

namespace Assets.Scripts.Level
{
    public class Target : MonoBehaviour
    {
        /// <summary>
        /// Lets the target know that it was hit
        /// </summary>
        /// <param name="fromPlayer">The player that hit the target</param>
        public void TargetHit(PlayerID fromPlayer)
        {
            //GameManager.instance.TargetDestroyed(fromPlayer);
            TargetLevelManager.instance.TargetDestroyed(fromPlayer);
            Destroy(gameObject);
        }
    } 
}
