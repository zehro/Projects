using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Text.RegularExpressions;

/**
 * Used to say something through the speech system.
 * Poor choice of file name though in retrospect
 * You can put this object on anything really, as long as there is a
 * Speech System gameObject in the Canvas. :>)
 */
public class Script : MonoBehaviour {
    public Text screenText;
    public Image image;
    public float textTime = 2.5f;
    public bool startOnPlay;
    [Tooltip("The dialogue to be said through the speech system. Press enter for new line")]
    [TextArea]
    public string dialogue;
    public UnityEvent endEvent;
    bool playing = false;
    Quote[] quotes;

    void Start() {
        quotes = createScript(dialogue);
        if (startOnPlay) {
            play();
        }
    }

    public void play() {
        if (!playing) {
            if (gameObject.tag == "speech") {
                StartCoroutine(fadeInImage(image));
                StartCoroutine(playScript());
            } else {
                Script script = GameObject.FindGameObjectWithTag("speech").GetComponent<Script>();
                script.createScript(dialogue);
                StartCoroutine(script.fadeInImage(image));
                StartCoroutine(script.playScript());
            }
        }
    }

    IEnumerator fadeInImage(Image img) {
        image.enabled = true;
        Color col = img.color;
        float curAlpha = 0;
        while (curAlpha < 1) {
            curAlpha += 0.05f;
            col.a = curAlpha;
            img.color = col;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator fadeOutImage(Image img) {
        Color col = img.color;
        float curAlpha = 1;
        while (curAlpha > 0) {
            curAlpha -= 0.05f;
            col.a = curAlpha;
            img.color = col;
            yield return new WaitForSeconds(0.01f);
        }
        img.enabled = false;
        screenText.enabled = false;
    }

    IEnumerator playScript() {
        if (!playing) {
            playing = true;
            screenText.enabled = true;
            foreach (Quote quote in quotes) {
                screenText.text = quote.getLine();
                yield return new WaitForSeconds(quote.getTime());
            }
            StartCoroutine(fadeOutImage(image));
            if (endEvent != null) endEvent.Invoke();
        }
        playing = false;
    }

    public Quote[] createScript(string body) {
        string[] bodyList = body.Split('\n');
        Quote[] newQuotes = new Quote[bodyList.Length];
        int i = 0;
        while (i < bodyList.Length) {
            string bodyText = bodyList[i];
            float timeOfText = textTime;
            Regex reg = new Regex("{[.0-9]+}");
            if (reg.IsMatch(bodyText)) {
                float num = float.Parse(reg.Match(bodyText).ToString().Replace("{", "").Replace("}", ""));
                timeOfText = num;
                bodyText = bodyText.Replace(reg.Match(bodyText).ToString(), "");
            }
            newQuotes[i] = new Quote(bodyText, timeOfText);
            i++;
        }
        return newQuotes;
    }
}
