using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameLoad : MonoBehaviour
{
    public static string filename = Application.persistentDataPath + "/sa.ve";

    GameSaveData data;
    float dataCompletion;

    Text text;
    Text textActive;
    Text textDisabled;
    Outline textOutline;
    Outline textActiveOutline;
    Outline textDisabledOutline;
    AudioSource[] sounds;
    PostProcessing fx;

    int textSize = 40;
    int textOutlineSize = 4;

    int state = 0;      // 0 = main screen, 1 = new game/quit confirmation, 2 = fadeout, 3 = extras screen
    int sMain = 0;      // continue, new game, extras, quit
    int sConfirm = 1;   // yes, no
    
    float fade = -1.5f;
    float fadeSpeed = 0.025f;
    float fadeCutoff = -2;

    string[] tMain = { "\n\n\nCONTINUE\nNEW GAME\n\nQUIT",
                       "\n\n\n\n\n\n EXTRAS \n\n" };
    string[] tMainActive = { ">> CONTINUE   ",
                         "\n\n>> NEW GAME   ",
                     "\n\n\n\n>>  EXTRAS    ",
                 "\n\n\n\n\n\n>>   QUIT     " };
    string[] tConfirm = { "ARE YOU SURE YOU"
                      + "\nWANT TO START A NEW GAME?"
                      + "\nPREVIOUS SAVE IS DELETED.\n"
                      + "\nYES"
                      + "\nNO ",
                          "ARE YOU SURE YOU"
                      + "\nWANT TO QUIT?\n\n"
                      + "\nYES"
                      + "\nNO " };
    string[] tConfirmActive = { "\n\n\n>> YES   ",
                            "\n\n\n\n\n>> NO    " };
    string[] tExtras = { "== AUDIO EXTRAS! ==\n\n",
                         "UNUSED TRACK A\n",
                         "UNUSED TRACK B\n",
                         "UNUSED TRACK C\n",
                         "CHORUS FUN\n\n",
                         "BACK" };
    string[] tExtrasActive = { ">> UNUSED TRACK A   \n\n\n",
                               ">> UNUSED TRACK B   \n",
                             "\n>> UNUSED TRACK C   ",
                         "\n\n\n>>   CHORUS FUN     ",
                 "\n\n\n\n\n\n\n>>      BACK        " };

    public AudioClip[] audioExtras;
    AudioSource audioExtrasPlayer;
    bool audioPlaying = false;
    float audioDuck = 1;
    float audioDuckSpeed = 0.2f;

    void Awake()
    {
        // If there's no save file, don't bother with any loading. Just go straight to the living room

        if (!File.Exists(filename))
        {
            SceneManager.LoadScene(1);
            return;
        }

        // Otherwise, we have something to load. Get the data from the save file and store it
        
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(filename, FileMode.Open);
        data = (GameSaveData)bf.Deserialize(file);
        file.Close();

        printGameSaveData(data);

        text = GameObject.Find("Text").GetComponent<Text>();
        textActive = GameObject.Find("TextActive").GetComponent<Text>();
        textDisabled = GameObject.Find("TextDisabled").GetComponent<Text>();
        textOutline = GameObject.Find("Text").GetComponent<Outline>();
        textActiveOutline = GameObject.Find("TextActive").GetComponent<Outline>();
        textDisabledOutline = GameObject.Find("TextDisabled").GetComponent<Outline>();
        sounds = GetComponents<AudioSource>();
        fx = FindObjectOfType<PostProcessing>();
        audioExtrasPlayer = transform.Find("AudioExtras").GetComponent<AudioSource>();

        dataCompletion = gameCompletion(data);
        tMain[0] = "== " + dataCompletion + "% COMPLETE ==" + tMain[0];
    }

    void Update()
    {
        AudioListener.volume = 1;

        // Scale the UI

        float scale = Mathf.Min(Screen.width / 640f, Screen.height / 480f);
        int t1 = (int)(textSize * scale);
        Vector2 t2 = new Vector2(textOutlineSize * 0.5f * scale, textOutlineSize * scale);

        text.fontSize = t1;
        textActive.fontSize = t1;
        textDisabled.fontSize = t1;
        textOutline.effectDistance = t2;
        textActiveOutline.effectDistance = t2;
        textDisabledOutline.effectDistance = t2;

        // User interaction and state machine

        int move = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);
        move -= (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);

        if (state == 0)
        {
            if (fade == 1 && move != 0)
            {
                sounds[0].Play();

                sMain = (sMain + move) % 4;
                if (sMain == -1)
                    sMain = 3;
                if (dataCompletion != 100 && sMain == 2)
                {
                    if (move == 1)
                        sMain = 3;
                    if (move == -1)
                        sMain = 1;
                }
            }

            text.text = tMain[0];
            textActive.text = tMainActive[sMain];
            textDisabled.text = tMain[1];

            if (Input.GetKeyDown(KeyCode.Space))
            {
                sounds[0].Play();
                fx.fxFlare(2000, 0.1f);

                if (sMain == 0)
                    state = 2;
                else if (sMain == 1 || sMain == 3)
                    state = 1;
                else
                {
                    state = 3;
                    sConfirm = 4;
                }
            }

            fade = Mathf.Min(fade + fadeSpeed, 1);
        }

        else if (state == 1)
        {
            if (move != 0)
            {
                sounds[0].Play();
                sConfirm = 1 - sConfirm;
            }

            text.text = tConfirm[sMain / 2];
            textActive.text = tConfirmActive[sConfirm];
            textDisabled.text = "";

            if (Input.GetKeyDown(KeyCode.Space))
            {
                sounds[0].Play();
                fx.fxFlare(2000, 0.1f);

                if (sConfirm == 0)
                    state = 2;
                else
                    state = 0;
            }
        }

        else if (state == 2)
        {
            if (fade == 1)
                fx.fxFlare(5000, 0.2f);
            fade -= fadeSpeed;

            if (fade < fadeCutoff)
            {
                // Load the saved game

                if (sMain == 0)
                {
                    GameController.LEVEL_PROGRESS = data.levelProgress;
                    GameController.GAME_PROGRESS = data.gameProgress;
                    GameController.GAME_SECRETS = data.gameSecrets;
                    GameController.GAME_COMPLETE = data.gameComplete;
                    SceneManager.LoadScene(data.levelStart);
                }

                // Delete the save and start a new game

                else if (sMain == 1)
                {
                    if (File.Exists(filename))
                        File.Delete(filename);
                    SceneManager.LoadScene(1);
                }

                // Quit the game

                else if (sMain == 3)
                {
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #endif
                    Application.Quit();
                }
            }
        }

        else if (state == 3)
        {
            if (audioDuck == 1 && move != 0)
            {
                sounds[0].Play();

                sConfirm = (sConfirm + move) % 5;
                if (sConfirm == -1)
                    sConfirm = 4;
            }

            text.text = "";
            foreach (string s in tExtras)
                text.text += s;
            textActive.text = tExtrasActive[sConfirm];
            textDisabled.text = "";

            if (Input.GetKeyDown(KeyCode.Space))
            {
                sounds[0].Play();

                if (sConfirm < 4)
                {
                    if (!audioPlaying)
                    {
                        audioExtrasPlayer.clip = audioExtras[sConfirm];
                        audioExtrasPlayer.PlayDelayed(1);
                        audioPlaying = true;
                    }

                    else if (audioPlaying)
                    {
                        audioExtrasPlayer.Stop();
                        audioPlaying = false;
                        fx.fxFlare(2000, 0.1f);
                    }
                }

                else
                {
                    state = 0;
                    sConfirm = 1;
                    audioDuck = 1;
                    fx.fxFlare(2000, 0.1f);
                }
            }

            if (audioPlaying && !audioExtrasPlayer.isPlaying)
                audioPlaying = false;
            if (audioPlaying)
                audioDuck = Mathf.Max(audioDuck - audioDuckSpeed, 0);
            else
                audioDuck = Mathf.Min(audioDuck + audioDuckSpeed, 1);
        }

        float a;
        a = Mathf.SmoothStep(0, 1, Mathf.Clamp01(fade));
        text.color = new Color(1, 1, 1, a * (0.2f + Mathf.SmoothStep(0, 1, audioDuck) * 0.8f));
        textActive.color = Color.Lerp(new Color(1, 1, 0, a), new Color(1, 0, 1, a), 1 - audioDuck);
        textDisabled.color = new Color(1, 1, 1, a * (dataCompletion == 100 ? 1 : 0.2f));

        a *= 0.5f;
        sounds[1].volume = (0.3f + fx.fxDesync * 0.5f) * a * audioDuck;
        sounds[2].volume = (fx.fxDesync * 0.0005f) * a * audioDuck;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void printGameSaveData(GameSaveData data)
    {
        /*string output;
        output = "| start in: " + data.levelStart + " "
               + "| level progress: {" + data.levelProgress[0] + ", " + data.levelProgress[1] + ", " + data.levelProgress[2] + "} "
               + "| game progress: {" + data.gameProgress[0] + ", " + data.gameProgress[1] + ", " + data.gameProgress[2] + "} "
               + "| secrets: {" + data.gameSecrets[0] + ", " + data.gameSecrets[1] + ", " + data.gameSecrets[2] + "} "
               + "| complete: " + (data.gameComplete ? "yes" : "no") + " |\n"
               + "| total completion: " + gameCompletion(data) + "% |";
        Debug.Log(output);*/
    }

    public static float gameCompletion(GameSaveData data)
    {
        // How to beat the game 100%:
        //
        // 7 Red Wing levels
        // 5 Green Wing levels
        // 4 Blue Wing levels
        // 3 switches to pull at the end of each wing
        // 3 secrets to find in the last level of each wing
        // 1 final TV to insert the tape into in the Finale
        //
        // 23 things to do in total

        float progress = data.levelProgress[0] + data.levelProgress[1] + data.levelProgress[2];
        progress += (data.gameProgress[0] > 0 ? 1 : 0) + (data.gameProgress[1] > 0 ? 1 : 0) + (data.gameProgress[2] > 0 ? 1 : 0);
        progress += (data.gameSecrets[0] ? 1 : 0) + (data.gameSecrets[1] ? 1 : 0) + (data.gameSecrets[2] ? 1 : 0);
        progress += (data.gameComplete ? 1 : 0);

        return Mathf.Floor(progress * 100f / 23f);
    }
}