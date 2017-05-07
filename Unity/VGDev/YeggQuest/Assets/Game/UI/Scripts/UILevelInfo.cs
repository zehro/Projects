using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YeggQuest.NS_UI
{
    public class UILevelInfo : MonoBehaviour
    {
        public Text uiTitle;                    // the title of the level
        public Image[] uiYeggs;                 // the yeggs in the level
        public Text[] uiYeggDescriptions;       // the descriptions of the yeggs in the level
        public Sprite[] uiYeggSprites;          // the sprites for the yeggs

        private LevelInfo info;

        void Awake()
        {
            info = FindObjectOfType<Game>().levelInfo;

            if (!info)
                gameObject.SetActive(false);
            else
            {
                uiTitle.text = info.title;
                for (int i = 0; i < 3; i++)
                {
                    if (i < info.yeggs.Length)
                        uiYeggDescriptions[i].text = info.yeggs[i].description;
                    else
                        uiYeggs[i].transform.parent.gameObject.SetActive(false);
                }
            }
        }

        void Update()
        {
            for (int i = 0; i < info.yeggs.Length; i++)
                uiYeggs[i].sprite = uiYeggSprites[GameData.IsYeggCollected(info.yeggs[i].index) ? 1 : 0];
        }
    }
}