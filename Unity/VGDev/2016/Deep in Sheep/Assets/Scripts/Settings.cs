using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

    public GameObject levelMusic;

    // Use this for initialization
    void Start () {
        levelMusic.GetComponent<AudioSource>().ignoreListenerVolume = true;
        if (MainMenu_Avery.getMusicSound())
        {
            levelMusic.GetComponent<AudioSource>().Play();
        }
        else
        {
            levelMusic.GetComponent<AudioSource>().Stop();
        }

        if (MainMenu_Avery.getFXSound() == false)
        {
            AudioListener.volume = 0;
        }
        else if (MainMenu_Avery.getFXSound() == true)
        {
            AudioListener.volume = 1;

        }
    }
	
}
