using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.CardSystem;
using Assets.Scripts.Util;

namespace Assets.Scripts.UI
{
	public class LoadingScreen : MonoBehaviour
	{
        public delegate void LoadingAction();
        public static event LoadingAction BeginLoadLevel, FinishedLoading;

		public static LoadingScreen instance;
		private static string levelToLoad = "MenuTest";

		private Transform loadingCard, randomCard, activeCard;
        private const float START_Y_ROT = -90, TURN_AMOUNT = 180;

		private Canvas win;

        [SerializeField]
        private Text cardName = null, type = null, damage = null, range = null, description = null;
        [SerializeField]
        private Image image = null, cardBase = null;
        [SerializeField]
        private SoundPlayer music;
        [SerializeField]
        Camera loadingCamera;

        private float vel, counter, turnStep = 1.5f;

        private List<Card> allCards;

        AsyncOperation async;

        private float timer = 0, timeToChange = 5f;

        private SoundPlayer[] soundsInScene;

		void Awake()
		{
			if(instance == null)
			{
				instance = this;
				DontDestroyOnLoad(this.gameObject);
                DontDestroyOnLoad(loadingCamera.gameObject);
			}
			else if(instance != this)
			{
				Destroy(this.gameObject);
                return;
			}

            win = this.GetComponent<Canvas>();
            loadingCard = GameObject.Find("Loading Card").transform;
            randomCard = GameObject.Find("Random Card").transform;
            randomCard.gameObject.SetActive(false);

            allCards = FindObjectOfType<CardList>().Cards;

            Init();

			LoadLevel(levelToLoad);
		}

        private void Init()
        {
            activeCard = loadingCard;
            activeCard.eulerAngles = new Vector3(0, -START_Y_ROT, 0);
            counter = 0;
        }

        void Update()
        {
            if (async != null)
            {
                LoadInProgress();
            }
        }

        private void LoadInProgress()
        {
            timer += Time.deltaTime;
            activeCard.Rotate(0f, -turnStep, 0f);
            counter += turnStep;
            foreach (SoundPlayer s in soundsInScene)
                if (s != null && s.audio.isPlaying)
                    s.Stop();
            if (counter > TURN_AMOUNT) SwapCards();
            if (timer >= timeToChange && async.progress >= 0.9f) FinishLoading();
        }

        private void SwapCards()
        {
            if(activeCard.Equals(loadingCard))
            {
                randomCard.gameObject.SetActive(true);
                loadingCard.gameObject.SetActive(false);
                activeCard = randomCard;

                Card nextCard = allCards[Random.Range(0, allCards.Count)];

                cardName.text = nextCard.Name;
                type.text = nextCard.Type.ToString();
                damage.text = nextCard.Action.Damage.ToString();
                range.text = nextCard.Action.Range.ToString();
                description.text = nextCard.Description;

                image.sprite = nextCard.Image;
                cardBase.color = CustomColor.ColorFromElement(nextCard.Element);
            }
            else
            {
                randomCard.gameObject.SetActive(false);
                loadingCard.gameObject.SetActive(true);
                activeCard = loadingCard;
            }
            counter = 0;
            activeCard.eulerAngles = new Vector3(0, -START_Y_ROT, 0);
        }

        public void LoadLevel(int level)
        {
            win.enabled = true;
            if (Camera.main != null) Camera.main.enabled = false;
            loadingCamera.enabled = true;
            timer = 0;
            if (BeginLoadLevel != null) BeginLoadLevel();
            async = Application.LoadLevelAsync(level);
            async.allowSceneActivation = false;
            SoundPlayer[] bgms = FindObjectsOfType<SoundPlayer>();
            soundsInScene = new SoundPlayer[bgms.Length - 1];
            int i = 0;
            foreach (SoundPlayer s in bgms)
            {
                if (s != music)
                {
                    s.Stop();
                    soundsInScene[i] = s;
                    i++;
                }
            }
            music.PlaySong(0);
        }

        public void LoadLevel(string level)
		{
			win.enabled = true;
            if (Camera.main != null) Camera.main.enabled = false;
            loadingCamera.enabled = true;
			timer = 0;
            LevelToLoad = level;
            if (BeginLoadLevel != null) BeginLoadLevel();
            async = Application.LoadLevelAsync(level);
            async.allowSceneActivation = false;
            SoundPlayer[] bgms = FindObjectsOfType<SoundPlayer>();
            soundsInScene = new SoundPlayer[bgms.Length - 1];
            int i = 0;
            foreach (SoundPlayer s in bgms)
            {
                if (s != music)
                {
                    s.Stop();
                    soundsInScene[i] = s;
                    i++;
                }
            }
            music.PlaySong(0);
        }

        private void FinishLoading()
        {
            async.allowSceneActivation = true;
            timer = 0;
            if (FinishedLoading != null) FinishedLoading();
        }

        void OnLevelWasLoaded(int level)
        {
            loadingCamera.enabled = false;
            music.Stop();
            win.enabled = false;
        }

		public static string LevelToLoad
		{
			set { levelToLoad = value; }
			get { return levelToLoad; }
		}
	}
}
