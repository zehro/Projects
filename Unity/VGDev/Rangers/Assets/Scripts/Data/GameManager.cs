using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Assets.Scripts.Level;
using Assets.Scripts.Player;
using Assets.Scripts.Timers;
using Assets.Scripts.Util;

namespace Assets.Scripts.Data
{
    /// <summary>
    /// Manager to control everything as the top of the pyramid
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public string settingsName = "Stock.dat";
        /// <summary>
        /// Use a singleton instance to make sure there is only one
        /// </summary>
        public static GameManager instance;

		public static string lastLoadedLevel;

        // List of all the controllers of the players
        private List<Controller> controllers;

		//List of all spawnpoints, collected on Start()
		private List<GameObject> spawnPoints;

        // List of all the types of tokens available
        [SerializeField]
        private List<GameObject> allTokens;
        private Dictionary<Enums.Tokens, Enums.Frequency> tokens;

        [SerializeField]
        private GameObject playerPrefab;

		[SerializeField]
		private GameObject aiPlayerPrefab;

        // Match timer (optional)
        private Timer matchTimer;

		//GameOver UI
		private GameObject gameOverUI;
		private bool gameOver;
		private PlayerID currentWinner;

        // Current game settings to abide by
		private GameSettings currentGameSettings;
		private TargetSettings currentTargetGameSettings;
        // CustomColor object for efficient color getting
        //private CustomColor customColor;

        private int numDead = 0;

		private bool paused = false, recentPause;
		public float deltaTime = 0f;
		private List<Vector3> pausedVelocities;

		public float countInTimer = 5f;

        // Sets up singleton instance. Will remain if one does not already exist in scene
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            controllers = new List<Controller>();
			spawnPoints = new List<GameObject>();
        }

        void Start()
        {
            // Call Init
			gameOverUI = GameObject.Find("GameOverUI");
			gameOverUI.SetActive(false);
			pausedVelocities = new List<Vector3>();
        }

		void Update()
		{
			if(countInTimer > 0) {
				if(matchTimer) matchTimer.On = false;
				countInTimer -= Time.deltaTime;
			} else if(countInTimer != -100 && matchTimer && !matchTimer.On) {
				matchTimer.On = true;
				countInTimer = -100f;
			}

			if(!GameFinished && countInTimer <= 0 && ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start)) {
				paused = !paused;
				recentPause = true;
			}
			if(!GameFinished && paused) {
				if(recentPause) {
					for(int i = 0; i < controllers.Count; i++) {
						pausedVelocities.Add(controllers[i].GetComponent<Rigidbody>().velocity);
						controllers[i].GetComponent<Rigidbody>().velocity = Vector3.zero;
						controllers[i].GetComponent<Rigidbody>().useGravity = false;
					}
					deltaTime = 0;
					if(matchTimer) matchTimer.On = false;
					recentPause = false;
				}
			} else if(!GameFinished) {
				if(recentPause) {
					for(int i = 0; i < controllers.Count; i++) {
						controllers[i].GetComponent<Rigidbody>().velocity = pausedVelocities[i];
						controllers[i].GetComponent<Rigidbody>().useGravity = true;
					}
					pausedVelocities.Clear();
					if(matchTimer) matchTimer.On = true;
					deltaTime = Time.deltaTime;
					recentPause = false;
				}
			}
		}

		void OnLevelWasLoaded(int level)
		{
			// Reinitialize when restarting a match.
			controllers = new List<Controller>();
			numDead = 0;
			InitializeMatch();
		}

        /// <summary>
        /// Initializes the match when one is started
        /// </summary>
        private void InitializeMatch()
        {
            // Find all the players
            Controller[] findControllers = FindObjectsOfType<Controller>();
            for (int i = 0; i < findControllers.Length; i++)
            {
                controllers.Add(findControllers[i]);
            }
			// Load the last settings used
			if(settingsName.Contains("Target"))
				currentTargetGameSettings = LoadManager.LoadTargetSettingsXML(settingsName);
			else
				currentGameSettings = LoadManager.LoadGameSettings(settingsName);

			//Find the spawnpoints
			spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("Respawn"));

			StatisticManager.instance.statistics = new Dictionary<PlayerID, Statistic>();

			//If there aren't already players in this scene, we need to create them
			if(controllers.Count == 0) {
				PlayerID currentID = PlayerID.None;
				for (int i = 0; i < ControllerManager.instance.NumPlayers; i++) {
					currentID = (PlayerID)(i + 1);
					GameObject spawnPrefab = ControllerManager.instance.IsAIController(currentID) ? aiPlayerPrefab : playerPrefab;
					GameObject temp = (GameObject)GameObject.Instantiate(spawnPrefab, spawnPoints[i].transform.position, Quaternion.Euler(0,90,0));
					Controller tempController = temp.GetComponent<Controller>();
					controllers.Add(tempController);
					tempController.ProfileComponent = ProfileManager.instance.GetProfile(currentID);
					tempController.ID = currentID;
					StatisticManager.instance.statistics.Add(currentID, new Statistic());
				}
			}

			for (int i = 0; i < ControllerManager.instance.NumPlayers; i++) {
				for (int j = 0; j < ControllerManager.instance.NumPlayers; j++) {
					if(i != j) {
						StatisticManager.instance.statistics[(PlayerID)(i+1)].killedPlayer.Add((PlayerID)(j+1),0);
						StatisticManager.instance.statistics[(PlayerID)(i+1)].killedByPlayer.Add((PlayerID)(j+1),0);
					}
				}
			}

            // Initialize the tokens
            TokenSpawner.instance.Init(currentGameSettings.EnabledTokens);

            for (int i = 0; i < controllers.Count; i++)
            {
                controllers[i].LifeComponent.Lives = currentGameSettings.StockLimit;
            }

            // Set up a timer that counts up for targets
			if (currentTargetGameSettings != null)
            {
				currentTargetGameSettings.TargetsInLevel = FindObjectsOfType<Target>().Length;
                matchTimer = gameObject.AddComponent<Timer>();
                matchTimer.Initialize(Mathf.Infinity, "Match Timer");
            }
            // All other games will have a countdown timer
            else
            {
                // If the timer is enabled in that game type
				if (currentGameSettings.TimeLimit > 0)
                {
					float timeLimit = currentGameSettings.TimeLimit == 0 ? Mathf.Infinity : currentGameSettings.TimeLimit;
					matchTimer = CountdownTimer.CreateTimer(gameObject, timeLimit, "Match Timer", TimeUp);
                }
            }
        }
        
        /// <summary>
        /// Timer that will end the match
        /// </summary>
        /// <param name="t">The timer that timed out</param>
        private void TimeUp(CountdownTimer t)
        {
			GameOver();
        }

        /// <summary>
        /// Handles a target being destroyed and checks to see if the game is over
        /// </summary>
        /// <param name="fromPlayer">The player that hit the target</param>
        public void TargetDestroyed(PlayerID fromPlayer)
        {
            if (--currentGameSettings.TargetsInLevel <= 0) GameOver();
        }

        /// <summary>
        /// Handles what happens when the game is over
        /// </summary>
		public void GameOver()
        {
            //Debug.Log("Match concluded");
            if (matchTimer != null) matchTimer.On = false;
			gameOverUI.SetActive(true);
			gameOver = true;
			foreach(Controller c in controllers) {
				c.Disable();
			}
			CountdownTimer.CreateTimer(gameObject, 10f, "GameOver", GoToMatchSummary);

        }

		private void GoToMatchSummary(CountdownTimer t)
		{
			GameObject g = new GameObject("MatchSummaryManager");
			MatchSummaryManager summary = g.AddComponent<MatchSummaryManager>();
			DontDestroyOnLoad(g);
			MatchSummaryManager.winner = currentWinner;
			MatchSummaryManager.playerInfo = new Dictionary<PlayerID, int>();
			if(currentGameSettings.Type == Enums.GameType.Deathmatch) {
				for(int i = 0; i < controllers.Count; i++) {
					MatchSummaryManager.playerInfo.Add(controllers[i].ID,controllers[i].LifeComponent.kills);
				}
			} else {
				for(int i = 0; i < controllers.Count; i++) {
					MatchSummaryManager.playerInfo.Add(controllers[i].ID,(int)controllers[i].LifeComponent.Lives);
				}
			}
			SceneManager.LoadScene("MatchSummary", LoadSceneMode.Single);
		}

        /// <summary>
        /// Resets the scene.
        /// </summary>
        private void ResetLevel(CountdownTimer t)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// What happens when a player is killed.
        /// </summary>
        /// <param name="killed">The id of the player that is killed</param>
        /// <param name="killedBy">The id of the player that is the killer</param>
        public void PlayerKilled(PlayerID killed, PlayerID killedBy = PlayerID.None)
        {
            // Find those two players
            Controller victim = controllers.Find(x => x.ID.Equals(killed));
            Controller killer = controllers.Find(x => x.ID.Equals(killedBy));
            
            // Check if the game is over based on gametype
            if(currentGameSettings.Type.Equals(Enums.GameType.Deathmatch))
            {
				// Increment killer score provided there is a killer that is not self
				if(killer == victim)
				{
					killer.LifeComponent.kills--;
					StatisticManager.instance.statistics[killer.ID].suicides++;
				} else if (killer != null) {
					killer.LifeComponent.kills++;

					if(StatisticManager.instance.statistics[killer.ID].killedPlayer.ContainsKey(victim.ID))
						StatisticManager.instance.statistics[killer.ID].killedPlayer[victim.ID]++;
					else
						StatisticManager.instance.statistics[killer.ID].killedPlayer.Add(victim.ID,1);

					if(StatisticManager.instance.statistics[victim.ID].killedByPlayer.ContainsKey(killer.ID))
						StatisticManager.instance.statistics[victim.ID].killedByPlayer[killer.ID]++;
					else
						StatisticManager.instance.statistics[victim.ID].killedByPlayer.Add(killer.ID,1);
				}

				// find the most kills
				float maxNumKills = Mathf.NegativeInfinity;
				PlayerID mostKills = PlayerID.None;

				//iterate through controllers
				foreach(Controller c in controllers) {
					//if this controller has the most kills so far
					if(c.LifeComponent.kills > maxNumKills) {
						maxNumKills = c.LifeComponent.kills;
						mostKills = c.ID;
					} else if(c.LifeComponent.kills == maxNumKills) { //if this controller has the same number of kills
						if (mostKills != PlayerID.None) {
							if (c.LifeComponent.Deaths < GetPlayer(mostKills).LifeComponent.Deaths) {
								mostKills = c.ID;
							} else if (c.LifeComponent.Deaths == GetPlayer(mostKills).LifeComponent.Deaths) {
								mostKills = PlayerID.None;
							}
						} else {
							PlayerID winningID = PlayerID.None;
							float leastDeaths = Mathf.Infinity;
							foreach (Controller controller in controllers) {
								if (controller.LifeComponent.kills == maxNumKills) {
									if (controller.LifeComponent.Deaths < leastDeaths) {
										winningID = controller.ID;
										leastDeaths = controller.LifeComponent.Deaths;
									} else if (controller.LifeComponent.Deaths == leastDeaths) {
										winningID = PlayerID.None;
									}
								}
							}
							mostKills = winningID;
						}
					}
				}
				currentWinner = mostKills;
                if (killer.LifeComponent.kills >= currentGameSettings.KillLimit) GameOver();
            }
            if (currentGameSettings.Type.Equals(Enums.GameType.Stock))
            {
                if (victim.LifeComponent.Lives <= 0) numDead++;

				int maxLives = -99999999;
				PlayerID mostLives = PlayerID.None;
				foreach(Controller c in controllers) {
					if(c.LifeComponent.Lives > maxLives) {
						maxLives = (int)c.LifeComponent.Lives;
						mostLives = c.ID;
					} else if (c.LifeComponent.Lives == maxLives) {
						mostLives = PlayerID.None;
					}
				}
				currentWinner = mostLives;
                if (numDead >= controllers.Count - 1) GameOver();
            }
        }

        /// <summary>
        /// Sets up the player so it can respawn.
        /// </summary>
        /// <param name="id">The ID of the player that died</param>
        public void Respawn(PlayerID id)
        {
            // Find the dead player
            Controller deadPlayer = controllers.Find(x => x.ID.Equals(id));
            if(deadPlayer != null)
            {
                // Initialize the respawn timer
                CountdownTimer t = gameObject.AddComponent<CountdownTimer>();
                t.Initialize(3f, deadPlayer.ID.ToString());
                t.TimeOut += new CountdownTimer.TimerEvent(RespawnHelper);
            }
        }

        /// <summary>
        /// Target for the respawn timer to run on timeout.
        /// </summary>
        /// <param name="t">The timer that is firing the event</param>
        private void RespawnHelper(CountdownTimer t)
        {
            // Find the dead player again
            Controller deadPlayer = controllers.Find(x => x.ID.Equals(System.Enum.Parse(typeof(PlayerID), t.ID)));
            if (deadPlayer != null)
            {
                // Find an appropriate spawning pod (set to default for now)
				deadPlayer.transform.position = spawnPoints[Random.Range(0,spawnPoints.Count)].transform.position;
                // Let the player revive itself
                deadPlayer.LifeComponent.Respawn();
            }
        }

        /// <summary>
        /// Creates a player when signing in and puts it in the list of active players
        /// </summary>
        /// <param name="id">ID of the player to create (the ID of the controller controlling it)</param>
        public void InitializePlayer(PlayerID id)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            Controller controller = newPlayer.GetComponent<Controller>();
            controller.ID = id;
            controllers.Add(controller);
            controller.Disable();
        }

        /// <summary>
        /// Remove the player from the list of active players
        /// </summary>
        /// <param name="id">ID of the player to deactivate from the game</param>
        public void RemovePlayer(PlayerID id)
        {
            Controller removePlayer = controllers.Find(x => x.ID.Equals(id));
            controllers.Remove(removePlayer);
        }

		public Controller GetPlayer(PlayerID id) {
			Controller p = controllers.Find(x => x.ID.Equals(id));
			return p;
		}

        #region C# Properties
        /// <summary>
        /// List of all the tokens prefabs
        /// </summary>
        public List<GameObject> AllTokens
        {
            get { return allTokens; }
        }
        /// <summary>
        /// Current game settings to check rules against
        /// </summary>
        public GameSettings CurrentGameSettings
        {
            get { return currentGameSettings; }
            set { currentGameSettings = value; }
        }
        /// <summary>
        /// All the players in the current game
        /// </summary>
        public List<Controller> AllPlayers
        {
            get { return controllers; }
        }

		public float CurrentTime
		{
			get {
				if(matchTimer) return matchTimer.CurrentTime;
				return -1f;
			}
		}

		public bool GameFinished
		{
			get {return gameOver || countInTimer > 0; }
		}

		public PlayerID CurrentWinner
		{
			get {return currentWinner; }
		}

		public bool IsPaused
		{
			get { return paused; }
			set { paused = value; }
		}
        #endregion
    }
}
