using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Cam;
using YeggQuest.NS_UI;

namespace YeggQuest.NS_Spawner
{
    public class SpawnerEntrance : CollisionReceiver
    {
        [Space(10)]
        [Header("Entrance Behavior")]
        public LevelInfo levelInfo;             // the level information this entrance is connected to

        [Space(10)]
        [Header("Internal References")]
        public Spawner spawner;                 // the spawner this entrance contains
        public CamVolume volume;                // the "opening shot" for this entrance
        public Transform inside;                // the inside of this entrance
        public ParticleSystem burst;            // the particle burst
        public AudioSource spawnAudio;          // the audio for spawning
        public AudioSource enterAudio;          // the audio for entering
        public AudioSource ambienceAudio;       // the ambient audio
        public UIBalloon uiBalloon;             // the UI balloon
        public Text uiTitle;                    // the title of the level
        public Image uiImage;                   // the preview image for the level
        public Image[] uiYeggs;                 // the yeggs of the level
        public Sprite[] uiYeggSprites;          // the sprites for the yeggs

        private Bird bird;                      // the bird
        private GameCoordinator coordinator;    // the game coordinator
        
        private float ambiencePitch;

        private bool isJiggling;
        private float jiggle = 0;
        private float jiggleVel = 0;
        private float jiggleAccel = 0.1f;
        private float jiggleDrag = 0.1f;

        private float startTimer = 0;
        private float startTimerReset = 2;

        void Awake()
        {
            bird = FindObjectOfType<Bird>();
            coordinator = FindObjectOfType<GameCoordinator>();

            ambiencePitch = Random.Range(0.99f, 1.01f);

            // Set up the balloon from the level information

            if (!levelInfo)
                Destroy(uiBalloon.gameObject);
            else
            {
                uiBalloon.gameObject.SetActive(true);
                uiTitle.text = levelInfo.title;
                uiImage.sprite = levelInfo.image;

                for (int i = 0; i < uiYeggs.Length; i++)
                {
                    bool active = i < levelInfo.yeggs.Length;
                    uiYeggs[i].gameObject.SetActive(active);

                    if (active)
                    {
                        bool collected = GameData.IsYeggCollected(levelInfo.yeggs[i].index);
                        uiYeggs[i].sprite = uiYeggSprites[collected ? 1 : 0];
                    }
                }
            }
        }

        void Update()
        {
            float dt = Time.deltaTime * 60;
            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;

            transform.localScale = new Vector3(1 - jiggle, 1 - jiggle, 1 + jiggle * 4);
            inside.transform.localRotation = Quaternion.Euler(0, 0, Time.time * 100);
            volume.transform.position = transform.position - Vector3.up;

            ambienceAudio.pitch = (0.66f + jiggle * 0.25f) * ambiencePitch;
            ambienceAudio.volume = 0.25f - Mathf.Abs(jiggle) * 2;

            startTimer = Mathf.Max(0, startTimer - Time.deltaTime);

            if (startTimer > 1.25f)
                volume.SetInfluence(1);

            if (uiBalloon)
            {
                Vector3 pos = transform.position - Vector3.up * 0.5f;
                float dist = Vector3.Distance(pos, bird.GetPosition());
                uiBalloon.SetOpen(startTimer == 0 && dist > 0.8f && dist < 8);
            }
        }

        // When the bird touches the despawn trigger down the barrel of the entrance,
        // quickly despawn the bird and tell the game coordinator to change the scene.

        public override void OnReceivedTriggerEnter(Collider other)
        {
            Bird bird = other.GetComponentInParent<Bird>();

            if (bird)
            {
                if (!isJiggling)
                    StartCoroutine(Jiggle(0.1f, -0.05f));

                coordinator.SetActiveSpawner(spawner);
                volume.maxInfluence = 0;

                if (levelInfo)
                    coordinator.GoToScene(levelInfo.gameObject.name);
                else
                    coordinator.GoToScene("");
            }
        }

        // Called by the spawner when the bird shoots out of it.

        public void Animate()
        {
            burst.Play();
            spawnAudio.Play();
            jiggleVel = 0.05f;
            startTimer = startTimerReset;
        }

        private IEnumerator Jiggle(float delay, float vel)
        {
            isJiggling = true;
            yield return new WaitForSeconds(delay);
            
            burst.Play();
            jiggleVel += vel;
            enterAudio.Play();

            yield return new WaitForSeconds(1);
            isJiggling = false;
        }
    }
}