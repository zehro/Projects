using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Cam;

namespace YeggQuest.NS_Spawner
{
    public class Spawner : MonoBehaviour
    {
        [Space(10)]
        [Header("Spawn Behavior")]
        public Vector3 spawnOffset;
        public float spawnRotation;
        public float spawnArc;
        public float spawnTime = 1f;

        [Space(10)]
        [Header("Internal References")]
        public SpawnerEntrance entrance;
        public SpawnerCheckpoint checkpoint;
        
        public void Animate()
        {
            if (entrance)
                entrance.Animate();
            if (checkpoint)
                checkpoint.Animate();
        }

        public void SetCamera()
        {
            foreach (CamVolume v in FindObjectsOfType<CamVolume>())
                v.SetInfluence(0);

            if (entrance)
                entrance.volume.SetInfluence(1);
            if (checkpoint)
                checkpoint.volume.SetInfluence(1);
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;

            Vector3 s = transform.position;
            Vector3 e = s + spawnOffset;
            Vector3 r = Quaternion.Euler(0, spawnRotation, 0) * Vector3.forward;
            Vector3 u = Vector3.up * 0.1f;

            Gizmos.DrawSphere(s, 0.05f);
            Gizmos.DrawSphere(e, 0.05f);
            Yutil.DrawArrow(e + r * 0.1f + u, e + r + u, Gizmos.color);

            for (int i = 0; i < 20; i++)
            {
                float t1 = i / 20f;
                float t2 = (i + 1) / 20f;
                Vector3 u1 = Vector3.up * (t1 * (1 - t1)) * spawnArc;
                Vector3 u2 = Vector3.up * (t2 * (1 - t2)) * spawnArc;
                Vector3 p1 = Vector3.Lerp(s, e, t1);
                Vector3 p2 = Vector3.Lerp(s, e, t2);
                Gizmos.DrawLine(p1 + u1, p2 + u2);
            }
        }
    }
}