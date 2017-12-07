using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public bool orig = false;
    int score = 0;
    int prevScore = 0;
    public int lives = 0;
    int multiplier = 1;
    string curLevel = "";
    Text scoreText;

    void Awake() {
        GameObject[] managers = GameObject.FindGameObjectsWithTag("GameManager");
        if (managers.Length != 1) {
            foreach (GameObject manager in managers) {
                if (!manager.GetComponent<GameManager>().orig) Destroy(manager);
            }
        }
    }
	// Use this for initialization
	void Start () {
        curLevel = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(gameObject);
        scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>();
        SceneManager.sceneLoaded += resetScene;
        orig = true;
        
	}

    void Update() {
        scoreText.text = "Score: " + score + "\nLives: " + lives;
        scoreText.verticalOverflow = VerticalWrapMode.Overflow;
    }
	
	// Update is called once per frame
	public int getScore() {
        return score;
    }

    public void addScore(int enemyVal) {
        score += multiplier * enemyVal;
    }

    public void resetScore() {
        score = 0;
    }

    public void resetScene(Scene scene, LoadSceneMode mode) {
        scoreText = GameObject.FindGameObjectWithTag("scoreText").GetComponent<Text>();
        if (scene.name == curLevel || SceneManager.GetActiveScene().name == "") {
            score = prevScore;
            curLevel = scene.name;
        } else {
            prevScore = score;
        }

    }

    public void addMult() {
        if (multiplier < 3) {
            multiplier += 1;
        }
    }

    public void resetMult() {
        multiplier = 1;
    }


    //Can specify a time for a restart
    bool resetting = false;
    public IEnumerator deathRestart(float waitTime = 3.0f) {
        if (resetting == false) {
            resetting = true;
            yield return new WaitForSeconds(waitTime);
            loseLives();
            if (lives <= 0) {
                SceneManager.LoadScene(PlayerPrefs.GetInt("Checkpoint"));
                lives = 3;
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            resetting = false;
        }
    }

    public void loseLives() {
        if (SceneManager.GetActiveScene().name != "EnemyTestingArena") {
            lives -= 1;
        }
    }
}
