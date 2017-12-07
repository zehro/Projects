using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YeggQuest.NS_UI
{
    public class UIPanel : MonoBehaviour
    {
        public GameObject firstSelected;

        private RectTransform rect;
        private CanvasGroup group;

        private bool open;
        private float openAmount;
        private float openAccel = 0.4f;
        private bool disabled;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            group = GetComponent<CanvasGroup>();
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime * 60;
            openAmount = Mathf.Lerp(openAmount, open ? 1 : 0, dt * openAccel);

            rect.localScale = new Vector3(openAmount, openAmount, 1);
            group.interactable = open && !disabled;
            group.alpha = openAmount;
        }

        public void Open()
        {
            open = true;
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        public void Close()
        {
            open = false;
        }

        public float GetOpenAmount()
        {
            return openAmount;
        }

        public void SetDisabled(bool disabled)
        {
            this.disabled = disabled;
        }
    }
}