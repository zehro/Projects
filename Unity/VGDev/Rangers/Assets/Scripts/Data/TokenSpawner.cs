using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Manager that spawns tokens in the level
    /// </summary>
    public class TokenSpawner : MonoBehaviour
    {
        /// <summary>
        /// Use singleton instance so there is only one
        /// </summary>
        public static TokenSpawner instance;
        // List of all possible token prefabs
        [SerializeField]
        private List<GameObject> tokens;

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
        }

        /// <summary>
        /// Initialize the manager to start spawning tokens
        /// </summary>
        /// <param name="tokenDict">The tokens to spawn and their frequencies</param>
        public void Init(Dictionary<Enums.Tokens, Enums.Frequency> tokenDict)
        {
            // Get all spawn points
            TokenSpawnPoint[] spawnPoints = FindObjectsOfType<TokenSpawnPoint>();
            if (spawnPoints.Length == 0) return;

            // Transfer the tokens from the dict to the list
            tokens = new List<GameObject>();
            foreach(Enums.Tokens key in tokenDict.Keys)
            {
                GameObject go = GameManager.instance.AllTokens.Find(x => x.name.StartsWith(key.ToString()));
                if (go != null)
                {
                    // Spawn a certain number of the same token based on its frequency
                    Enums.Frequency freq = tokenDict[key];
                    for (int i = 0; i < (int)freq; i++)
                    {
                        GameObject spawnToken;
                        spawnToken = Instantiate(go);
                        tokens.Add(spawnToken);
                        spawnToken.SetActive(false);
                    }
                }
                //else Debug.Log("Key: " + key.ToString() + " is null");
            }
            // Initialize all the spawn points
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                spawnPoints[i].Init();
            }
        }

        /// <summary>
        /// Gets a random token
        /// </summary>
        /// <returns>A token prefab</returns>
        public GameObject GetToken()
        {
			if(GameManager.instance.IsPaused) return null;
            // Perform Fisher-Yates shuffling algorithm.
            // This is because we are pooling objects so getting a random token
            // means that if we pick a random token that is enabled, we should
            // increment to the first, disabled token but that increases the odds
            // of later gameobjects instead of being truly random.
            GameObject temp;
            for (int i = 0; i < tokens.Count; i++)
            {
                int r = i + (int)(Random.Range(0f, 1f) * (tokens.Count - i));
                temp = tokens[r];
                tokens[r] = tokens[i];
                tokens[i] = temp;
            }
            // Find the first inactive token
            temp = tokens.Find(x => !x.gameObject.activeSelf);
            return temp;
        }

        #region C# Properties
        /// <summary>
        /// List of all the tokens
        /// </summary>
        public List<GameObject> Tokens
        {
            get { return tokens; }
        }
        #endregion
    }
}
