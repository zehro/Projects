using Assets.Scripts.Level;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Data
{
    public class TargetLevelManager : LevelManager
    {
        new public static TargetLevelManager instance;
        private TargetSettings settings;

        private float timer = 0;
        public Text time;
        public Image medal;

        public Color platinum, gold, silver, bronze, fail;

        new public void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            settings = LoadManager.LoadTargetSettingsXML(SceneManager.GetActiveScene().name);
            settings.TargetsInLevel = FindObjectsOfType<Target>().Length;
            running = true;
            medal.color = platinum;
        }

        void Update()
        {
            if (running)
            {
                timer += Time.deltaTime;
                int minutes = (int)timer / 60;
                int seconds = (int)timer % 60;
                int fraction = (int)(timer * 1000);
                fraction = fraction % 1000;
                time.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
                if (timer >= settings.BronzeTime) medal.color = fail;
                else if (timer >= settings.SilverTime) medal.color = bronze;
                else if (timer >= settings.GoldTime) medal.color = silver;
                else if (timer >= settings.PlatinumTime) medal.color = gold;
            }
        }

        public override void InitializeMatch()
        {

        }

        protected override void GameOver()
        {
            running = false;
        }

        /// <summary>
        /// Handles a target being destroyed and checks to see if the game is over
        /// </summary>
        /// <param name="fromPlayer">The player that hit the target</param>
        public void TargetDestroyed(PlayerID fromPlayer)
        {
            if (--settings.TargetsInLevel <= 0) GameOver();
        }
    }
}
