using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace YeggQuest.NS_Water
{
    // Applies a water image effect to the camera with a driveable strength.

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class WaterEffect : MonoBehaviour
    {
        public Shader waterShader;
        public Texture waterRefraction;

        private Material mat;
        private Material material
        {
            get
            {
                if (mat == null)
                {
                    mat = new Material(waterShader);
                    mat.hideFlags = HideFlags.HideAndDontSave;
                    mat.SetTexture("_RefractionTex", waterRefraction);
                }

                return mat;
            }
        }

        private bool on;
        private float strength;
        private float strengthSpeed = 0.02f;

        void Update()
        {
            if (Application.isPlaying)
            {
                float dt = Time.deltaTime * 60;

                if (on)
                    strength += strengthSpeed * dt;
                else
                    strength -= strengthSpeed * dt;
                strength = Mathf.Clamp01(strength);

                material.SetFloat("_Strength", Yutil.Smootherstep(strength));
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }

        void OnDisable()
        {
            if (mat)
                DestroyImmediate(mat);
        }

        public void IsInWater(bool inWater)
        {
            on = inWater;
        }
    }
}