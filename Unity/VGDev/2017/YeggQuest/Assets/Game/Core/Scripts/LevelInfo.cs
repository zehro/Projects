using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest
{
    public class LevelInfo : MonoBehaviour
    {
        public string title;
        public Sprite image;
        public YeggInfo[] yeggs;
    }

    [System.Serializable]
    public class YeggInfo
    {
        public int index;
        public string description;
    }
}