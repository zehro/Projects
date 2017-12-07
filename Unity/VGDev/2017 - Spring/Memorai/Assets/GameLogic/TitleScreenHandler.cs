using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
public class TitleScreenHandler : MonoBehaviour {
    int selectedState = 2;
    public Text option1; //Atlas
    public Text option2; //Story
    public Text option3; // Credits
    public Color selectedColor;
    public Color idleColor;
    public int storyLevelNum = 0;
    public int atlasLevelNum = 0;
    public int creditsLevelNum = 0;

    public UnityEvent transitionEvent;

	// Use this for initialization
	void Start () {
        idleColor = option1.color;
        StartCoroutine(goBackToIntro());
	}
	
    IEnumerator goBackToIntro() {
        yield return new WaitForSeconds(25);
        SceneManager.LoadScene(0);
    }
	// Update is called once per frame
	void Update () {
        if (selectedState != -1) {
            if (Input.GetAxis("Horizontal") > 0) {
                selectedState = 3;
            } else if (Input.GetAxis("Horizontal") < 0) {
                selectedState = 1;
            } else {
                selectedState = 2;
            }
        }


        if (selectedState == 1) {
            option1.color = selectedColor;
        } else {
            option1.color = idleColor;
        }

        if (selectedState == 2) {
            option2.color = selectedColor;
        } else {
            option2.color = idleColor;
        }

        if (selectedState == 3) {
            option3.color = selectedColor;
        } else {
            option3.color = idleColor;
        }

        if (Input.GetButtonDown("Jump") && selectedState > 0) {
            transitionEvent.Invoke();
            StartCoroutine(exitTitlePage(selectedState));
        }
	}

    IEnumerator exitTitlePage(int nextState) {
        selectedState = -1;
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(new int[3] { atlasLevelNum, storyLevelNum, creditsLevelNum }[nextState - 1]);
    }
}
