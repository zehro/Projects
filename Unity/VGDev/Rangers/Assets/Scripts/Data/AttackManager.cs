using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Manager that deals with the attacks and their effects
    /// </summary>
    public class AttackManager : MonoBehaviour
    {
        /// Use a singleton instance to make sure there is only one
        public static AttackManager instance;

        /// List of possible attack effect prefabs
        [SerializeField]
        private List<GameObject> effectPrefabs;

        // Sets up singleton instance. Will remain if one does not already exist in scene
        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            if(effectPrefabs == null) effectPrefabs = new List<GameObject>();
        }

        /// <summary>
        /// Gets the prefab based on the arrow type.
        /// </summary>
        /// <param name="type">The type of arrow that needs a prefab.</param>
        /// <returns>The appropriate effect for the arrow property.</returns>
        public GameObject GetEffect(Enums.Arrows type)
        {
            return effectPrefabs.Find(x => x.name.StartsWith(type.ToString()));
        }
    } 
}
