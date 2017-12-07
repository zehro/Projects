using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public static LevelManager instance;
    public Text Restart, Dialogue;
    public GameObject GameOver, Survived, loadingScreen, DialogueManager;

    GameObject Player;
    Scene menu;
    bool gameover;

    void Start () {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    public void restart()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void backToMenu()
    {
        loadingScreen.SetActive(true);
        SceneManager.LoadScene("MainMenu");
    }

    public void gameOver()
    {
        shutdown();
        StartCoroutine(gameOverGUI(2f));
    }

    public void survived() {
        shutdown();
        StartCoroutine(survivedGUI(2f));
    }

    public void shutdownDialogue() {
        Dialogue.gameObject.SetActive(false);
        DialogueManager.SetActive(false);
    }

    private void shutdown() {
        if (gameover)
            return;
        Player.GetComponent<TruckController>().shutdown();
        StartCoroutine(DelayedAnimation(Player.
            GetComponentInChildren<Camera>().GetComponent<Animation>(),
            "DollyIn", 1f));
        shutdownDialogue();
        gameover = true;
    }

    private IEnumerator DelayedAnimation(Animation a, string anim, float delay)
    {
        yield return new WaitForSeconds(delay);
        a.Play(anim, PlayMode.StopAll);
    }

    private IEnumerator gameOverGUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameOver.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator survivedGUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        Survived.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
