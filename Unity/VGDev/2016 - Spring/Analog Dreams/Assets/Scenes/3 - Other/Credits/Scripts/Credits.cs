using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Credits : MonoBehaviour
{
    Text text;
    Outline textOutline;
    int textSize = 40;
    int textOutlineSize = 4;
    float creditsStart = -0.1f;
    float creditsEnd = 1.1f;
    float creditsTime = 80f;
    float creditsEndTime = 82f;

    void Awake()
    {
        text = GameObject.Find("Text").GetComponent<Text>();
        textOutline = GameObject.Find("Text").GetComponent<Outline>();
        AudioListener.volume = 1;

        text.text += "\n\n(";
        for (int i = 0; i < GameController.GAME_SECRETS.Length; i++)
            text.text += (GameController.GAME_SECRETS[i] ? "*" : ".");
        text.text += ")";
    }

    void Update()
    {
        float scale = Mathf.Min(Screen.width / 640f, Screen.height / 480f);
        int t1 = (int)(textSize * scale);
        Vector2 t2 = new Vector2(textOutlineSize * 0.5f * scale, textOutlineSize * scale);
        text.fontSize = t1;
        textOutline.effectDistance = t2;

        float t = Time.timeSinceLevelLoad / creditsTime;
        float s = Mathf.Lerp(creditsStart, creditsEnd, t);
        text.rectTransform.localPosition = new Vector3(0, text.preferredHeight * s, 0);

        if (Time.timeSinceLevelLoad > creditsEndTime)
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
    }
}