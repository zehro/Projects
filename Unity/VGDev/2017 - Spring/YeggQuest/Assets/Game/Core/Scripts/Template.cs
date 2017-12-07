using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest
{
    [ExecuteInEditMode]
    public class Template : MonoBehaviour
    {
        public LevelStyle style;

        [Space(10)]
        [Header("Palette")]
        public Color[] sunlightColors;
        public Color[] backlightColors;
        public Color[] ambientColors;

        [Space(10)]
        [Header("Internal References")]
        public Material[] skyboxes;
        public Cubemap reflection;
        public CloudMaterials[] cloudMats;
        public MeshRenderer[] clouds;
        public Light sunlight;
        public Light backlight;
        public GameObject snow;

        void Update()
        {
            int s = (int) style;

            for (int i = 0; i < 3; i++)
            {
                clouds[i].material = cloudMats[s].mats[i];
            }

            RenderSettings.skybox = skyboxes[s];
            RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;
            RenderSettings.customReflection = reflection;

            sunlight.color = sunlightColors[s];
            backlight.color = backlightColors[s];
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColors[s];

            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.white;
            RenderSettings.fogDensity = 0.0025f;
            RenderSettings.fogMode = FogMode.ExponentialSquared;

            snow.SetActive(style == LevelStyle.Winter);
            if (Application.isPlaying && style == LevelStyle.Winter)
                snow.transform.position = Camera.main.transform.position + Vector3.up * 10f;
        }
    }

    [System.Serializable]
    public struct CloudMaterials
    {
        public Material[] mats;
    }

    public enum LevelStyle
    {
        Summer, Autumn, Winter
    }
}