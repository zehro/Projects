using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using YeggQuest.NS_Spawner;

namespace YeggQuest
{
    // The GameCoordinator is the class responsible for the global state transitions: starting scenes,
    // respawning inside scenes, and changing between scenes. It keeps track of the "active spawner"
    // (which can be both checkpoints and entrances) as well as the two scenes the player has been
    // in most recently, so that the player always enters in the right place.

    public class GameCoordinator : MonoBehaviour
    {
        private static string CUR_SCENE = "";           // The name of the scene the player is currently in
        private static string LAST_SCENE = "";          // The name of the scene the player was in last
        private const string DEFAULT_SCENE = "Hub";     // The name of the scene transitioned to by default

        private Game game;                              // The game
        private Spawner activeSpawner;                  // The active spawner
        private bool isRunningRoutine;                  // Whether or not the coordinator is "busy" (refuses to accept transitions)
        private float spawnDelay = 1.25f;               // How long to wait before spawning the bird
        private float minSpawnDelay = 1.25f;            // The fastest the bird can spawn (from scene load time)

        void Awake()
        {
            CUR_SCENE = SceneManager.GetActiveScene().name;
            game = GetComponent<Game>();

            // TODO: Debug alerts

            if (FindObjectOfType<NS_Generic.RespawnPlane>() == null)
                Debug.LogWarning("This level doesn't have a respawn plane in it!");
            if (FindObjectOfType<SpawnerEntrance>() == null)
                Debug.LogWarning("This level doesn't have a SpawnerEntrance prefab in it!");
        }

        // ======================================================================================================================== MESSAGES
        
        internal void StartScene()
        {
            if (!isRunningRoutine)
                StartCoroutine(StartSceneRoutine());
        }

        public void RespawnInScene()
        {
            if (!isRunningRoutine && game.bird.animator.CanDespawn())
                StartCoroutine(RespawnInSceneRoutine());
        }

        public void GoToScene(string scene, bool shortened = false)
        {
            if (!isRunningRoutine && game.bird.animator.CanDespawn())
                StartCoroutine(GoToSceneRoutine(scene == "" ? DEFAULT_SCENE : scene, shortened));
        }

        // Sets the currently active spawner, returning if it was set successfully (different from the previous spawner).

        public bool SetActiveSpawner(Spawner spawner)
        {
            if (activeSpawner == spawner)
                return false;

            activeSpawner = spawner;
            return true;
        }

        // Delay the initial spawn for the current scene.

        public void SetSpawnDelay(float delay)
        {
            spawnDelay = Mathf.Max(minSpawnDelay, delay);
        }

        // ======================================================================================================================== GETTERS

        // Gets the currently active spawner.

        internal Spawner GetActiveSpawner()
        {
            return activeSpawner;
        }

        // Gets whether or not the game can be paused.

        internal bool CanPause()
        {
            return !isRunningRoutine && game.bird.animator.CanDespawn();
        }

        // ======================================================================================================================== HELPERS

        // A coroutine which starts up a new scene.

        private IEnumerator StartSceneRoutine()
        {
            isRunningRoutine = true;

            // When the scene begins, we need to find which spawner should be the active one.
            // This is based on which spawner connects to the scene we were just in.

            SpawnerEntrance[] entrances = FindObjectsOfType<SpawnerEntrance>();

            if (entrances.Length == 0)
            {
                yield return new WaitForSeconds(0.5f);
                game.ui.CircleWipeIn(0.75f);
                yield return new WaitForSeconds(0.75f);
                game.music.Play(0);

                isRunningRoutine = false;
                yield break;
            }

            foreach (SpawnerEntrance e in entrances)
            {
                if (e.levelInfo && e.levelInfo.gameObject.name == LAST_SCENE)
                {
                    activeSpawner = e.spawner;
                    break;
                }
            }

            // If none of the spawners seem to be related, just pick the first one.

            if (activeSpawner == null)
                activeSpawner = entrances[0].spawner;

            activeSpawner.SetCamera();

            // Now, actually start the level.
            
            yield return new WaitForSeconds(0.5f);
            game.ui.CircleWipeIn(0.75f);
            game.music.Play(2f);
            yield return new WaitForSeconds(spawnDelay - 0.5f);
            game.bird.Spawn();
            activeSpawner.Animate();

            isRunningRoutine = false;
        }

        // A coroutine which respawns the bird in the current scene.

        private IEnumerator RespawnInSceneRoutine()
        {
            isRunningRoutine = true;

            game.bird.Despawn(0.5f);
            game.ui.CircleWipeOut(0.5f);
            yield return new WaitForSeconds(0.5f);
            activeSpawner.SetCamera();
            yield return new WaitForSeconds(0.25f);
            game.ui.CircleWipeIn(0.5f);
            yield return new WaitForSeconds(0.5f);
            game.bird.Spawn();
            activeSpawner.Animate();

            isRunningRoutine = false;
        }

        // A coroutine which closes the current scene and enters a new one.
        // The "shortened" parameter plays a different version of this animation
        // which is used when the level is exited via the menu.

        private IEnumerator GoToSceneRoutine(string scene, bool shortened)
        {
            isRunningRoutine = true;

            if (!shortened)
            {
                game.music.FadeOut(2);
                game.bird.Despawn(0.25f);
                yield return new WaitForSeconds(1f);
                game.ui.CircleWipeOut(1f);
                yield return new WaitForSeconds(1f);
            }

            else
            {
                game.music.FadeOut(1);
                game.bird.Despawn(0.75f);
                game.ui.CircleWipeOut(0.75f);
                yield return new WaitForSeconds(1f);
            }
            
            LAST_SCENE = CUR_SCENE;
            CUR_SCENE = scene;
            SceneManager.LoadScene(scene);
        }
    }
}
