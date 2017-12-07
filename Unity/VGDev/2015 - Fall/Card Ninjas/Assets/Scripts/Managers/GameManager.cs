using UnityEngine;
using Assets.Scripts.Util;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        private static Enums.GameStates state;
        private static Enums.GameStates prevState;
        private static float musicVol, sfxVol;
        private static bool player1Win, player1Lose;

        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
                state = Enums.GameStates.CardSelection;
                prevState = Enums.GameStates.CardSelection;
                musicVol = .5f;
                sfxVol = .5f;
                player1Win = false;
                player1Lose = false;
            }
            else if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }

        private void CardSelectorStateEnable()
        {
            state = Enums.GameStates.CardSelection;
        }
        private void CardSelectorStateDisable()
        {
            state = Enums.GameStates.Battle;
        }

        public static Enums.GameStates State
        {
            get { return state; }
        }

        void OnEnable()
        {
            UI.CardSelector.CardSelectorEnabled += CardSelectorStateEnable;
            UI.CardSelector.CardSelectorDisabled += CardSelectorStateDisable;
            UI.CardSelectorMultiplayer.CardSelectorEnabled += CardSelectorStateEnable;
            UI.CardSelectorMultiplayer.CardSelectorDisabled += CardSelectorStateDisable;
        }
        void OnDisable()
        {
            UI.CardSelector.CardSelectorEnabled -= CardSelectorStateEnable;
            UI.CardSelector.CardSelectorDisabled -= CardSelectorStateDisable;
            UI.CardSelectorMultiplayer.CardSelectorEnabled -= CardSelectorStateEnable;
            UI.CardSelectorMultiplayer.CardSelectorDisabled -= CardSelectorStateDisable;
        }

        public static bool Pause
        {
            get { return state == Enums.GameStates.Paused; }
            set
            {
                if (value && state != Enums.GameStates.Paused)
                {
                    prevState = state;
                    state = Enums.GameStates.Paused;
                }
                else
                    state = prevState;
            }
        }

		public static bool CardSelect
		{
			get { return state == Enums.GameStates.CardSelection; }
			set
			{
				if (value)
				{
					prevState = state;
					state = Enums.GameStates.CardSelection;
				}
				else
					state = prevState;
			}
		}

        public static float MusicVol
        {
            get { return musicVol; }
            set
            {
                musicVol = value;
                Util.SoundPlayer[] sounds = FindObjectsOfType<Util.SoundPlayer>() as Util.SoundPlayer[];
                foreach (Util.SoundPlayer s in sounds)
                    if (!s.SFX)
                        s.SetVolume(musicVol);
            }
        }

        public static float SFXVol
        {
            get { return musicVol; }
            set
            {
                sfxVol = value;
                Util.SoundPlayer[] sounds = FindObjectsOfType<Util.SoundPlayer>() as Util.SoundPlayer[];
                foreach (Util.SoundPlayer s in sounds)
                    if (s.SFX)
                        s.SetVolume(sfxVol);
            }
        }

        public static bool Player1Win
        {
            get { return player1Win; }
            set
            {
                player1Win = value;
                state = Enums.GameStates.Paused;
            }
        }

        public static bool Player1Lose
        {
            get { return player1Lose; }
            set
            {
                player1Lose = value;
                state = Enums.GameStates.Paused;
            }
        }
    }
}
