using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PauseMenu : MonoBehaviour {
    public Selectable firstItem;
    public Slider volSlider;

    public Text memOrbStatText;
    public Text defeatedEnemiesText;
    public GameObject manager;

    int volume;

    void OnEnable() {
        GameObject manager = GameObject.FindGameObjectWithTag("GameManager");
        defeatedEnemiesText.text = "Lives Currently Left: " + manager.GetComponent<GameManager>().lives.ToString();
        if (manager != null) {
            memOrbStatText.text = "Memory Orbs Collected: " + manager.GetComponent<GameManager>().getScore()/10;
        } else {
            memOrbStatText.text = "Memory Orbs Collected: " + 0;
        }
        firstItem.Select();
        volSlider.maxValue = 1.0f;
        volSlider.minValue = 0.0f;
        volSlider.value = AudioListener.volume;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetButtonDown("Pause")) && Time.timeScale == 0) {
            PlayerPrefs.SetFloat("Volume", volSlider.value);
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    public void resume() {
        PlayerPrefs.SetFloat("Volume", volSlider.value);
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void mainMenu() {
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().buildIndex == 23) {
            SceneManager.LoadScene(4);
        } else {
            SceneManager.LoadScene(1);
            Destroy(manager);
        }
    }

    public void restartLevel() {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void lowerVol(float factor) {
        if (AudioListener.volume > 0) {
            AudioListener.volume -= factor;
        }
        if (AudioListener.volume < 0) AudioListener.volume = 0;
        volSlider.value = AudioListener.volume;
    }

    public void raiseVol(float factor) {
        if (AudioListener.volume < 1) {
            AudioListener.volume += factor;
        }
        if (AudioListener.volume > 1) AudioListener.volume = 1;
        volSlider.value = AudioListener.volume;
    }

    public void volSliderChange() {
        AudioListener.volume = volSlider.value;
    }
}
