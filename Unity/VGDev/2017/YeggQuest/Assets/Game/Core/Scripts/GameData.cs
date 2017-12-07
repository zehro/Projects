using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest
{
    public static class GameData
    {
        private static HashSet<int> yeggs = new HashSet<int>();         // Which yeggs the player has
        public static bool[] unlockedPaint = new bool[4];               // Which primary paints the player has unlocked
        public static bool camHInvert = false;                          // If the camera is inverted horizontally
        public static bool camVInvert = false;                          // If the camera is inverted vertically

        // ======================================================================================================================== MESSAGES

        // Collects a yegg based on its unique global index.

        public static void CollectYegg(int index)
        {
            if (IsYeggCollected(index))
                Debug.LogError("There are multiple yeggs with the index " + index + "."
                             + "\nIndices need to be globally unique.");
            yeggs.Add(index);
        }

        // Loads the game data.

        public static void Load()
        {

        }

        // Saves the game data.

        public static void Save()
        {

        }

        // Deletes the game data.

        public static void Clean()
        {
            yeggs = new HashSet<int>();
            unlockedPaint = new bool[4];
        }

        // ======================================================================================================================== GETTERS

        // Gets if the player has collected a specific yegg.

        public static bool IsYeggCollected(int index)
        {
            return yeggs.Contains(index);
        }

        // Gets how many yeggs the player has collected.

        public static int GetYeggCount()
        {
            return yeggs.Count;
        }
    }
}