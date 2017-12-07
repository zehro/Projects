using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.Util
{
    /// <summary>
    /// Destroys arrows that enter it so that they do not fly offscreen forever
    /// </summary>
    public class ArrowDestroyer : MonoBehaviour
    {
        void OnTriggerEnter(Collider col)
        {
            if (col.transform.tag.Equals("Arrow")) Destroy(col.gameObject);
			if (col.transform.root.tag.Equals("Player")) col.transform.root.GetComponent<Controller>().LifeComponent.ModifyHealth(-100, col.transform.root.GetComponent<Controller>().ID);
        }
    }
}