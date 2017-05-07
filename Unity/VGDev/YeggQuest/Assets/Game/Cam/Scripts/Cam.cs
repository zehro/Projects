using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Cam
{
    public class Cam : MonoBehaviour
    {
        private List<CamVolume> volumes;
        private CamStrategyPlayerControlled defaultStrategy;
        private Camera cam;

        // The variables which define the current state of the camera.

        private CamStrategyResult state;
        private float playerControl = 1;

        private CamCutscene cutscene;
        private float cutsceneInfluence = 0;

        void Awake()
        {
            volumes = new List<CamVolume>(FindObjectsOfType<CamVolume>());
            volumes.Sort((x, y) => x.priority.CompareTo(y.priority));
            defaultStrategy = gameObject.AddComponent<CamStrategyPlayerControlled>();

            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            // Start by setting the camera state variables based on the default strategy.

            state = defaultStrategy.Direct();
            playerControl = 1;

            // Now, loop through all the volumes in the scene (in priority order)
            // and have them each influence the camera state variables.

            foreach (CamVolume v in volumes)
            {
                CamVolumeResult vol = v.Direct();
                playerControl = Mathf.Max(playerControl - vol.influence, 0);
                ApplyStrategy(vol.result, vol.influence);
            }

            // Now, apply the current cutscene.

            if (cutscene != null)
            {
                if (!cutscene.IsPlaying())
                {
                    cutscene = null;
                    cutsceneInfluence = 0;
                }

                else
                {
                    CamVolumeResult vol = cutscene.Direct();
                    playerControl = Mathf.Max(playerControl - vol.influence, 0);
                    cutsceneInfluence = vol.influence;
                    ApplyStrategy(vol.result, vol.influence);
                }
            }

            // Set the actual transform and camera variables to the state variables.
            
            transform.position = state.anchorPosition + state.offsetPosition;
            transform.LookAt(state.lookAtPosition, Vector3.up);
            transform.localRotation *= Quaternion.Euler(0, 0, state.roll);
            cam.fieldOfView = state.fov;
        }
        
        public float GetPlayerControl()
        {
            return playerControl;
        }

        public void PlayCutscene(CamCutscene cutscene)
        {
            if (this.cutscene == null)
            {
                this.cutscene = cutscene;
                cutscene.Play();
            }
        }

        public float GetCutsceneInfluence()
        {
            return cutsceneInfluence;
        }

        private void ApplyStrategy(CamStrategyResult strategy, float influence)
        {
            state.anchorPosition = Vector3.Lerp(state.anchorPosition, strategy.anchorPosition, influence);
            state.offsetPosition = Vector3.Lerp(state.offsetPosition, strategy.offsetPosition, influence);
            state.lookAtPosition = Vector3.Lerp(state.lookAtPosition, strategy.lookAtPosition, influence);
            state.roll = Mathf.Lerp(state.roll, strategy.roll, influence);
            state.fov = Mathf.Lerp(state.fov, strategy.fov, influence);
        }
    }
}