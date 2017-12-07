using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts.UI
{
    public class CardTimer : MonoBehaviour
    {
        public float timeToSelect = 3f;
        private float timer = 0;
        private bool shouldFire = false;

        private const string beginningText = "Round Starting in: ";

        private float displayTime = 1f;

        private Text timerText;

        private Canvas canvas;

        #region Events
        public delegate void SelectionTimerAction();
        public static event SelectionTimerAction TimerFinish;

        void OnEnable()
        {
            CardSelectorMultiplayer.CardsSelected += AltSetup;
            CardSelectorMultiplayer.CardsDeselected += NormalSetup;
            SelectionTimer.TimerFinish += Show;
        }
        void OnDisable()
        {
            CardSelectorMultiplayer.CardsSelected -= AltSetup;
            CardSelectorMultiplayer.CardsDeselected -= NormalSetup;
            SelectionTimer.TimerFinish -= Show;
        }
        #endregion

        void Start()
        {
            timerText = GameObject.Find("Play Timer").GetComponent<Text>();
            canvas = this.GetComponent<Canvas>();

            canvas.enabled = true;
        }

        void Update()
        {
            if (shouldFire)
            {
                timer += Time.deltaTime;
                displayTime = Mathf.Clamp((timeToSelect - timer), 0, timeToSelect);
                timerText.text = beginningText + Mathf.Floor(displayTime).ToString(); ;

                if (timer >= timeToSelect)
                {
                    Hide();
                }
            }
        }

        private void Show()
        {
            canvas.enabled = true;
            NormalSetup();
        }

        private void AltSetup()
        {
            shouldFire = true;
            timer = 0f;
        }

        private void NormalSetup()
        {
            timerText.text = "Waiting for all players to select cards.";
            shouldFire = false;
        }

        private void Hide()
        {
            timer = 0f;
            canvas.enabled = false;
            if (TimerFinish != null && shouldFire) TimerFinish();
            shouldFire = false;
        }
    }
}
