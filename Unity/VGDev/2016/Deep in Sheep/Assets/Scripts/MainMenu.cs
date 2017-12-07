using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Button play, tutorial, credits, exit;
	//public LoadingScreen load;
	public GameObject tutI, credI, fade, title;
	private bool exitSubMenu;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void StartGame() {
        SceneManager.LoadScene(1);
    }

    public void Tutorial() {
		hideButtons (false);
		tutI.SetActive(true);
		fade.SetActive (true);
		title.SetActive(false);
	}

	public void Credits() {
		hideButtons (false);
		credI.SetActive(true);
		fade.SetActive (true);
		title.SetActive(false);
	}

	public void Exit() {
		if (!exitSubMenu)
			Application.Quit ();
		else {
			hideButtons (true);
			tutI.SetActive(false);
			credI.SetActive(false);
			fade.SetActive (false);
			title.SetActive(true);
		}
	}

	public void hideButtons(bool b) {
		play.gameObject.SetActive(b);
		tutorial.gameObject.SetActive(b);
		credits.gameObject.SetActive(b);
		exitSubMenu = !b;

	}
}
