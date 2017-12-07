using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TeamUtility.IO;

public class Game : MonoBehaviour
{
    public GameObject players, corrals;
    public string menu;
    public Camera winCam;
    public Camera winCam2, startCam;
    public Text[] timer;
    public Text[] playerScoreText;
    public Text[] playerWinText;
    public Image[] scoreGUI;
    public Image[] barnDoors;
    public Texture2D cursor;
    public AudioSource music;

    public float matchTime;
    public int scoreMax;
    public static int playerCount;

    State state;
    Camera cam, cam2;
    Vector3 camTarget1, camTarget2, winSpot1, winSpot2;
    GameObject player1, player2;
    Timer clock;
    int score1, score2, winTeam;
    bool gameover;
    string nextScene;
    private static Game instance;

    private Game() { }

    public static Game inst
    {
        get
        {
            if (instance == null)
                instance = new Game();
            return inst;
        }
    }

    void Start()
    {
        //Set cursor to not show and to the custom cursor
        Cursor.visible = false;
        Cursor.SetCursor(cursor, new Vector2(0, 0), CursorMode.ForceSoftware);
        //Get players
        player1 = players.transform.GetChild(0).gameObject;
        player2 = players.transform.GetChild(1).gameObject;
        //Get the player Cameras
        cam = player1.GetComponentInChildren<Camera>();
        cam2 = player2.GetComponentInChildren<Camera>();
        //Set up the End Game Cameras
        camTarget1 = winCam.transform.position + new Vector3(-3, 0, 0);
        winSpot1 = corrals.transform.GetChild(0).transform.position + new Vector3(10, 1.07f, 0);
        camTarget2 = winCam2.transform.position + new Vector3(3, 0, 0);
        winSpot2 = corrals.transform.GetChild(1).transform.position + new Vector3(-10, 1.07f, 0);
        //Initialize a clock
        clock = GetComponent<Timer>();
        //Turn split screen off/on
        //SetSplitSceen(false);
        //Set control scheme
        //SetControllers(true);
        //Players
        PlayerControlToggle(false);
        //Opening animations
        SetScoreGUI(false);
        barnDoors[0].transform.GetComponent<Animation>().Play("RightDoorOpen");
        barnDoors[1].transform.GetComponent<Animation>().Play("LeftDoorOpen");
        startCam.gameObject.transform.GetComponent<Animation>().Play("StartGame1");
        state = State.START;
    }

    void Update()
    {
        GameUpdate();
    }

    void GameUpdate()
    {
        switch (state)
        {
            case State.START:
                Cursor.visible = false;
                StartGame();
                break;
            case State.PLAY:
                UpdateTime();
                if (InputManager.GetButtonDown("Pause"))
                {
                    PauseGame(true);
                }

                CheckWin();
                break;
            case State.PAUSE:
                if (InputManager.GetButtonDown("Pause"))
                {
                    PauseGame(false);
                }
                break;
            case State.GAMEOVER:
                if (!gameover)
                {
                    player1.transform.position = winSpot1;
                    player1.transform.LookAt(new Vector3(0, 0, 0));
                    player2.transform.position = winSpot2;
                    player2.transform.LookAt(new Vector3(0, 0, 0));
                    SetScoreGUI(false);
                    PlayerControlToggle(false);
                    SheepToggle(false);
                    music.Pause();
                }

                if (winTeam == 1)
                {
                    winCam.enabled = true;
                    winCam.transform.position = Vector3.Lerp(winCam.transform.position, camTarget1, .01f);
                    playerWinText[0].enabled = true;
                    textBounce(playerWinText[0]);
                }
                else if (winTeam == 2)
                {
                    winCam2.enabled = true;
                    winCam2.transform.position = Vector3.Lerp(winCam2.transform.position, camTarget2, .01f);
                    playerWinText[1].enabled = true;
                    textBounce(playerWinText[1]);
                } else
                {
                    winCam.enabled = true;
                    winCam.transform.position = Vector3.Lerp(winCam.transform.position, camTarget1, .01f);
                    playerWinText[0].enabled = true;
                    playerWinText[0].text = "Tie!";
                    textBounce(playerWinText[0]);
                }
                Invoke("LoadNextScene", 10f);
                Invoke("CloseDoors", 9.65f);
                gameover = true;
                break;
        }
    }

    void StartGame()
    {
        //BARN ANIMATION
        if (clock.getTimeRemaining() <= 5f)
        {
            startCam.transform.GetComponent<Animation>().Play("StartGame2");
        }
        if (GetComponent<Timer>().isTimeRemaining() || InputManager.GetButtonDown("Pause"))
        {
            startCam.enabled = false;
            PlayerControlToggle(true);
            SetScoreGUI(true);
            state = State.PLAY;
        }
    }

    public void PauseGame(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
            PlayerControlToggle(false);
            Cursor.visible = true;
            state = State.PAUSE;
        }
        else
        {
            Time.timeScale = 1;
            PlayerControlToggle(true);
            Cursor.visible = false;
            state = State.PLAY;
        }
    }

    /// <summary>
    /// Set if the players can move or not
    /// </summary>
    /// <param name="control"></param> True for movement
    public void PlayerControlToggle(bool control)
    {
        if (control)
        {
            foreach (PlayerController player in players.GetComponentsInChildren<PlayerController>())
            {
                player.setPlayerControl(1);
            }
        }
        else
        {
            foreach (PlayerController player in players.GetComponentsInChildren<PlayerController>())
            {
                player.setPlayerControl(0);
            }
        }
    }

    /// <summary>
    /// Set if sheep are active or not
    /// </summary>
    /// <param name="enable"></param> True for enable
    public void SheepToggle(bool enable)
    {
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Sheep"))
        {
            g.GetComponent<Sheep>().enabled = enable;
        }
    }

    /// <summary>
    /// Decrement the match time
    /// </summary>
    private void UpdateTime()
    {
        //http://answers.unity3d.com/questions/905990/how-can-i-make-a-timer-with-the-new-ui-system.html
        matchTime = Mathf.Clamp(matchTime, 0, matchTime - Time.deltaTime);

        //Divide the time by sixty to get the minutes.
        var minutes = (matchTime) / 60;
        //Use the euclidean division for the seconds.
        var seconds = (matchTime - 30) % 60;
        var fraction = (matchTime * 10) % 10;
        //update the label value
        string temp = string.Format("{0:0}:{1:00}", minutes, seconds, fraction);
        string sec1 = temp[2].ToString();
        string sec2 = temp[3].ToString();
        if (sec1 == "6")
        {
            sec1 = "0";
        }
        timer[0].text = temp[0].ToString();
        timer[1].text = temp[1].ToString();
        timer[2].text = sec1;
        timer[3].text = sec2;

        if (sec1 == "0" && temp[0].ToString() == "0")
        {
            //timer.GetComponent<Animation>().wrapMode = WrapMode.Loop;
            //timer[0].GetComponent<Animation>().Play();
            //timer[1].GetComponent<Animation>().Play();
            //timer[2].GetComponent<Animation>().Play();
            //timer[3].GetComponent<Animation>().Play();
        }
    }

    /// <summary>
    /// If a team scores by placing a sheep in their corral
    /// increment there score
    /// </summary>
    /// <param name="team"></param> True for team 1, false for team 2
    public void Score(int team, int amt)
    {
        if (team == 1)
        {
            score1 = Mathf.Clamp(score1 + amt, 0, score1 + amt);
            playerScoreText[0].text = "" + score1;
            textBounce(playerScoreText[0]);
        }
        else
        {
            score2 = Mathf.Clamp(score2 + amt, 0, score2 + amt);
            playerScoreText[1].text = "" + score2;
            textBounce(playerScoreText[1]);
        }
    }

    /// <summary>
    /// Activate text animation
    /// </summary>
    /// <param name="text"></param> a text object with an animation component
    private void textBounce(Text text)
    {
        text.transform.GetComponent<Animation>().Play();
    }

    /// <summary>
    /// Call to check win condition after scores have been updated
    /// </summary>
    public void CheckWin()
    {
        if (matchTime <= 30 || score1 >= scoreMax || score2 >= scoreMax)
        {
            if (!(state == State.GAMEOVER))
            {
                if (score1 > score2)
                {
                    winTeam = 1;

                }
                else if (score1 < score2)
                {
                    winTeam = 2;
                }
            }
            state = State.GAMEOVER;
        }
    }

    private void CloseDoors()
    {
        playerWinText[0].enabled = false;
        playerWinText[1].enabled = false;
        barnDoors[0].transform.GetComponent<Animation>().Play("RightDoorClose");
        barnDoors[1].transform.GetComponent<Animation>().Play("LeftDoorClose");
    }

    private void LoadNextScene()
    {

        if (menu != "")
        {
            SceneManager.LoadScene(menu);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void SetScoreGUI(bool b)
    {
        scoreGUI[0].enabled = b;
        scoreGUI[1].enabled = b;
        scoreGUI[2].enabled = b;
        playerScoreText[0].enabled = b;
        playerScoreText[1].enabled = b;
        timer[0].enabled = b;
        timer[1].enabled = b;
        timer[2].enabled = b;
        timer[3].enabled = b;
    }

    /// <summary>
    /// Switches between splitscreen and single player
    /// </summary>
    /// <param name="split"></param> Splitscreen or not
    void SetSplitSceen(bool split)
    {
        if (!split)
        {
            //Andrew Oliveira Was Here o/
            cam.pixelRect = new Rect(0, 0, Screen.width, Screen.height);
            cam.fieldOfView = 70;
            cam2.pixelRect = new Rect(0, 0, 0, 0);
            scoreGUI[1].enabled = false;
            playerScoreText[1].enabled = false;
        }
        else {
            cam.pixelRect = new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2);
            cam.fieldOfView = 50;
            cam2.pixelRect = new Rect(0, 0, Screen.width, Screen.height / 2);
            scoreGUI[1].enabled = true;
            playerScoreText[1].enabled = true;
            playerScoreText[1].text = "" + score2;
        }
    }

    void SetControllers(bool b)
    {
        if (b)
        {
            InputManager.SetInputConfiguration("KeyboardAndMouse", PlayerID.One);
            InputManager.SetInputConfiguration("Windows_Gamepad1", PlayerID.Two);
        }
        else {
            InputManager.SetInputConfiguration("Windows_Gamepad2", PlayerID.Two);
            InputManager.SetInputConfiguration("Windows_Gamepad1", PlayerID.One);
        }
    }

    /// <summary>
    /// Get the current scores printed to the console
    /// </summary>
    public void getDebugScore()
    {
        Debug.Log("The score is Sheperds 1: " + score1 + " Shpepherds 2: " + score2);
    }

    private enum State { START, PLAY, PAUSE, GAMEOVER };
}
