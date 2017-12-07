using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A world-space UI balloon which floats up and down and
// can be opened and closed. It also wiggles its contents.

namespace YeggQuest.NS_UI
{
    public class UIBalloon : MonoBehaviour
    {
        private RectTransform rect;

        private Vector3 initialPos;
        private Vector3 initialScale;

        private Vector2 scale = Vector2.zero;
        private Vector2 scaleVel = Vector2.zero;
        private Vector2 scaleAccel = new Vector2(0.075f, 0.125f);
        private float scaleDrag = 0.14f;
        private float scaleTarg = 1;

        void Awake()
        {
            rect = GetComponent<RectTransform>();

            initialPos = transform.localPosition;
            initialScale = rect.localScale;
        }

        void Update()
        {
            // Orient the balloon so it faces the camera

            transform.rotation = Camera.main.transform.rotation;

            // Move the balloon up and down gently

            Vector3 up = transform.InverseTransformDirection(Vector3.forward);
            transform.localPosition = initialPos + up * Mathf.Sin(Time.time * 2) * 0.05f;

            // Scale the balloon with opening and closing

            float dt = Time.deltaTime * 60;
            scaleVel += Vector2.Scale((Vector2.one * scaleTarg) - scale, scaleAccel * dt);
            scaleVel *= 1 - (scaleDrag * dt);
            scale += scaleVel * dt;

            if (scale.x < 0)
            {
                scale.x = 0;
                scaleVel.x = 0;
            }

            if (scale.y < 0)
            {
                scale.y = 0;
                scaleVel.y = 0;
            }

            Vector3 bScale = Vector3.Scale(initialScale, scale);
            bScale.z = initialScale.z;
            rect.localScale = bScale;
        }

        public void SetOpen(bool open)
        {
            scaleTarg = (open ? 1 : 0);
        }
    }
}