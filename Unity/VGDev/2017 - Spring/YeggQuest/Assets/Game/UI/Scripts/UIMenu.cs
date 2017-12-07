using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace YeggQuest.NS_UI
{
    public class UIMenu : MonoBehaviour
    {
        public Color normalColor;
        public Color hoverColor;
        public Color fadeMinColor;
        public Color fadeMaxColor;

        [Space(10)]
        [Header("Internal References")]
        public Image mainFade;
        public UIPanel mainPanel;
        public LayoutElement spacer;
        public GameObject resume;
        public GameObject newGame;
        public GameObject credits;
        public GameObject returnToCheckpoint;
        public GameObject returnToHub;

        public Image addlFade;
        public UIPanel confirmationPanel;
        public Text confirmationText;
        public UIPanel optionsPanel;
        public Toggle optionsCamHInvert;
        public Toggle optionsCamVInvert;
        public UIPanel creditsPanel;

        private GameObject prevMainSelection;
        private UIPanel addlPanel;
        private float addlFadeAmount;

        public delegate void ConfirmCallback();
        private ConfirmCallback confirm;
        private Game game;

        private bool open;

        void Awake()
        {
            game = FindObjectOfType<Game>();

            if (game.levelType != LevelType.NormalLevel)
            {
                returnToCheckpoint.SetActive(false);
                returnToHub.SetActive(false);
            }

            if (game.levelType == LevelType.MainMenu)
            {
                resume.SetActive(false);
                newGame.SetActive(true);
                credits.SetActive(true);
                spacer.minHeight = 330;
                Open();
                EventSystem.current.SetSelectedGameObject(newGame);
            }

            optionsCamHInvert.isOn = GameData.camHInvert;
            optionsCamVInvert.isOn = GameData.camVInvert;
        }

        void Update()
        {
            // Pausing

            if (Yinput.Pause() && game.levelType != LevelType.MainMenu)
            {
                // If the menu is closed, we need to ask the coordinator if it's okay to pause
                // (if it's currently not busy doing anything else.) If it isn't, we can open.

                if (!open)
                {
                    if (game.coordinator.CanPause())
                        Open();
                }

                // If the menu is open, the player can press Start to close the menu as long
                // as they're still on the first layer (no additional panel is open.)

                else if (open)
                {
                    if (!addlPanel)
                        Close();
                }
            }

            // Set global timescale.
            
            if (game.levelType != LevelType.MainMenu)
                Time.timeScale = (mainPanel.GetOpenAmount() < 0.5f) ? 1 : 0;

            // Set fades.

            float mainFadeAmount = (game.levelType == LevelType.MainMenu ? 0 : mainPanel.GetOpenAmount());
            float dt = Time.unscaledDeltaTime * 60;
            addlFadeAmount *= (1 - 0.4f * dt);
            if (addlPanel)
                addlFadeAmount = Mathf.Max(addlFadeAmount, addlPanel.GetOpenAmount());

            mainFade.color = Color.Lerp(fadeMinColor, fadeMaxColor, mainFadeAmount);
            addlFade.color = Color.Lerp(fadeMinColor, fadeMaxColor, addlFadeAmount);
        }

        private void Open()
        {
            open = true;
            mainPanel.Open();
        }

        private void Close()
        {
            open = false;
            mainPanel.Close();
            CloseAdditional();
        }

        private void OpenAdditional(UIPanel panel)
        {
            if (open && !addlPanel)
            {
                prevMainSelection = EventSystem.current.currentSelectedGameObject;
                mainPanel.SetDisabled(true);
                addlPanel = panel;
                addlPanel.Open();
            }
        }

        private void CloseAdditional()
        {
            if (addlPanel)
            {
                EventSystem.current.SetSelectedGameObject(prevMainSelection);
                mainPanel.SetDisabled(false);
                addlPanel.Close();
                addlPanel = null;
            }
        }

        public bool IsOpen()
        {
            return open;
        }

        // ========================================================== Methods called by menu buttons when they're pressed, via the event system

        public void BNewGame()
        {
            confirm = CCNewGame;
            confirmationText.text = "Are you sure you want to start a new game?";
            OpenAdditional(confirmationPanel);
        }

        public void BResume()
        {
            Close();
        }

        public void BReturnToCheckpoint()
        {
            confirm = CCReturnToCheckpoint;
            confirmationText.text = "Are you sure you want to return to the last checkpoint?";
            OpenAdditional(confirmationPanel);
        }

        public void BReturnToHub()
        {
            confirm = CCReturnToHub;
            confirmationText.text = "Are you sure you want to return to the Hub?";
            OpenAdditional(confirmationPanel);
        }

        public void BOptions()
        {
            OpenAdditional(optionsPanel);
        }

        public void BCredits()
        {
            OpenAdditional(creditsPanel);
        }

        public void BQuit()
        {
            confirm = CCQuit;
            confirmationText.text = "Are you sure you want to quit?";
            OpenAdditional(confirmationPanel);
        }

        public void BConfirm()
        {
            CloseAdditional();
            confirm();
        }

        public void BDeny()
        {
            CloseAdditional();
            confirm = null;
        }

        public void BOptionsCamHInvert(bool input)
        {
            GameData.camHInvert = input;
        }

        public void BOptionsCamVInvert(bool input)
        {
            GameData.camVInvert = input;
        }

        public void BOptionsDone()
        {
            CloseAdditional();
        }

        public void BCreditsDone()
        {
            CloseAdditional();
        }

        // ========================================================== Methods that actually do the things requiring confirmation (delegates point at these)

        public void CCNewGame()
        {
            Close();
            game.coordinator.GoToScene("Tutorial", true);
        }

        public void CCReturnToCheckpoint()
        {
            Close();
            game.coordinator.RespawnInScene();
        }

        public void CCReturnToHub()
        {
            Close();
            game.coordinator.GoToScene("", true);
        }

        public void CCQuit()
        {
            // TODO: SAVE
            Application.Quit();
            Debug.Break(); // remove
        }
    }
}