using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveShrine : MonoBehaviour {
    bool activated = false;
    public GameObject fire;
    public GameObject showText;
    GameManager manager;
	// Use this for initialization
	void Start () {
        activated = PlayerPrefs.GetInt("Checkpoint") == SceneManager.GetActiveScene().buildIndex;
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F3)) {
            PlayerPrefs.SetInt("Checkpoint", 0);
            print("Checkpoint reset");
        }
#endif
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && !activated) {
            save();
        }
    }

    void save() {
        PlayerPrefs.SetInt("Checkpoint", SceneManager.GetActiveScene().buildIndex);
        showText.SetActive(true);
        GameManager manager = FindObjectOfType<GameManager>();
        manager.lives += 3;
        fire.SetActive(true);
        activated = true;
    }
}
