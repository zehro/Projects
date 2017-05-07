using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Logic;

namespace YeggQuest.NS_Paint
{
    public class PaintButton : MonoBehaviour
    {
        public Transform button;
        public LogicalPaintDemand input;
        public AudioSource sound;

        private bool pushedPrev;

        private float jiggle;
        private float jiggleVel;
        private float jiggleAccel = 0.2f;
        private float jiggleDrag = 0.15f;

        private float buttonSpeed = 0;
        private float buttonAccel = 0.01f;
        private float buttonScale = 1;
        private float buttonDrag = 0.5f;

        void Update()
        {
            // Update the scale of the base

            float dt = Time.deltaTime * 60;
            bool pushed = Logic.SafeEvaluate(input, false);

            if (pushed && !pushedPrev)
            {
                jiggleVel = 0.15f;
                sound.Play();
            }

            if (!pushed && pushedPrev)
                jiggleVel = -0.15f;

            jiggleVel -= jiggle * jiggleAccel * dt;
            jiggleVel *= 1 - (jiggleDrag * dt);
            jiggle += jiggleVel * dt;

            transform.localScale = new Vector3(1 + jiggle * 0.5f, 1 + jiggle * 0.5f, 1 - jiggle);
            pushedPrev = pushed;

            // Update the scale and rotation of the button

            float scaleTarg = (pushed ? 0.25f : 1);
            buttonScale += (scaleTarg - buttonScale) * buttonDrag * dt;
            button.localScale = new Vector3(1, 1, buttonScale);

            float speedDelta = (pushed ? 1 : -1) * buttonAccel * dt;
            buttonSpeed = Mathf.Clamp01(buttonSpeed + speedDelta);
            button.localRotation *= Quaternion.Euler(0, 0, buttonSpeed * dt);
        }
    }
}