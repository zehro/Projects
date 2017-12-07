using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;

namespace YeggQuest.NS_Cam
{
    [DisallowMultipleComponent]
    public class CamVolume : MonoBehaviour
    {
        [Header("Volume Settings")]
        public Vector3 size = Vector3.one;      // The size of the volume in world space
        [Range(0, 100)]
        public int priority = 0;                // The priority of this camera volume. Higher priorities override lower priorities
        [Range(0, 1)]
        public float maxInfluence = 1;          // The maximum influence this camera volume has over the camera
        [Range(0.1f, 10f)]
        public float transitionTime = 1;        // How long it takes for this camera volume to change influence when the bird enters or leaves it

        private Bird bird;                      // The bird character
        private Bounds bounds;                  // The bounds of this volume (center and size)
        private CamStrategy strategy;           // The strategy this volume uses to influence the camera

        private float curInfluence;             // Approaches 1 when the bird is inside (in transitionTime seconds) and approaches 0 otherwise

        void Awake()
        {
            bird = FindObjectOfType<Bird>();
            bounds = new Bounds();
            strategy = GetComponent<CamStrategy>();
        }

        void Update()
        {
            bounds.center = transform.position;
            bounds.size = size;
            
            bool hasBird = bounds.Contains(bird.GetPosition());
            curInfluence += Time.deltaTime * (hasBird ? 1 : -1) / transitionTime;
            curInfluence = Mathf.Clamp01(curInfluence);
        }

        // Draws this volume

        void OnDrawGizmos()
        {
            Gizmos.color = Color.Lerp(Color.black, Color.yellow, Influence());
            Gizmos.DrawWireCube(transform.position, size);
        }

        // Gets the total influence of this volume (current influence and maximum influence considered)

        float Influence()
        {
            return Yutil.Smootherstep(curInfluence) * maxInfluence;
        }

        // Returns how this volume and its strategy should be affecting the camera.

        public CamVolumeResult Direct()
        {
            if (!strategy)
            {
                Debug.LogError("Every CamVolume needs an attached CamStrategy!", gameObject);
                return new CamVolumeResult(0, new CamStrategyResult());
            }

            return new CamVolumeResult(Influence(), strategy.Direct());
        }

        public void SetInfluence(float influence)
        {
            curInfluence = influence;
        }
    }

    public struct CamVolumeResult
    {
        public float influence;             // How much this volume and its strategy should influence the camera
        public CamStrategyResult result;    // The result of the strategy this volume uses

        public CamVolumeResult(float influence, CamStrategyResult result)
        {
            this.influence = influence;
            this.result = result;
        }
    }
}