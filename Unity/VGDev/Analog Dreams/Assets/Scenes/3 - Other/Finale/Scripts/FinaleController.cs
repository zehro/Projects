using UnityEngine;
using System.Collections;

public class FinaleController : MonoBehaviour
{
    public AudioClip endingLoop;
    public AudioClip explosion;

    GameController game;
    public AudioSource[] music;
    AudioReverbZone reverb;
    float musCymbalTime = 86.9f;
    float musEndingTime = 88.5f;
    bool musCymbal;
    bool musEnding;

    public Transform starDome;
    GameObject star;
    GameObject star2;

    public Light sunlight;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        music = GetComponents<AudioSource>();
        reverb = GetComponent<AudioReverbZone>();
        starDome = GameObject.Find("Stardome").transform;
        star = GameObject.Find("Stardome/Star");
        star2 = GameObject.Find("Stardome/Star 2");
        sunlight = GameObject.Find("Lighting/Sunlight").GetComponent<Light>();
    }

    void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            GameObject newStar = (GameObject) Instantiate((i > 800 ? star : star2), Vector3.zero, Random.rotationUniform);
            newStar.transform.parent = starDome;
        }
    }

    void Update()
    {
        if (Time.timeSinceLevelLoad > 1)
            reverb.enabled = true;

        // Stardome

        float s = Time.time * music[0].pitch;
        starDome.rotation = Quaternion.Euler(new Vector3(s, s, s));

        // Music

        float t = Time.timeSinceLevelLoad;

        if (!musCymbal && t > musCymbalTime)
        {
            musCymbal = true;
            music[1].Play();
        }

        if (!musEnding && t > musEndingTime)
        {
            musEnding = true;

            music[0].Stop();
            music[0].clip = endingLoop;
            music[0].loop = true;
            music[0].Play();
        }

        float v = Mathf.InverseLerp(310, 490, game.playerCamera.transform.position.z);
        music[0].volume = 0.01f + (1 - v) * 0.99f;
        reverb.decayTime = 4 + v * 8;
        reverb.room = (int)(-1000 * (1 - v));
        v *= (1 - Mathf.InverseLerp(475, 500, game.playerCamera.transform.position.z));
        music[2].volume = 0.125f + v * 0.625f;
        music[2].pitch = 0.25f + v * 0.5f;
    }
}