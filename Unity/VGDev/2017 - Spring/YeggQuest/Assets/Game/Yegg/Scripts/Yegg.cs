using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using YeggQuest.NS_Bird;
using YeggQuest.NS_Logic;
using YeggQuest.NS_UI;

namespace YeggQuest.NS_Yegg
{
    public class Yegg : Logical
    {
        [Tooltip("Every yegg needs a globally unique index.")]
        public int index;
        public float pickupRadius = 1;
        public AudioClip sound;
        public AudioMixerGroup mixerGroup;

        private GameMusic music;
        private UIYeggs uiYeggs;
        private Bird bird;
        private Transform shine;
        private Transform burstQuad1;
        private Transform burstQuad2;

        private Vector3 origin;
        private float angle = 0f;
        private float minSpeed = 1.5f;
        private float maxSpeed = 3f;
        private bool isCollected = false;

        private float jiggle = 0;
        private float jiggleVel = 0;
        private float jiggleAccel = 0.2f;
        private float jiggleDrag = 0.15f;

        void Start()
        {
            if (!Application.isPlaying)
                return;

            // If this yegg has already been collected, delete it immediately

            if (GameData.IsYeggCollected(index))
                Destroy(gameObject);

            music = FindObjectOfType<GameMusic>();
            uiYeggs = FindObjectOfType<UIYeggs>();
            bird = FindObjectOfType<Bird>();
            shine = transform.Find("FX");
            burstQuad1 = transform.Find("FX/BurstQuad1");
            burstQuad2 = transform.Find("FX/BurstQuad2");

            origin = transform.position;
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                gameObject.name = "Yegg (#" + index + ")";
                return;
            }

            // Animate the shine effects

            shine.rotation = Camera.main.transform.rotation;
            burstQuad1.localRotation = Quaternion.Euler(0, 0, Time.time * +10f);
            burstQuad2.localRotation = Quaternion.Euler(0, 0, Time.time * -15f);
            burstQuad1.localScale = Vector3.one * (2 + Mathf.Sin(Time.time * 2) * 0.4f);
            burstQuad2.localScale = Vector3.one * (2 + Mathf.Cos(Time.time * 2f) * 0.3f);

            // Animate the jiggle

            float dt = Time.deltaTime * 60;
            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;

            if (!isCollected)
            {
                // Run the hovering animation

                transform.position = origin + Vector3.up * Mathf.Sin(Time.time * 1.75f) * 0.1f;
                transform.rotation = Quaternion.Euler(10, angle, Mathf.Sin(Time.time) * 5);

                // Check if the player is close enough to collect this yegg

                float dist = Vector3.Distance(origin, bird.GetPosition());
                float t = 1 - Mathf.InverseLerp(pickupRadius, pickupRadius * 4, dist);
                angle += Mathf.Lerp(minSpeed, maxSpeed, t) * dt;

                if (t == 1)
                {
                    isCollected = true;
                    GameData.CollectYegg(index);
                    StartCoroutine(CollectRoutine());

                    music.Duck(5);
                    AudioSource a = new GameObject().AddComponent<AudioSource>();
                    a.outputAudioMixerGroup = mixerGroup;
                    a.PlayOneShot(sound);
                    Destroy(a.gameObject, 6);
                }
            }
        }

        public override bool Evaluate()
        {
            return isCollected;
        }

        void OnValidate()
        {
            if (pickupRadius < 1)
                pickupRadius = 1;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }

        // TODO: comments/cleanup

        private IEnumerator CollectRoutine()
        {
            jiggleVel = -0.2f;
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            float scale = 1;

            uiYeggs.Show(6);
            float animationTime = 3f;
            float dt = Time.deltaTime * 60;

            for (float f = 0; f < animationTime; f += Time.deltaTime)
            {
                float t = f / animationTime;
                
                float circleTime = Mathf.Pow(f, 2f) * 8;
                float circleAmount = Mathf.Pow(Mathf.Clamp01(Mathf.InverseLerp(0, 0.75f, t) * 0.99f), 0.25f);
                circleAmount = circleAmount * (1 - circleAmount) * 4;
                Vector3 circle = new Vector3(Mathf.Sin(circleTime), 0, Mathf.Cos(circleTime)) * circleAmount;
                
                Vector3 targPos = bird.GetPosition();
                targPos += Vector3.up * (Mathf.Pow(1 - Mathf.Clamp01(Mathf.InverseLerp(0.9f, 1, t)), 0.25f) * (0.5f + t * 0.75f));
                pos = Vector3.Lerp(pos, targPos + circle, Mathf.Clamp01(t * 2) * dt);

                float targScale = Mathf.Clamp01(1 - Mathf.InverseLerp(0.95f, 1, t)) * 0.5f;
                scale = Mathf.Lerp(scale, targScale, Mathf.Clamp01(t * 4) * dt);

                transform.position = pos;
                transform.localScale = new Vector3(1 - jiggle, 1 + jiggle, 1 - jiggle) * scale;
                transform.rotation = Quaternion.Euler(0, f * maxSpeed + t * t * 360 * 8, 0) * rot;
                shine.rotation = Camera.main.transform.rotation;

                yield return null;
            }

            uiYeggs.Recount();
            Destroy(gameObject);
            bird.Jiggle(-0.15f);
        }
    }
}