using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LivingRoomController : MonoBehaviour
{
    public Material tvBlack;

    Transform game;

    Transform night;
    GUITexture nightFade;
    AudioSource nightAudio;
    PlayerCursor cursor;

    Transform day;
    Transform dayCam;
    Transform dayCamBegin;
    Transform dayCamEnd;
    AudioSource dayAudio;
    GUITexture dayFade;

    void Awake()
    {
        game = GameObject.Find("GameController").transform;

        night = transform.Find("Night");
        nightFade = transform.Find("Night/NightFade").GetComponent<GUITexture>();
        nightAudio = transform.Find("Night/NightAudio").GetComponent<AudioSource>();
        cursor = game.Find("PlayerPedestal/Player/PlayerCamera/PlayerCursor").GetComponent<PlayerCursor>();

        day = transform.Find("Day");
        dayCam = transform.Find("Day/DayCam");
        dayCamBegin = transform.Find("Day/DayCamBegin");
        dayCamEnd = transform.Find("Day/DayCamEnd");
        dayAudio = transform.Find("Day/DayAudio").GetComponent<AudioSource>();
        dayFade = transform.Find("Day/DayFade").GetComponent<GUITexture>();
    }

    void Start()
    {
        // Turn on the proper objects (day after we've completed the game)

        if (GameController.GAME_COMPLETE)
        {
            game.Find("PlayerPedestal").gameObject.SetActive(false);
            game.Find("VHSPedestal").gameObject.SetActive(false);
            game.Find("TVPedestal/TV/TVWhine").gameObject.SetActive(false);
            game.Find("TVPedestal/TV/TVScreen").GetComponent<MeshRenderer>().material = tvBlack;
            day.gameObject.SetActive(true);
        }
        else
            night.gameObject.SetActive(true);
    }

    void Update()
    {
        if (GameController.GAME_COMPLETE)
        {
            // Audio

            float v = Mathf.InverseLerp(30f, 40f, Time.timeSinceLevelLoad);
            v *= (1 - Mathf.InverseLerp(84f, 94f, Time.timeSinceLevelLoad));
            dayAudio.volume = v;

            // Fade

            float f = Mathf.InverseLerp(36f, 46f, Time.timeSinceLevelLoad);
            f *= (1 - Mathf.InverseLerp(81f, 93f, Time.timeSinceLevelLoad));
            f = f * f * f * (f * (f * 6 - 15) + 10);
            dayFade.color = new Color(0, 0, 0, (1 - f) * 0.5f);

            // Camera

            float t = Mathf.InverseLerp(36f, 90, Time.timeSinceLevelLoad);
            t = t * t * t * (t * (t * 6 - 15) + 10);
            dayCam.position = Vector3.Lerp(dayCamBegin.position, dayCamEnd.position, t);
            dayCam.rotation = Quaternion.Slerp(dayCamBegin.rotation, dayCamEnd.rotation, t);

            if (Time.timeSinceLevelLoad > 95)
                SceneManager.LoadScene("Credits");
        }

        else
        {
            if (Time.timeSinceLevelLoad > 1 && !nightAudio.isPlaying)
                nightAudio.Play();

            if (Time.timeSinceLevelLoad < 10)
            {
                float t = Mathf.InverseLerp(2, 7, Time.timeSinceLevelLoad);
                t = t * t * t * (t * (t * 6 - 15) + 10);
                nightFade.color = new Color(0, 0, 0, (1 - t) * 0.5f);
                AudioListener.volume = t;
                cursor.cursorScale = t;
            }
        }
    }
}