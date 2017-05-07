using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Logic;

namespace YeggQuest.NS_Paint
{
    public class PaintPool : MonoBehaviour
    {
        public Logical input;

        private PaintVolume volume;
        private AudioSource sound;
        private ParticleSystem particles;
        private Transform fountain;
        private Material grilleMat;
        private Material fountainMat;
        private Material particleMat;

        float flow = 0;
        float flowSpeed = 0.01f;
        float pitch;

        void Awake()
        {
            volume = GetComponent<PaintVolume>();
            sound = GetComponentInChildren<AudioSource>();
            particles = GetComponentInChildren<ParticleSystem>();
            grilleMat = transform.Find("Grille").GetComponent<MeshRenderer>().material;
            fountain = transform.Find("Fountain");
            fountainMat = transform.Find("Fountain").GetComponent<MeshRenderer>().material;
            particleMat = transform.Find("Fountain/Particles").GetComponent<ParticleSystemRenderer>().material;
            
            Color c = PaintColors.ToColor(volume.color);
            grilleMat.SetColor("_Color", c);
            fountainMat.SetColor("_Color", c);
            particleMat.SetColor("_Color", c);
            particleMat.SetColor("_EmissionColor", c * 0.2f);
            
            pitch = 1 + (int) volume.color * 0.03f + Random.Range(-0.01f, 0.01f);
        }

        void Start()
        {
            flow = (Logic.SafeEvaluate(input, true) ? 1 : 0);
        }

        void Update()
        {
            bool on = Logic.SafeEvaluate(input, true);
            float dt = Time.deltaTime * 60;
            flow = Mathf.MoveTowards(flow, on ? 1 : 0, flowSpeed * dt);
            float t = Yutil.Smootherstep(flow);

            volume.enabled = on;
            sound.pitch = pitch * t;
            sound.volume = Mathf.Sqrt(t);

            if (flow > 0.5f)
            {
                if (!particles.isPlaying)
                    particles.Play();
            }
            else if (particles.isPlaying)
                particles.Stop();

            fountain.localScale = Vector3.one * Mathf.Sqrt(t);
            fountainMat.SetFloat("_Flow", t);
        }
    }
}