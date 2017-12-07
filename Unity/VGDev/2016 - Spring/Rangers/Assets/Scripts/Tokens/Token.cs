using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.Tokens
{
    /// <summary>
    /// Tokens will be collected by players to add effects. Effects do not stack
    /// </summary>
    public abstract class Token : MonoBehaviour
    {
        // Tokens will be collected via trigger
        //void OnTriggerEnter(Collider col)
        //{
		//	TokenCollected(col.transform.root.GetComponent<Controller>());
        //}

        /// <summary>
        /// Token will use the Controller to get the appropriate component
        /// </summary>
        /// <param name="controller">The controller that is collecting the token</param>
        public abstract void TokenCollected(Controller controller);
	}
}
