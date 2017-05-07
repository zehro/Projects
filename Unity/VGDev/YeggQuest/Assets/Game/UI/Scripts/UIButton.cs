using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YeggQuest.NS_UI
{
    public class UIButton : Button
    {
        UI ui;
        UIMenu menu;
        RectTransform rect;

        bool hovered;
        bool active;
        bool activePrev;

        private Vector2 scale;
        private Vector2 scaleVel;
        private Vector2 scaleAccel = new Vector2(0.15f, 0.25f);
        private float scaleDrag = 0.175f;
        private float scaleTarg;
        private float scaleNormal = 0.95f;
        private float scaleHovered = 1.1f;
        private float scalePressed = 1.05f;

        protected override void Awake()
        {
            base.Awake();

            ui = GetComponentInParent<UI>();
            menu = GetComponentInParent<UIMenu>();
            rect = GetComponent<RectTransform>();

            scaleTarg = scaleNormal;
            scale = Vector2.one * scaleTarg;
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;

            float t = Time.unscaledTime * 2 + rect.localPosition.y / 100;
            rect.localRotation = Quaternion.Euler(Mathf.Cos(t), 0, Mathf.Sin(t));

            // Scale

            activePrev = active;
            active = EventSystem.current.currentSelectedGameObject == gameObject || hovered;
            image.color = active ? menu.hoverColor : menu.normalColor;
            scaleTarg = (active ? scaleHovered : scaleNormal);

            if (IsPressed() && IsInteractable())
            {
                scaleTarg = scalePressed;
                scaleVel = Vector2.zero;
                scale = Vector2.one * scaleTarg;
            }

            if (!activePrev && active)
                ui.HoverSound();

            float dt = Mathf.Min(Time.unscaledDeltaTime * 60, 2);
            scaleVel += Vector2.Scale((Vector2.one * scaleTarg) - scale, scaleAccel * dt);
            scaleVel *= 1 - (scaleDrag * dt);
            scale += scaleVel * dt;

            /*if (scale.x < 0 || scale.x > 2)
            {
                scale.x = Mathf.Clamp(scale.x, 0, 2);
                scaleVel.x = 0;
            }

            if (scale.y < 0 || scale.y > 2)
            {
                scale.y = Mathf.Clamp(scale.y, 0, 2);
                scaleVel.y = 0;
            }*/

            rect.localScale = new Vector3(scale.x, scale.y, 1);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (IsInteractable())
                hovered = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            hovered = false;
        }
    }
}