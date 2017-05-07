using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Dialogue script based on TutorialDirector from Bit.Spike
/// For use in cutscenes, or one sided conversations during gameplay
/// </summary>
public class DialogueManager : MonoBehaviour {

	public static DialogueManager instance;

	public TextAsset dialogue;
    public GameObject[] images;
	public Text dialogueDisplay;

	private StringReader lineReader;

    private float nextText;
	private string targetText;
	private string current;

    // Use this for initialization
    void Start () {
		instance = this;
		nextText = Time.time;
        lineReader = new StringReader(dialogue.text);
        current = lineReader.ReadLine ();
		clearText ();
		targetText = current;
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogueDisplay.enabled && current != null) {
			if (Time.time > nextText) {
				if (targetText != null) {
                    if (!dialogueDisplay.text.Equals (targetText)) {
						dialogueDisplay.text = targetText.Substring (0, dialogueDisplay.text.Length + 1);
						nextText = nextText + 0.05f;
                        if (skipLine())
                            nextLine ();
					} else {
                        if (current.Equals("<End>")) {
                            clearText();
                            LevelManager.instance.shutdownDialogue();
                        } else if (current.Equals ("")) {
							nextLine ();
							nextText = Time.time;
						} else {
							current = "";
							nextText = nextText + 2f;
						}
					}
				}
			}
		}
	}

    void clearText()
    {
        dialogueDisplay.text = "";
    }

    // Modify this to allow any reasonable number of images.
    void nextLine()
    {
        current = lineReader.ReadLine();
        if (current != null)
        {
            clearText();
            switch (current)
            {
                case "<0>":
                    images[0].SetActive(true);
                    nextLine();
                    break;
                case "</0>":
                    images[0].SetActive(false);
                    nextLine();
                    break;
            }
            targetText = current;
        }
    }

    private bool skipLine() {
        if (TeamUtility.IO.InputManager.GetKey(KeyCode.Space)) {
            return true;
        }
        return false;
    }
}
