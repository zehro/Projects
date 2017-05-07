using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_UI;

namespace YeggQuest
{
    public class GameMusic : MonoBehaviour
    {
        public AudioClip musicIntro;
        public AudioClip musicLoop;

        private Game game;
        private UIMenu menu;
        private AudioSource music;
        private float musicVol;

        private bool isBusy;
        private bool lowpass;
        private float lowpassAmount;

        void Start()
        {
            game = GetComponentInParent<Game>();
            menu = FindObjectOfType<UIMenu>();
            music = GetComponent<AudioSource>();
            musicVol = music.volume;
        }

        void Update()
        {
            lowpass = false;
            if (game.ui.GetWipe() < 0.5f)
                lowpass = true;
            if (menu.IsOpen() && game.levelType != LevelType.MainMenu)
                lowpass = true;

            float dt = Time.unscaledDeltaTime * 60;
            lowpassAmount = Mathf.MoveTowards(lowpassAmount, lowpass ? 1 : 0, 0.02f * dt);

            float t = Yutil.Smootherstep(Mathf.Sqrt(lowpassAmount));
            game.mixer.SetFloat("MusicCutoff", Mathf.Lerp(22000, 220, t));
        }

        public void Play(float time)
        {
            if (!isBusy)
                StartCoroutine(PlayRoutine(Mathf.Max(0, time)));
        }

        public void Duck(float time)
        {
            if (!isBusy)
                StartCoroutine(DuckRoutine(Mathf.Max(1, time)));
        }

        public void FadeOut(float time)
        {
            if (!isBusy)
                StartCoroutine(FadeOutRoutine(Mathf.Max(0.1f, time)));
        }

        private IEnumerator PlayRoutine(float time)
        {
            isBusy = true;

            yield return new WaitForSeconds(time);

            if (musicIntro)
            {
                music.PlayOneShot(musicIntro);
                yield return new WaitForSecondsRealtime(musicIntro.length);
            }

            if (music.volume > 0)
            {
                music.clip = musicLoop;
                music.loop = true;
                music.Play();
            }

            isBusy = false;
        }

        private IEnumerator DuckRoutine(float time)
        {
            isBusy = true;

            for (float f = 0; f < time; f += Time.deltaTime)
            {
                float v = 0.5f;
                v = Mathf.Max(v, 1 - f * 2);
                v = Mathf.Max(v, (f - time) * 0.5f + 1);
                v = Mathf.Clamp01(v);
                music.volume = Yutil.Smootherstep(v * v) * musicVol;
                yield return null;
            }

            music.volume = musicVol;
            isBusy = false;
        }

        private IEnumerator FadeOutRoutine(float time)
        {
            isBusy = true;

            for (float f = 0; f < time; f += Time.deltaTime)
            {
                float t = 1 - f / time;
                music.volume = Yutil.Smootherstep(t * t) * musicVol;
                yield return null;
            }

            music.Stop();
            isBusy = false;
        }
    }
}