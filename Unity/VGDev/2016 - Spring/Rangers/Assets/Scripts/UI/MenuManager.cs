using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.Data;
using Assets.Scripts.Util;
using Assets.Scripts.UI.Profiles;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager instance;

        public static Enums.UIStates state = Enums.UIStates.Splash;
		private Enums.UIStates prevState = Enums.UIStates.None;

		/// <summary> The last object that was selected before the Navigator was set to uninteractable. </summary>
		private GameObject prevSelected;

        private float hTimer, vTimer, delay = 0.3f;

        private Transform activePanel;
		private Transform prevPanel;

        private bool dpadPressed;

		private ValueModifierUI currentValueMod;

		public GameObject menuTitle;

        [SerializeField]
		private Transform SplashPanel = null, MainPanel = null, SignInPanel = null, LevelSelectPanel = null, SinglePanel = null, MultiPanel = null, SettingPanel = null, AudioPanel = null, VideoPanel = null, PlayerPanel = null, ArenaStandardPanel = null;

		[SerializeField]
		private Transform TargetLevelSelectPanel = null;

		/// <summary> The panel displaying player names. </summary>
		[SerializeField]
		[Tooltip("The panel displaying player names.")]
		private MainMenuPlayerTabsController TabsPanel = null;

        
        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                UpdatePanels(SplashPanel);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
				
        }

		void OnEnable() {
			if(ControllerManager.instance.NumPlayers > 0) {
				state = Enums.UIStates.Main;
				UpdatePanels(MainPanel);
				PlayerPanel.gameObject.SetActive(true);
				PlayerPanel.SetAsFirstSibling();
				menuTitle.SetActive(true);
            }
		}

        void Update()
        {
            switch(state)
            {
                case Enums.UIStates.Splash:
                    Splash();
                    break;
                case Enums.UIStates.Main:
                    Main();
                    break;
				case Enums.UIStates.Signin:
					SignIn();
					break;
                case Enums.UIStates.SinglePlayer:
                    SinglePlayer();
                    break;
                case Enums.UIStates.Multiplayer:
                    Multiplayer();
                    break;
                case Enums.UIStates.Settings:
                    Settings();
                    break;
                case Enums.UIStates.Audio:
                    Audio();
                    break;
                case Enums.UIStates.Video:
                    Video();
                    break;
				case Enums.UIStates.ArenaStandard:
					ArenaStandardMatch();
					break;
				case Enums.UIStates.ValueModifier:
					ValueModifier();
					break;
				case Enums.UIStates.LevelSelect:
					LevelSelect();
					break;
				case Enums.UIStates.TargetLevelSelect:
					TargetLevelSelect();
					break;
                case Enums.UIStates.None:
                    break;
            }
        }

        private void Splash()
        {
			ControllerManager.instance.AddPlayer(ControllerInputWrapper.Buttons.Start);
			if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.One))
            {
				state = Enums.UIStates.Signin;
				UpdatePanels(SignInPanel);
				SFXManager.instance.PlayAffirm();
            }
			if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                ExitGame();
            }
        }

        private void Main()
        {
            Navigate(); 
        }

		private void SignIn()
		{
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
			{
				SignInPanel.FindChild("NameCreator").FindChild("LetterHolder").GetComponent<NameCreator>().Reset();
				state = Enums.UIStates.Splash;
				UpdatePanels(SplashPanel);
				SFXManager.instance.PlayNegative();
				ControllerManager.instance.AllowPlayerRemoval(ControllerInputWrapper.Buttons.B);
			}
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start, PlayerID.One))
			{
				NameCreator nameCreator = SignInPanel.FindChild("NameCreator").FindChild("LetterHolder").GetComponent<NameCreator>();
				string text = nameCreator.t.text;
				if(text.Length > 0 && text.ToCharArray()[text.Length-1] != ' ') {
					nameCreator.Reset();
					ProfileData pd = new ProfileData(text);
					ProfileManager.instance.AddProfile(pd);
					SignInToMain();
					SFXManager.instance.PlayAffirm();
				}
			}
		}

        private void SinglePlayer()
        {
            Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                state = Enums.UIStates.Main;
                UpdatePanels(MainPanel);
				SFXManager.instance.PlayNegative();
            }
        }

        private void Multiplayer()
        {
            Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                state = Enums.UIStates.Main;
                UpdatePanels(MainPanel);
				SFXManager.instance.PlayNegative();
            }
        }

		private void ArenaStandardMatch()
		{
			Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
			{
				state = Enums.UIStates.Main;
				UpdatePanels(MainPanel);
				SFXManager.instance.PlayNegative();
			}
		}

        private void Settings()
        {
            Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                state = Enums.UIStates.Main;
                UpdatePanels(MainPanel);
				SFXManager.instance.PlayNegative();
            }
        }

        private void Audio()
        {
            Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                state = Enums.UIStates.Settings;
                UpdatePanels(SettingPanel);
				SFXManager.instance.PlayNegative();
            }
        }

        private void Video()
        {
            Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
            {
                state = Enums.UIStates.Settings;
                UpdatePanels(SettingPanel);
				SFXManager.instance.PlayNegative();
            }
        }

		private void LevelSelect() {
			Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
			{
				state = prevState;
				UpdatePanels(prevPanel);
				SFXManager.instance.PlayNegative();
			}
		}

		private void TargetLevelSelect() {
			Navigate();
			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One))
			{
				state = Enums.UIStates.Main;
				UpdatePanels(MainPanel);
				SFXManager.instance.PlayNegative();
			}
		}

		public void GoToGame(MapSelector selection) {
			string selectedMap = selection.arenaSelector ? ((Enums.BattleStages)selection.currentSelectedMap).ToString() : ((Enums.TargetPracticeStages)selection.currentSelectedMap).ToString();
			GameManager.lastLoadedLevel = selectedMap;
			if(ProfileManager.instance.NumSignedIn() > 1) SceneManager.LoadScene(selectedMap, LoadSceneMode.Single);
		}

        public void GoToCredits()
        {
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);
        }

		private void ValueModifier() {
			if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY, PlayerID.One) > ControllerManager.CUSTOM_DEADZONE
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY, PlayerID.One) > 0
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, PlayerID.One) > ControllerManager.CUSTOM_DEADZONE
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX, PlayerID.One) > 0) {
				if (vTimer >= delay || vTimer == 0)
				{
					currentValueMod.IncrementValue();
					vTimer = 0;
					SFXManager.instance.PlayClick();
				}
				vTimer += Time.deltaTime;
			} else if(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY, PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY, PlayerID.One) < 0
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE
				|| ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX, PlayerID.One) < 0) {
				if (vTimer >= delay || vTimer == 0)
				{
					currentValueMod.DecrementValue();
					vTimer = 0;
					SFXManager.instance.PlayClick();
				}
				vTimer += Time.deltaTime;
			} else {
				vTimer = 0;
			}

			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.B, PlayerID.One)
				|| ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, PlayerID.One))
			{
				state = prevState;
				currentValueMod.GetComponent<Selectable>().interactable = true;
				Navigator.CallCancel();
				EventSystem.current.SetSelectedGameObject(prevSelected);
			}
		}

		public void ValueModifierEntered(ValueModifierUI val) {
			prevState = state;
			prevSelected = EventSystem.current.currentSelectedGameObject;
			state = Enums.UIStates.ValueModifier;
			currentValueMod = val;
			currentValueMod.GetComponent<Selectable>().interactable = false;
		}

        private void None()
        {

        }

        private void HideAllPanels()
        {
            state = Enums.UIStates.None;
            SplashPanel.gameObject.SetActive(false);
            MainPanel.gameObject.SetActive(false);
            SinglePanel.gameObject.SetActive(false);
            MultiPanel.gameObject.SetActive(false);
            SettingPanel.gameObject.SetActive(false);
            AudioPanel.gameObject.SetActive(false);
            VideoPanel.gameObject.SetActive(false);
			ArenaStandardPanel.gameObject.SetActive(false);
        }

        private void Navigate()
        {
			if (PlayerOneOccupied())
			{
				return;
			}
            // No axis is being pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) == 0)
            {
                // Reset the timer so that we don't continue scrolling
                hTimer = 0;
            }
            // Horizontal joystick is held right
            // Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) > ControllerManager.CUSTOM_DEADZONE)
            {
                // If we can move and it is time to move
                if (hTimer >= delay)
                {
                    // Move and reset timer
                    Navigator.Navigate(Enums.MenuDirections.Right);
                    hTimer = 0;
                }
                hTimer += Time.deltaTime;
            }
            // Horizontal joystick is held left
            // Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX,PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE)
            {
                // If we can move and it is time to move
                if (hTimer >= delay)
                {
                    // Move and reset timer
                    Navigator.Navigate(Enums.MenuDirections.Left);
                    hTimer = 0;
                }
                hTimer += Time.deltaTime;
            }

            // No axis is being pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) == 0)
            {
                // Reset the timer so that we don't continue scrolling
                vTimer = 0;
            }
            // Horizontal joystick is held right
            // Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) > ControllerManager.CUSTOM_DEADZONE)
            {
                // If we can move and it is time to move
                if (vTimer >= delay)
                {
                    // Move and reset timer
                    Navigator.Navigate(Enums.MenuDirections.Up);
                    vTimer = 0;
                }
                vTimer += Time.deltaTime;
            }
            // Horizontal joystick is held left
            // Use > 0.5f so that sensitivity is not too high
			else if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,PlayerID.One) < -ControllerManager.CUSTOM_DEADZONE)
            {
                // If we can move and it is time to move
                if (vTimer >= delay)
                {
                    // Move and reset timer
                    Navigator.Navigate(Enums.MenuDirections.Down);
                    vTimer = 0;
                }
                vTimer += Time.deltaTime;
            }

            // Have dpad functionality so that player can have precise control and joystick quick navigation
            // Check differently for Windows vs OSX
            
            // No dpad button is pressed
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) == 0 && (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) == 0)) dpadPressed = false;
            // Dpad right is pressed; treating as DPADRightOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) > 0 && !dpadPressed)
            {
                dpadPressed = true;
                Navigator.Navigate(Enums.MenuDirections.Right);
            }
            // Dpad right is pressed; treating as DPADLeftOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadX,PlayerID.One) < 0 && !dpadPressed)
            {
                dpadPressed = true;
                Navigator.Navigate(Enums.MenuDirections.Left);
            }
            // Dpad up is pressed; treating as DPADUpOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) > 0 && !dpadPressed)
            {
                dpadPressed = true;
                Navigator.Navigate(Enums.MenuDirections.Up);
            }
            // Dpad down is pressed; treating as DPADDownOnDown
			if (ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.DPadY,PlayerID.One) < 0 && !dpadPressed)
            {
                dpadPressed = true;
                Navigator.Navigate(Enums.MenuDirections.Down);
            }
            

			if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.A, PlayerID.One)) Navigator.CallSubmit();
        }

		public void SignInToMain()
		{
			state = Enums.UIStates.Main;
			UpdatePanels(MainPanel);
			PlayerPanel.gameObject.SetActive(true);
			PlayerPanel.SetAsFirstSibling();
			menuTitle.SetActive(true);
		}


        public void CallSinglePlayer()
        {
            state = Enums.UIStates.SinglePlayer;
            UpdatePanels(SinglePanel);
        }

		public void CallLevelSelect(MatchDesigner match)
		{
			GameSettings settings = match.GetSettings();
			SaveManager.SaveGameSettings(settings,"Current.dat");
			prevState = state;
			prevPanel = activePanel;
			state = Enums.UIStates.LevelSelect;
			UpdatePanels(LevelSelectPanel);
		}

		public void CallMain()
		{
			state = Enums.UIStates.Main;
			UpdatePanels(MainPanel);
		}

        public void CallMultiPlayer()
        {
            state = Enums.UIStates.Multiplayer;
            UpdatePanels(MultiPanel);
        }

        public void CallSettings()
        {
            state = Enums.UIStates.Settings;
            UpdatePanels(SettingPanel);
        }

		public void CallSplash()
		{
			state = Enums.UIStates.Splash;
			PlayerPanel.gameObject.SetActive(false);
			menuTitle.SetActive(false);
			UpdatePanels(SplashPanel);
		}

        public void CallAudio()
        {
            state = Enums.UIStates.Audio;
            UpdatePanels(AudioPanel);
        }

        public void CallVideo()
        {
            state = Enums.UIStates.Video;
            UpdatePanels(VideoPanel);
        }

		public void CallArenaStandard()
		{
			state = Enums.UIStates.ArenaStandard;
			UpdatePanels(ArenaStandardPanel);
		}

		public void CallTargetLevelSelect()
		{
			state = Enums.UIStates.TargetLevelSelect;
			UpdatePanels(TargetLevelSelectPanel);
		}

        public void ExitGame()
        {
             if(!Application.platform.ToString().Contains("Web")) Application.Quit();
        }

        private void UpdatePanels(Transform panel)
        {
            if (panel)
            {
				panel.gameObject.SetActive(true);
                panel.SetAsLastSibling();
            }
            GameObject defaultButton = panel.GetComponent<MenuOption>().DefaultButton;
            if (defaultButton != null && EventSystem.current !=  null)
            {
                EventSystem.current.SetSelectedGameObject(defaultButton);
                Navigator.defaultGameObject = defaultButton;
            }
			if (activePanel) activePanel.gameObject.SetActive(false);
            activePanel = panel;
        }

		/// <summary>
		/// Checks if player one is occupied in a tab menu.
		/// </summary>
		/// <returns>Whether player one is occupied in a tab menu.</returns>
		private bool PlayerOneOccupied()
		{
			MainMenuPlayerInfoBlock block = TabsPanel.GetBlock(0);
			return block == null ? false : block.Occupied();
		}
    }
}
