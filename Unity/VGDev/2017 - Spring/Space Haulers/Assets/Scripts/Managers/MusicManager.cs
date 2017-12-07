using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    // Adds music files and chooses the track to play
    public AudioClip[] music;                 
    public AudioSource musicSource;
    public int track = 0;
    public bool loopSingle;
    public bool firstElementIntro;
    public Slider slider;

    private Settings settings;
    void Start()
    {
        float vol = PlayerPrefs.HasKey("Volume") ? PlayerPrefs.GetFloat("Volume") : .5f;
        settings = new global::Settings(vol, 0f, false, null);
        if (slider)
            slider.value = vol;
            
        if (music.Length > 0)
        {
            track = track % music.Length;
            musicSource.clip = music[track];
            musicSource.Play();
        }
    }

    void Update()
    {
        if (!musicSource.isPlaying && music.Length > 0)
        {
            if (!loopSingle)
            {
                // Clamp the tracks to everything but the first when it's an intro
                track = firstElementIntro
                    ? Mathf.Clamp((track + 1) % music.Length, 1, music.Length - 1)
                    : (track + 1) % music.Length;
            }
            musicSource.clip = music[track];
            musicSource.Play();
        }
        musicSource.volume = settings.volume;
    }

    public void setVolume()
    {
        settings.volume = slider.value;
        PlayerPrefs.SetFloat("Volume", slider.value);
    }
}
