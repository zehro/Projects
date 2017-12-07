using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Paint;
using YeggQuest.NS_UI;

// The main game controller.

namespace YeggQuest
{
    public class Game : MonoBehaviour
    {
        // References

        [Header("Level Setup")]
        public LevelInfo levelInfo;
        public LevelType levelType;

        [Space(10)]
        [Header("Internal References")]
        public GameCoordinator coordinator;                            // The game coordinator
        public GameMusic music;                                        // The music player
        public AudioMixer mixer;                                       // The audio mixer
        public Bird bird;                                              // The bird
        public Transform uiPrefab;                                     // The UI prefab
        public UI ui;                                                  // The UI

        void Awake()
        {
            Paintable.LoadPaintTextures();
            ui = Instantiate(uiPrefab).GetComponent<UI>();
            ui.gameObject.SetActive(true);
            coordinator.StartScene();
        }

        void Update()
        {
            Shader.SetGlobalVector("_BirdPosition", bird.GetPosition());
            Cursor.lockState = CursorLockMode.Locked;

            // Audio mixing

            float v = Mathf.Pow(ui.GetWipe(), 0.05f);
            mixer.SetFloat("GameVolume", Mathf.Lerp(-80, 0, v));
            mixer.SetFloat("UIVolume", Mathf.Lerp(-80, 0, v));
            mixer.SetFloat("GamePitch", Time.timeScale);

            // TODO: DEBUG RESET

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift)
             && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.P))
            {
                GameData.Clean();
                SceneManager.LoadScene(0);
            }
        }
    }
}