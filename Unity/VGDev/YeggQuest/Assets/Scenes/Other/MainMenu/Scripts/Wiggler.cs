using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest
{
    public class Wiggler : MonoBehaviour
    {
        public Vector3 orientation;
        public Vector3 angles;
        public Vector3 periods;
        public float offset;

        private Vector3 pos;

        void Start()
        {
            pos = transform.position;
        }

        void Update()
        {
            float t = Time.time + offset;
            transform.position = pos + Vector3.up * Mathf.Sin(Time.time) * 0.1f;
            transform.rotation = Quaternion.Euler(Mathf.Cos(t / periods.x) * angles.x,
                                                  Mathf.Sin(t / periods.y) * angles.y,
                                                  Mathf.Cos(t / periods.z) * angles.z) * Quaternion.Euler(orientation);
        }
    }
}