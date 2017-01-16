using System;

/// <summary>
/// Class for saving video settings
/// </summary>
[Serializable]
public class VideoData
{
    private int resolutionIndex;
    private int qualityIndex;
    private bool fullScreen;

    #region Constructors
    /// <summary>
    /// Default settings
    /// </summary>
    public VideoData()
    {
        resolutionIndex = 0;
        qualityIndex = 0;
        fullScreen = false;
    }

    /// <summary>
    /// Manually initialize the settings
    /// </summary>
    /// <param name="resolutionIndex">The index to the current resolution</param>
    /// <param name="qualityIndex">The index to the current quality</param>
    /// <param name="fullScreen">Whether or not the game is fullscreen</param>
    public VideoData(int resolutionIndex, int qualityIndex, bool fullScreen)
    {
        this.resolutionIndex = resolutionIndex;
        this.qualityIndex = qualityIndex;
        this.fullScreen = fullScreen;
    }
    #endregion

    #region C# Properties
    /// <summary>
    /// The index to the current resolution
    /// </summary>
    public int ResolutionIndex
    {
        get { return resolutionIndex; }
        set { resolutionIndex = value; }
    }
    /// <summary>
    /// The index to the current quality
    /// </summary>
    public int QualityIndex
    {
        get { return qualityIndex; }
        set { qualityIndex = value; }
    }
    /// <summary>
    /// Whether or not the game is fullscreen
    /// </summary>
    public bool Fullscreen
    {
        get { return fullScreen; }
        set { fullScreen = value; }
    }
    #endregion
}
