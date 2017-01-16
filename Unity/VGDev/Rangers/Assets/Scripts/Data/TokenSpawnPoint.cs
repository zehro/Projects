using UnityEngine;
using Assets.Scripts.Timers;
using Assets.Scripts.Tokens;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Loacation where a token will spawn
    /// </summary>
    public class TokenSpawnPoint : MonoBehaviour
    {
        // The token of the spawn point
        private Token spawnItem;

        /// <summary>
        /// Initializes the spawn point
        /// </summary>
        public void Init()
        {
            if (TokenSpawner.instance.Tokens.Count > 0)
            {
                // Add a repeating timer to keep spawning tokens
                RepetitionTimer t = gameObject.AddComponent<RepetitionTimer>();
                t.Initialize(10f, "Token Spawn Timer");
                t.TimeOut += new RepetitionTimer.TimerEvent(SpawnTokenHelper);
            }
        }

        /// <summary>
        /// Spawns a token when timer times out
        /// </summary>
        /// <param name="t">Timer that is spawning tokens</param>
        private void SpawnTokenHelper(RepetitionTimer t)
        {
            if (!HasToken()) SpawnToken(TokenSpawner.instance.GetToken());
            else spawnItem.gameObject.SetActive(false);
            
        }

        /// <summary>
        /// Determines if the spawner already contains a token
        /// </summary>
        /// <returns>Whether or not a token exists</returns>
        private bool HasToken()
        {
            return (spawnItem != null && spawnItem.gameObject.activeSelf);
        }

        /// <summary>
        /// Spawns a token
        /// </summary>
        /// <param name="tokenPrefab">The token to be spawned</param>
        private  void SpawnToken(GameObject tokenPrefab)
        {
			if(tokenPrefab != null) {
	            tokenPrefab.transform.position = transform.position;
	            // Turn on token since pooling
	            tokenPrefab.SetActive(true);
	            spawnItem = tokenPrefab.GetComponent<Token>();
			}
        }
    }
}
