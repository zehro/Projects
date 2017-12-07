using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Secret : MonoBehaviour
{
    public int index = 0;

    GameController game;
    ParticleSystem particles;
    AudioSource[] sound;

    bool collected = false;
    float collectionDist = 0.25f;
    float collectionTime;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        particles = GetComponent<ParticleSystem>();
        sound = GetComponents<AudioSource>();

        if (GameController.GAME_SECRETS[index] == true)
            gameObject.SetActive(false);
    }

    void Start()
    {
        particles.startColor = new Color((index == 0 ? 1 : 0.25f), (index == 1 ? 1 : 0.25f), (index == 2 ? 1 : 0.25f));
    }

    void Update()
    {
        if (!collected)
        {
            particles.Emit(1);

            if (Vector3.Distance(transform.position, game.playerCamera.transform.position) < collectionDist)
            {
                collected = true;
                collectionTime = Time.time;

                GameController.GAME_SECRETS[index] = true;
                game.saveGame(SceneManager.GetActiveScene().name);

                game.player.cameraShake(0.05f);
                particles.Emit(60);
                sound[1].Play();
            }
        }

        else
        {
            float t = Mathf.Clamp01((Time.time - collectionTime) / 2.5f);

            sound[0].pitch = 1 + Mathf.Pow(t, 5);
            sound[0].volume = 1 - Mathf.SmoothStep(0, 1, t);
        }
    }
}