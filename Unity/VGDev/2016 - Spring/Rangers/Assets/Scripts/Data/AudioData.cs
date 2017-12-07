using System;

/// <summary>
/// Class for saving audio settings
/// </summary>
[Serializable]
public class AudioData
{
    // Volume variables
    private float sfxVol;
    private float musicVol;
    private float voiceVol;

    #region Constructors
    /// <summary>
    /// Default new AudioData settings
    /// </summary>
    public AudioData()
    {
        sfxVol = 1f;
        musicVol = 1f;
        voiceVol = 1f;
    }

    /// <summary>
    /// Manually initialize the settings
    /// </summary>
    /// <param name="sfxVol">Volume of the sound effects</param>
    /// <param name="musicVol">Volume of the music</param>
    /// <param name="voiceVol">Volume fo the voices</param>
    public AudioData(float sfxVol, float musicVol, float voiceVol)
    {
        this.sfxVol = sfxVol;
        this.musicVol = musicVol;
        this.voiceVol = voiceVol;
    }
    #endregion

    #region C# Properties
    /// <summary>
    /// Volume of the sound effects
    /// </summary>
    public float SFXVol
    {
        get { return sfxVol; }
        set { sfxVol = value; }
    }
    /// <summary>
    /// Volume of the music
    /// </summary>
    public float MusicVol
    {
        get { return musicVol; }
        set { musicVol = value; }
    }
    /// <summary>
    /// Volume fo the voices
    /// </summary>
    public float VoiceVol
    {
        get { return voiceVol; }
        set { voiceVol = value; }
    }
    #endregion
}
