using UnityEngine;
using YeggQuest.NS_ProcBlock;

namespace YeggQuest.NS_Water
{
    // This script is attached to an object inside the camera, and detects
    // whether or not the camera is currently submerged in water. If it is,
    // this script turns on the camera's attached water image effect.
    
    public class WaterEffectDriver : MonoBehaviour, Submergeable
    {
        public Camera cam;                      // The camera
        public WaterEffect waterEffect;         // The water effect on the camera
        public bool inWater                     // Whether the camera is currently in water
        {
            get;
            set;
        }

        private void Start()
        {
            cam = transform.parent.GetComponent<Camera>();
            waterEffect = cam.GetComponent<WaterEffect>();
        }

        private void Update()
        {
            waterEffect.IsInWater(inWater);
        }
    }
}