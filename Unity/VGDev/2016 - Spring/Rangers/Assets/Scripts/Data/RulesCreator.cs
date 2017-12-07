using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Creates a set of game rules based on the fields given
    /// </summary>
    public class RulesCreator : MonoBehaviour
    {
        /// <summary>
        /// Name of the file to save the settings to
        /// </summary>
        public string fileName;

        /// <summary>
        /// Type of the match
        /// </summary>
        [Space]
        public Enums.GameType type;

        /// <summary>
        /// Whether or not a time limit is enabled on the match
        /// </summary>
        [Space]
        public bool timeLimitEnabled;
        /// <summary>
        /// Time limit for the match
        /// </summary>
        [Range(0, GameSettings.MAX_TIME)]
        public int timeLimit = (int)GameSettings.DEFAULT_TIME;

        /// <summary>
        /// Number of kills before a player wins
        /// </summary>
        [Space]
        [Range(0, GameSettings.MAX_KILLS)]
        public int killLimit = (int)GameSettings.DEFAULT_KILLS;
        /// <summary>
        /// Number of lives each player gets
        /// </summary>
        [Range(0, GameSettings.MAX_STOCK)]
        public int stockLimit = (int)GameSettings.DEFAULT_STOCK;

        /// <summary>
        /// Number of arrows each player starts with
        /// </summary>
        [Space]
        [Range(0, GameSettings.MAX_TIME)]
        public int arrowLimit = 0;

        /// <summary>
        /// Multipliers for damage, gravity, and speed
        /// </summary>
        [Space]
        [Range(0.1f, 10f)]
        public float damageModifier = 1f, gravityModifier = 1f, speedModifier = 1f;

        /// <summary>
        /// Time before the tokens respawn
        /// </summary>
        [Space]
        [Range(0.1f, 180f)]
        public float tokenSpawnFreq = 3f;
        /// <summary>
        /// Time before the player respawns
        /// </summary>
        [Range(0.1f, 10f)]
        public float playerSpawnFreq = 5f;

        /// <summary>
        /// List of tokens to be in the match
        /// </summary>
        [Space]
        public List<Enums.Tokens> tokens;
        /// <summary>
        /// List of frequencies to go along with the tokens
        /// </summary>
        public List<Enums.Frequency> freqs;
        /// <summary>
        /// Dictionary of the token list and frequency list
        /// </summary>
        private Dictionary<Enums.Tokens, Enums.Frequency> enabledTokens;

        /// <summary>
        /// Default effects applied to players from the beginning
        /// </summary>
        [Space]
        public List<Enums.Tokens> defaultTokens;

        void Start()
        {
            if (fileName == "") return;
            GameSettings g = new GameSettings();

            g.Type = type;

            g.TimeLimitEnabled = timeLimitEnabled;
            g.TimeLimit = (timeLimit == 0 ? Mathf.Infinity : timeLimit);
            g.KillLimit = (killLimit == 0 ? Mathf.Infinity : killLimit);
            g.StockLimit = (stockLimit == 0 ? Mathf.Infinity : stockLimit);
            g.ArrowLimit = (arrowLimit == 0 ? Mathf.Infinity : arrowLimit);
            g.DamageModifier = damageModifier;
            g.GravityModifier = gravityModifier;
            g.SpeedModifier = speedModifier;
            g.TokenSpawnFreq = tokenSpawnFreq;
            g.PlayerSpawnFreq = playerSpawnFreq;

            enabledTokens = new Dictionary<Enums.Tokens, Enums.Frequency>();
            for(int i = 0; i < tokens.Count; i++)
            {
                enabledTokens.Add(tokens[i], freqs[i]);
            }
            g.EnabledTokens = enabledTokens;
            g.DefaultTokens = defaultTokens;

            // Save the settings
            SaveManager.SaveGameSettings(g, fileName + ".dat");
        }
    }
}
