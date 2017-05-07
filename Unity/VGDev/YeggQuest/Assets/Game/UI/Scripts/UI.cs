using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Cam;

namespace YeggQuest.NS_UI
{
    // The class which handles all UI, including UI sounds.

    public class UI : MonoBehaviour
    {
        public AudioClip hoverSound;
        public AudioClip circleWipeInSound;
        public AudioClip circleWipeOutSound;

        [Space(10)]
        [Header("Internal References")]
        public Image circleWipe;
        public AudioSource sound;
        public RectTransform[] cutsceneBars;
        public UIBPaint bPaint;
        public UIYeggs yeggs;
        public UIGPaint gPaint;
        public UITooltip tooltip;

        private Bird bird;
        private Cam cam;

        private float wipe;

        void Awake()
        {
            bird = FindObjectOfType<Bird>();
            cam = FindObjectOfType<Cam>();
        }

        void Update()
        {
            // Set the circle wipe

            circleWipe.material.SetFloat("_Wipe", wipe);
            Vector3 screen = Camera.main.WorldToScreenPoint(bird.GetPosition());
            circleWipe.material.SetVector("_Center", screen);

            // Set the cutscene bars

            Vector3 s = new Vector3(1, cam.GetCutsceneInfluence(), 1);
            cutsceneBars[0].localScale = s;
            cutsceneBars[1].localScale = s;

            // Let the player view the UI elements

            if (Yinput.ViewUI())
            {
                bPaint.Show(3);
                yeggs.Show(3);
                gPaint.Show(3);
            }
        }

        public float GetWipe()
        {
            return wipe;
        }

        public void CircleWipeIn(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(CircleWipeRoutine(1, duration));
            sound.PlayOneShot(circleWipeInSound);
        }

        public void CircleWipeOut(float duration)
        {
            StopAllCoroutines();
            StartCoroutine(CircleWipeRoutine(0, duration));
            sound.PlayOneShot(circleWipeOutSound);
        }

        public void HoverSound()
        {
            sound.pitch = Random.Range(0.95f, 1.05f);
            sound.PlayOneShot(hoverSound);
        }

        private IEnumerator CircleWipeRoutine(float target, float duration)
        {
            float init = wipe;

            for (float f = 0; f < duration; f += Time.deltaTime)
            {
                float t = f / duration;
                wipe = Mathf.Lerp(init, target, Yutil.Smootherstep(t));
                yield return null;
            }

            wipe = target;
        }
    }
}