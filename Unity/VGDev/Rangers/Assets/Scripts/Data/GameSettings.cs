using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;

/// <summary>
/// Class for saving game settings
/// </summary>
[Serializable]
public class GameSettings
{
    /// <summary>
    /// Extension to the file where the previous game settings were saved
    /// </summary>
    public const string persistentExtension = "PrevSettings.dat";
    /// <summary>
    /// Constants for defaults and maxes
    /// </summary>
    public const float DEFAULT_TIME = 5f, DEFAULT_KILLS = 15f, DEFAULT_STOCK = 5f, MAX_TIME = 99f, MAX_KILLS = 99f, MAX_STOCK = 99f, MAX_ARROWS = 99f;
    // All modifiable fields
    private float timeLimit, killLimit, stockLimit, arrowLimit, damageModifier, gravityModifier, speedModifier, tokenSpawnFreq, playerSpawnFreq;
    // Non modifiable field
    private int targetsInLevel;
    // Whether or not time is enabled
    private bool timeLimitEnabled;
    // What type of game is running
    private Enums.GameType type;
	// What type of variant does this gametype have
    // Dictionary of enabled tokns and their frequencies that they spawn
    private Dictionary<Enums.Tokens, Enums.Frequency> enabledTokens;
    // The token effects that are initialized by default that can't be turned off
    private List<Enums.Tokens> defaultTokens;

    #region Constructors
    /// <summary>
    /// Default settings
    /// </summary>
    public GameSettings()
    {
        type = Enums.GameType.Stock;
        timeLimit = Mathf.Infinity;
        killLimit = Mathf.Infinity;
        stockLimit = DEFAULT_STOCK;
        arrowLimit = Mathf.Infinity;
        damageModifier = 1f;
        gravityModifier = 1f;
        speedModifier = 1f;
        tokenSpawnFreq = 5f;
        playerSpawnFreq = 3f;
        targetsInLevel = 0;
        timeLimitEnabled = false;
        enabledTokens = new Dictionary<Enums.Tokens, Enums.Frequency>();
        for(int i = 0; i < (int)Enums.Tokens.NumTypes; i++)
        {
            enabledTokens.Add((Enums.Tokens)i, Enums.Frequency.Infrequent);
        }
        defaultTokens = new List<Enums.Tokens>();
}
    #endregion
    /// <summary>
    /// Game is set to the kill game type
    /// </summary>
    public void UpdateKillSettings()
    {
        type = Enums.GameType.Deathmatch;
        killLimit = DEFAULT_KILLS;
        stockLimit = Mathf.Infinity;
    }

    /// <summary>
    /// Game is set to the stock game type
    /// </summary>
    public void UpdateStockSettings()
    {
        type = Enums.GameType.Stock;
        killLimit = Mathf.Infinity;
        stockLimit = DEFAULT_STOCK;
    }

    #region C# Properties
    /// <summary>
    /// Gets or sets the time limit. When setting the time limit to anything greater than zero,
	/// sets the timelimit enabled bool to true
    /// </summary>
    /// <value>The time limit.</value>
    public float TimeLimit
    {
        get { return timeLimit; }
        set {
			if(timeLimit > 0) {
				TimeLimitEnabled = true;
			} else {
				TimeLimitEnabled = false;
			}
			timeLimit = value;
		}
    }
    /// <summary>
    /// The number of kill one player can reach before the game ends
    /// </summary>
    public float KillLimit
    {
        get { return killLimit; }
        set {
			if(value == 0)
				killLimit = Mathf.Infinity;
			else
				killLimit = value;
		}
    }
    /// <summary>
    /// The amount of lives each player starts with
    /// </summary>
    public float StockLimit
    {
        get { return stockLimit; }
		set {
			if(value == 0)
				stockLimit = Mathf.Infinity;
			else
				stockLimit = value;
		}
    }
    /// <summary>
    /// The number of arrows each player starts with
    /// </summary>
    public float ArrowLimit
    {
        get { return arrowLimit; }
        set { arrowLimit = value; }
    }
    /// <summary>
    /// The multiplier for damage
    /// </summary>
    public float DamageModifier
    {
        get { return damageModifier; }
        set { damageModifier = value; }
    }
    /// <summary>
    /// The multiplier for gravity
    /// </summary>
    public float GravityModifier
    {
        get { return gravityModifier; }
        set { gravityModifier = value; }
    }
    /// <summary>
    /// The multiplier for speed
    /// </summary>
    public float SpeedModifier
    {
        get { return speedModifier; }
        set { speedModifier = value; }
    }
    /// <summary>
    /// The time it takes for a new token to spawn
    /// </summary>
    public float TokenSpawnFreq
    {
        get { return tokenSpawnFreq; }
        set { tokenSpawnFreq = value; }
    }
    /// <summary>
    /// The time it takes for a player to respawn after death
    /// </summary>
    public float PlayerSpawnFreq
    {
        get { return playerSpawnFreq; }
        set { playerSpawnFreq = value; }
    }
    /// <summary>
    /// The number of targets in a level for target practice
    /// </summary>
    public int TargetsInLevel
    {
        get { return targetsInLevel; }
        set { targetsInLevel = value; }
    }
    /// <summary>
    /// Whether or not there is a time limit on the match
    /// </summary>
    public bool TimeLimitEnabled
    {
        get { return timeLimitEnabled; }
        set { timeLimitEnabled = value; }
    }
    /// <summary>
    /// The current game type
    /// </summary>
    public Enums.GameType Type
    {
        get { return type; }
		set { type = value; }
    }

    /// <summary>
    /// The dictionary of active tokens and their spawn frequencies
    /// </summary>
    public Dictionary<Enums.Tokens, Enums.Frequency> EnabledTokens
    {
        get { return enabledTokens; }
        set { enabledTokens = value; }
    }
    /// <summary>
    /// The default effects that are on for players by default
    /// </summary>
    public List<Enums.Tokens> DefaultTokens
    {
        get { return defaultTokens; }
        set { defaultTokens = value; }
    }
    #endregion
}
