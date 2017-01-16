using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Util;

/// <summary>
/// Class for saving game settings
/// </summary>
[Serializable]
public class TargetSettings
{
    // Non modifiable field
    private int targetsInLevel;
    /// <summary>
    /// Platinum target time
    /// </summary>
    private float platinumTime;
    /// <summary>
    /// Gold target time
    /// </summary>
    private float goldTime;
    /// <summary>
    /// Silver target time
    /// </summary>
    private float silverTime;
    /// <summary>
    /// Bronze target time
    /// </summary>
    private float bronzeTime;

    #region Constructors
    public TargetSettings()
    {
        targetsInLevel = 0;
        platinumTime = 0;
        goldTime = 0;
        bronzeTime = 0;
    }
    #endregion

    #region C# Properties
    /// <summary>
    /// The number of targets in a level for target practice
    /// </summary>
    public int TargetsInLevel
    {
        get { return targetsInLevel; }
        set { targetsInLevel = value; }
    }

    /// <summary>
    /// The time to beat to get platinum
    /// </summary>
    public float PlatinumTime
    {
        get { return platinumTime; }
        set { platinumTime = value; }
    }
    /// <summary>
    /// The time to beat to get gold
    /// </summary>
    public float GoldTime
    {
        get { return goldTime; }
        set { goldTime = value; }
    }
    /// <summary>
    /// The time to beat to get silver
    /// </summary>
    public float SilverTime
    {
        get { return silverTime; }
        set { silverTime = value; }
    }
    /// <summary>
    /// The time to beat to get bronze
    /// </summary>
    public float BronzeTime
    {
        get { return bronzeTime; }
        set { bronzeTime = value; }
    }
    #endregion
}
