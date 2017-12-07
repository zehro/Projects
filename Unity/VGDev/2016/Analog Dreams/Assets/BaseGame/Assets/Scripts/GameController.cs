using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameController : MonoBehaviour
{
    // Global gamestate data
    
    public static int[] LEVEL_COUNT = { 7, 5, 4 };                  // the total number of levels in each wing

    public static int[] LEVEL_PROGRESS = { 0, 0, 0 };               // how many levels you've beaten in each wing
    public static int[] GAME_PROGRESS = { 0, 0, 0 };                // 1 if you've flipped the switch in each wing, 2 if you've started the animation in the hub
    public static bool[] GAME_SECRETS = { false, false, false };    // whether or not you found the secret orb in each wing
    public static bool GAME_COMPLETE = false;                       // whether or not you inserted the tape into the final pyramid TV

    public static int SWITCH_INDEX = 0;                             // which switch room to load (used to genericize)
    public static int PLAYER_FOV = 85;                              // just a constant to keep this consistent/easily changeable
    public static bool CAN_SKIP_ALL = false;                        // whether or not the player can skip puzzles even if they haven't beaten them

    // Basic information about this level

    public LevelType levelType;
    public int levelNumber;
    public string levelDesigner;

    // The game controller holds public references to the important objects in the game
    // This is so that everything else can simply find the GameController and use them

    public Player player;
    public Camera playerCamera;
    public PlayerCursor playerCursor;
    public VHSTape vhsTape;
    public TV[] tvs;

    // The game controller controls the game state and also does the transitions

    GameState state = GameState.EnteringLevel;
    float stateTime;
    float enteringTime = 0.25f;
    float startingTime = 0.1f;
    float stoppingTime = 0.75f;
    float exitingTime = 6f;
    Vector3 exitingPos;
    Vector3 exitingVelocity;
    Vector3 exitingVelocity2;
    Quaternion exitingAngularVelocity;
    float exitingDrag = 0.75f;
    Quaternion exitingRot;
    string exitingScene;
    float exitingFOV = 20;
    bool firstFrame = true;
    bool canPause = true;

    FinaleController finale;
    bool finaleExplosion;

    // Also manages the effects for objects being destroyed by portals

    public float pdDanger = 0;
    float pdDangerDrag = 0.1f;
    
    public PostProcessing playerFX;
    FlareLayer playerFlare;
    PortalDestroy[] destroyables;
    AudioSource[] stateAudio;
    PauseMenu pauseMenu;

    void Awake()
    {
        finale = FindObjectOfType<FinaleController>();

        playerFX = transform.Find("PlayerPedestal/Player/PlayerCamera").GetComponent<PostProcessing>();
        playerFlare = transform.Find("PlayerPedestal/Player/PlayerCamera").GetComponent<FlareLayer>();
        destroyables = FindObjectsOfType<PortalDestroy>();
        stateAudio = transform.Find("GameAudio/StateAudio").GetComponents<AudioSource>();
        pauseMenu = transform.Find("GameUI/PauseMenu").GetComponent<PauseMenu>();

        if (GAME_COMPLETE && FindObjectOfType<LivingRoomController>() != null)
            canPause = false;
    }

    void Start()
    {
        foreach (AudioSource a in stateAudio)
            a.ignoreListenerPause = true;
        playerCamera.fieldOfView = PLAYER_FOV;
    }

    void Update()
    {
        // DEBUG STUFF
        
        if (Input.GetKeyDown(KeyCode.Tab))
            Debug.Break();

        GameState prevState = state;

        switch (state)
        {
            case GameState.EnteringLevel:
                if (stateTime > enteringTime)
                {
                    state = GameState.Play;
                    AudioListener.volume = 1;
                }
                break;

            case GameState.Play:

                // Find maximum danger among destroyables

                float danger = 0;
                foreach (PortalDestroy d in destroyables)
                {
                    if (d.getDanger() > danger && playerCursor.isGrabbing(d.GetComponent<PlayerGrabbable>()))
                        danger = d.getDanger();
                }

                // Update the portal destroy danger accordingly

                pdDanger += (danger - pdDanger) * pdDangerDrag;

                // Do the glitchy effects

                if (danger > 0)
                {
                    playerFX.fxFlare(pdDanger * Random.Range(1.5f, 3), 0.975f);
                    player.cameraShake(pdDanger * 0.05f);
                }

                if (pdDanger > 0)
                {
                    stateAudio[1].pitch = 0.2f + Mathf.Min(playerFX.fxDesync, 10);
                    stateAudio[1].volume = Mathf.Clamp01(playerFX.fxDesync - 0.05f);
                    stateAudio[2].pitch = 0.6f + Mathf.Min(playerFX.fxDesync * 0.3f, 1);
                    stateAudio[2].volume = Mathf.Clamp01(playerFX.fxDesync * 0.2f - 0.1f) * 0.75f;
                }

                else
                {
                    stateAudio[1].volume = 0;
                    stateAudio[2].volume = 0;
                }

                if (Input.GetButtonDown("Pause") && canPause)
                {
                    state = GameState.Stopping;
                    stateAudio[0].Play();
                    stateAudio[1].volume = 0;
                    stateAudio[2].volume = 0;
                    player.cameraShake(0);
                }
                break;
            
            case GameState.Stopping:
                if (stateTime > stoppingTime)
                {
                    playerFX.fxFlare(0, 0);
                    pauseMenu.gameObject.SetActive(true);
                    state = GameState.Pause;
                    stateAudio[1].volume = 1;
                    stateAudio[1].pitch = 1;
                    stateAudio[2].volume = 1;
                    stateAudio[2].pitch = 1;
                }
                break;
            
            case GameState.Pause:
                stateAudio[1].volume = 0.3f + playerFX.fxDesync * 0.5f;
                stateAudio[2].volume = playerFX.fxDesync * 0.02f;
                if (Input.GetButtonDown("Pause") || !pauseMenu.isActiveAndEnabled)
                {
                    state = GameState.Starting;
                    stateAudio[1].volume = 0;
                    stateAudio[2].volume = 0;
                    stateAudio[3].Play();
                }
                break;

            case GameState.Starting:
                if (stateTime > startingTime)
                    state = GameState.Play;
                break;

            case GameState.ExitingLevel:
                Vector3 goalPos = new Vector3(0, 1, 2.2f);
                Quaternion goalRot = Quaternion.Euler(90f, 0, 0);

                exitingVelocity *= exitingDrag;
                exitingVelocity2 *= exitingDrag * 0.5f;
                exitingPos += exitingVelocity;
                vhsTape.insertPos += exitingVelocity2;

                float angle;
                Vector3 axis;
                exitingAngularVelocity.ToAngleAxis(out angle, out axis);
                exitingAngularVelocity = Quaternion.AngleAxis(angle * 0.95f, axis);
                exitingRot = exitingRot * exitingAngularVelocity;

                float t = Mathf.SmoothStep(0, 1, stateTime / 3f);
                t = Mathf.SmoothStep(0, 1, t);
                playerCamera.transform.localPosition = Vector3.Lerp(exitingPos, goalPos, t);
                playerCamera.transform.localRotation = Quaternion.Slerp(exitingRot, goalRot, t);

                if (finale == null)
                {
                    t = Mathf.SmoothStep(0, 1, stateTime / 5f);
                    t = Mathf.SmoothStep(0, 1, t);
                    playerCamera.fieldOfView = Mathf.Lerp(PLAYER_FOV, exitingFOV, t);

                    stateAudio[1].volume = 0;
                    stateAudio[2].volume = 0;
                    AudioListener.volume = Mathf.Pow(1 - (stateTime / exitingTime), 2);

                    if (stateTime > exitingTime)
                    {
                        if (exitingScene == "")
                            exitingScene = SceneManager.GetActiveScene().name;
                        SceneManager.LoadScene(exitingScene);
                    }
                }

                else
                {
                    float time = 37.5f;

                    if (stateTime < time)
                    {
                        float f = Mathf.InverseLerp(1, time, stateTime);
                        f = Mathf.Pow(f, 5);
                        playerCamera.transform.localPosition += new Vector3(0, 20000 * f, 0);
                        playerCamera.transform.localPosition += Random.onUnitSphere * f * f * 500;
                        playerCamera.fieldOfView = Mathf.Lerp(PLAYER_FOV, 179, f * f);
                        finale.music[0].pitch = 1 + f * f * 16;
                        finale.sunlight.intensity = 0.5f + 0.75f * f;
                        playerFX.fxFinale(f);

                        if (!stateAudio[1].isPlaying)
                            stateAudio[1].Play();
                        if (!stateAudio[2].isPlaying)
                            stateAudio[2].Play();
                        stateAudio[1].pitch = 0.2f + Mathf.Min(playerFX.fxDesync * 0.01f, 4);
                        stateAudio[1].volume = Mathf.Clamp01(playerFX.fxDesync * 3) * 1.5f;
                        stateAudio[2].pitch = 0.6f + Mathf.Min(playerFX.fxDesync * 0.3f, 1);
                        stateAudio[2].volume = Mathf.Clamp01(playerFX.fxDesync * 0.2f - 0.1f) * 0.75f;

                        if (!finaleExplosion && stateTime > time - 2)
                        {
                            finaleExplosion = true;
                            stateAudio[0].bypassReverbZones = false;
                            stateAudio[0].clip = finale.explosion;
                            stateAudio[0].reverbZoneMix = 1.1f;
                            stateAudio[0].volume = 1;
                            stateAudio[0].Play();
                        }
                    }

                    else
                    {
                        playerCamera.transform.localPosition += Vector3.up * 20000;
                        playerCamera.clearFlags = CameraClearFlags.SolidColor;
                        playerCamera.backgroundColor = Color.black;
                        playerCamera.cullingMask = (1 << 5);
                        stateAudio[1].Stop();
                        stateAudio[2].Stop();
                        finale.music[0].Stop();
                        finale.music[2].Stop();
                        finale.starDome.gameObject.SetActive(false);
                        playerFX.fxFinale(0);

                        if (stateTime > 50)
                            SceneManager.LoadScene(exitingScene);
                    }
                }

                break;
        }

        if (!firstFrame)
            stateTime += Time.unscaledDeltaTime;
        if (state != prevState)
            stateTime = 0;
        firstFrame = false;

        Time.timeScale = (state == GameState.Play || state == GameState.ExitingLevel ? 1 : 0);
        AudioListener.pause = (state == GameState.Play || state == GameState.ExitingLevel ? false : true);
        if (state != GameState.ExitingLevel)
            playerCamera.cullingMask = (state == GameState.Pause ? (1 << 5) : -1);
        playerFlare.enabled = (state != GameState.Pause);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void win(TV tv)
    {
        // Increment the level progress if we're in a wing

        if (levelType != LevelType.Other && levelNumber > LEVEL_PROGRESS[(int) levelType])
            LEVEL_PROGRESS[(int) levelType] = levelNumber;

        // If we just beat a Green Wing level, beat all Red Wing levels
        // If we just beat a Blue Wing level, beat all Red and Green Wing levels
        // This is entirely so that the progression works within the editor

        if (levelType == LevelType.GreenWing || levelType == LevelType.BlueWing)
        {
            LEVEL_PROGRESS[0] = LEVEL_COUNT[0];
            GAME_PROGRESS[0] = 2;
        }
        if (levelType == LevelType.BlueWing)
        {
            LEVEL_PROGRESS[1] = LEVEL_COUNT[1];
            GAME_PROGRESS[1] = 2;
        }

        // Exiting level logic

        state = GameState.ExitingLevel;
        stateTime = 0;
        stateAudio[4].Play();
        
        playerCamera.transform.parent = tv.transform;
        exitingPos = playerCamera.transform.localPosition;
        exitingVelocity = tv.transform.InverseTransformVector(player.getPhysicalVelocity() * Time.fixedDeltaTime);
        exitingVelocity2 = player.getPhysicalVelocity() * Time.fixedDeltaTime;
        exitingAngularVelocity = player.getAngularVelocity();
        exitingRot = playerCamera.transform.localRotation;
        exitingScene = tv.nextScene;
        player.gameObject.SetActive(false);

        // As long as we're not going to the finale or currently in it, just save the game normally

        if (exitingScene != "Finale" && finale == null)
            saveGame(tv.nextScene);

        // If we are in the finale, save the game (still going back to the hub) with the flag that we've completed it

        if (finale != null)
        {
            if (!GAME_COMPLETE)
            {
                GAME_COMPLETE = true;
                saveGame("Hub");
            }
        }
    }

    // Pause the game when the window loses focus

    void OnApplicationFocus(bool p)
    {
        if (p == false && state == GameState.Play && canPause)
        {
            state = GameState.Stopping;
            stateTime = stoppingTime;
        }
    }

    // Allow other objects to ping the game state and data

    public GameState gameState()
    {
        return state;
    }

    public float getStateTime()
    {
        return stateTime;
    }

    public float getStartingTime()
    {
        return startingTime;
    }

    public float getStoppingTime()
    {
        return stoppingTime;
    }

    // Pause menu stuff

    public void playPauseClick(bool strong)
    {
        stateAudio[5].Play();
        if (strong)
            playerFX.fxFlare(2000, 0.1f);
    }

    // Portal destroy stuff

    public void playDestroyNoise()
    {
        stateAudio[6].Play();
    }

    // Saving

    public void saveGame(string levelStart)
    {
        // Save game data to file

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(GameLoad.filename, FileMode.Create);

        GameSaveData data = new GameSaveData();
        data.levelStart = levelStart;
        data.levelProgress = LEVEL_PROGRESS;
        data.gameProgress = GAME_PROGRESS;
        data.gameSecrets = GAME_SECRETS;
        data.gameComplete = GAME_COMPLETE;

        bf.Serialize(file, data);
        file.Close();

        GameLoad.printGameSaveData(data);
    }
}

[System.Serializable]
public class GameSaveData
{
    public string levelStart;      // where the player should enter the game upon reloading (this is the next map as soon as they put the tape in the TV)
    public int[] levelProgress;    // the three-length array which stores how many levels the player has beaten in Red, Green, and Blue Wing respectively
    public int[] gameProgress;     // the three-length array which stores 1) if the player has turned on each switch and 2) seen the animation in the hub
    public bool[] gameSecrets;     // the three-length array which stores if the player obtained the secret orb in Red, Green, and Blue Wing respectively
    public bool gameComplete;      // whether or not the player has inserted the tape into the final TV on top of the pyramid (have they done the ending)
}

public enum GameState
{
    EnteringLevel, Play, Stopping, Pause, Starting, ExitingLevel
}

public enum LevelType
{
    RedWing, GreenWing, BlueWing, Other
}