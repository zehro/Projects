using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    GameController game;
    Text text;
    Text textActive;
    Outline textOutline;
    Outline textActiveOutline;
    int textSize = 40;
    int textOutlineSize = 4;

    int state = 0;      // 0 = main menu, 1 = confirmations
    int sMain = 0;      // resume, rewind, skip, eject
    int sConfirm = 1;   // yes, no

    bool canSkip = false;

    string[] tMain = {"== MENU ==\n\n\nRESUME\nREWIND\n",
                      "SKIP  ", "\nEJECT "};
    string[] tMainActive = {">> RESUME   ",
                        "\n\n>> REWIND   ",
                    "\n\n\n\n>> SKIP     ",
                "\n\n\n\n\n\n>> EJECT    "};
    string[] tConfirm = {"ARE YOU SURE YOU" +
                       "\nWANT TO RESTART" +
                       "\nTHIS LEVEL?\n" +
                       "\nYES" +
                       "\nNO ",
                         "ARE YOU SURE YOU" +
                       "\nWANT TO SKIP" +
                       "\nTHIS LEVEL?\n" +
                       "\nYES" +
                       "\nNO ",
                         "ARE YOU SURE YOU" +
                       "\nWANT TO QUIT? UNSAVED" +
                       "\nPROGRESS IS LOST.\n" +
                       "\nYES" +
                       "\nNO "};
    string[] tConfirmActive = {"\n\n\n>> YES   ",
                           "\n\n\n\n\n>> NO    "};

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        text = GameObject.Find("Text").GetComponent<Text>();
        textActive = GameObject.Find("TextActive").GetComponent<Text>();
        textOutline = GameObject.Find("Text").GetComponent<Outline>();
        textActiveOutline = GameObject.Find("TextActive").GetComponent<Outline>();

        canSkip = (game.levelType != LevelType.Other);                                              // You can only skip this level IF: you're in a level in any of the three wings

        if (!GameController.CAN_SKIP_ALL && canSkip == true)                                        // ... and if you've beaten this specific level before (accelerates backtracking for secrets)
            canSkip &= (game.levelNumber <= GameController.LEVEL_PROGRESS[(int) game.levelType]);   // Comment out these 2 lines for the "nice" version of the game where you can skip any puzzle
    }

    void Update()
    {
        if (game.gameState() == GameState.Pause)
        {
            // Scale the UI

            float scale = Mathf.Min(Screen.width / 640f, Screen.height / 480f);
            int t1 = (int)(textSize * scale);
            Vector2 t2 = new Vector2(textOutlineSize * 0.5f * scale, textOutlineSize * scale);

            text.fontSize = t1;
            textActive.fontSize = t1;
            textOutline.effectDistance = t2;
            textActiveOutline.effectDistance = t2;

            // User interaction and state machine

            int move = (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0);
            move -= (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);

            if (state == 0)
            {
                if (move != 0)
                    game.playPauseClick(false);

                sMain = (sMain + move) % 4;
                if (sMain == -1)
                    sMain = 3;
                if (!canSkip && sMain == 2)
                {
                    if (move == 1)
                        sMain = 3;
                    if (move == -1)
                        sMain = 1;
                }

                text.text = tMain[0];
                if (canSkip)
                    text.text += tMain[1];
                text.text += tMain[2];
                textActive.text = tMainActive[sMain];

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (sMain == 0)
                    {
                        state = 0;
                        sConfirm = 1;
                        gameObject.SetActive(false);
                    }

                    else
                    {
                        state = 1;
                        game.playPauseClick(true);
                    }
                }
            }

            else
            {
                if (move != 0)
                {
                    game.playPauseClick(false);
                    sConfirm = 1 - sConfirm;
                }

                text.text = tConfirm[sMain - 1];
                textActive.text = tConfirmActive[sConfirm];

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (sConfirm == 0)
                    {
                        if (sMain == 1)
                        {
                            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        }

                        else if (sMain == 2)
                        {
                            game.win(game.tvs[0]);

                            state = 0;
                            sMain = 0;
                            sConfirm = 1;
                            gameObject.SetActive(false);
                        }

                        else if (sMain == 3)
                        {
                            #if UNITY_EDITOR
                                UnityEditor.EditorApplication.isPlaying = false;
                            #endif
                            Application.Quit();
                        }
                    }

                    else
                    {
                        state = 0;
                        game.playPauseClick(true);
                    }
                }
            }
        }

        // Reset if the game is no longer paused

        else
        {
            state = 0;
            sMain = 0;
            sConfirm = 1;
            gameObject.SetActive(false);
        }
    }
}