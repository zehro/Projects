using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Cam;
using YeggQuest.NS_Logic;

namespace YeggQuest
{
    public class Hub : MonoBehaviour
    {
        public HubUnlockChain[] unlockChains;       // Chains of yegg demands to unlock for progression, in order [color 0-3][index]
        public CamCutscene[] unlockCutscenes;       // The cutscenes to play for each of the 4 color unlocks

        private GameCoordinator coordinator;
        private Cam cam;

        // The hub is pretty much the only scene in the game that needs to remember some form
        // of global state, because it's responsible for playing cutscenes and also visually
        // unlocking the circle of paint fountains in the courtyard / the big factory door.
        // To do this, it runs some cutscene and game data stuff tightly coupled with the
        // GameCoordinator and the camera.
        
        void Awake()
        {
            // TODO: REMOVE

            int cheat = 0;

            for (int i = 0; i < cheat; i++)
                GameData.CollectYegg(i);
            if (cheat >= 1)
                GameData.unlockedPaint[0] = true;
            if (cheat >= 5)
                GameData.unlockedPaint[1] = true;
            if (cheat >= 10)
                GameData.unlockedPaint[2] = true;
            if (cheat >= 20)
                GameData.unlockedPaint[3] = true;

            // Get the game coordinator and camera

            coordinator = FindObjectOfType<GameCoordinator>();
            cam = FindObjectOfType<Cam>();

            // First off, we need to immediately activate the yegg demands that were once activated
            // on a prior visit to the hub. This ensures that when the scene starts, the paint fountains
            // and the big factory door are in their proper state depending on the game progression.

            for (int i = 0; i < 4; i++)
            {
                if (GameData.unlockedPaint[i])
                    foreach (LogicalYeggDemand d in unlockChains[i].demands)
                        d.Check();
            }

            // Now, we need to check if the player has unlocked a new part of the progression. If (for example)
            // they have 5 yeggs and didn't before, we "unlock" the yellow paint, playing its cutscene when
            // they enter and turning on the demands in a coroutine.

            int yeggs = GameData.GetYeggCount();

            for (int i = 0; i < 4; i++)
            {
                if (yeggs >= unlockChains[i].demands[0].requiredAmount && !GameData.unlockedPaint[i])
                {
                    GameData.unlockedPaint[i] = true;

                    StartCoroutine(UnlockRoutine(unlockChains[i]));

                    coordinator.SetSpawnDelay(unlockCutscenes[i].GetLength());
                    cam.PlayCutscene(unlockCutscenes[i]);

                    // TODO: save game here
                }
            }
        }

        private IEnumerator UnlockRoutine(HubUnlockChain chain)
        {
            for (int i = 0; i < chain.demands.Length; i++)
            {
                yield return new WaitForSeconds(chain.delays[i]);
                chain.demands[i].Check();
            }
        }
    }

    [System.Serializable]
    public class HubUnlockChain
    {
        public LogicalYeggDemand[] demands;
        public float[] delays;
    }
}