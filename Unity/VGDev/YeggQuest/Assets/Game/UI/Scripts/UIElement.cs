using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YeggQuest.NS_UI
{
    public class UIElement : MonoBehaviour
    {
        public Vector2 closedPosition;

        private RectTransform rect;
        private Vector2 openPosition;
        private float openTimer;
        private float openSpeed = 1 / 32f;
        private float open;

        public void Awake()
        {
            rect = GetComponent<RectTransform>();
            openPosition = rect.anchoredPosition;
        }

        public void Update()
        {
            float dt = 60 * Time.deltaTime;
            openTimer = Mathf.Max(openTimer - Time.deltaTime, 0);
            open = Mathf.Clamp01(open + openSpeed * (openTimer > 0 ? 1 : -1) * dt);

            float time = Time.time * 0.5f;
            float t = Yutil.Smootherstep(Mathf.Pow(1 - open, 4));
            Vector2 off = new Vector2(Mathf.Sin(time * 2) * 2, Mathf.Cos(time * 2) * 5);

            rect.localScale = Vector3.one * (1 - t);
            rect.anchoredPosition = Vector2.Lerp(openPosition, closedPosition, t) + off;
            rect.rotation = Quaternion.Euler(Mathf.Sin(time) * 15, Mathf.Cos(time) * 15, 0);
        }

        public void Show(float time)
        {
            openTimer = Mathf.Max(openTimer, time);
        }
    }
}