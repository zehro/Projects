using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Timers;
using Assets.Scripts.Tokens;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// This class wil manage all the player's (and enemy's) components,
    /// such as movement, data , etc
    /// Will also allow the different components to talk to one another
    /// </summary>
    public class Controller : MonoBehaviour
    {
        // ID for identifying which player is accepting input
        [SerializeField]
        protected PlayerID id;

        // List of timers for checking effects
        protected List<Timer> effectTimers;

        // Distance from firePoint to player
        protected float distanceToPlayer = 1.5f;

        // Components to manage
        protected Parkour parkour;
        protected Life life;
        protected Archery archery;
		protected ProfileData profile;

        public const int INVINCIBLE_FRAMES = 2;
        protected int invincibleFrames = INVINCIBLE_FRAMES;

		//Body parts for fun destruction
		private List<RobotBodyPart> bodyParts;

		public List<Material> playerMats;
		public bool justDamaged;

        void Awake()
        {
            // Initialize all componenets
            InitializePlayerComponents();
        }

        protected void Start()
        {
            // Initialize the effect timers list
            effectTimers = new List<Timer>();

			//Find all the body parts
			bodyParts = new List<RobotBodyPart>();
			bodyParts.AddRange(Resources.FindObjectsOfTypeAll<RobotBodyPart>());
			bodyParts.RemoveAll((RobotBodyPart obj) => obj == null || obj.pid != id);

			if(profile != null) {
				foreach(RobotBodyPart rbp in bodyParts) {
					if(rbp.GetComponent<MeshRenderer>().material.name.Equals("PlayerMat1 (Instance)")) {
						rbp.GetComponent<MeshRenderer>().material.color = profile.PrimaryColor;
						rbp.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.PrimaryColor.r/3f,profile.PrimaryColor.g/3f, profile.PrimaryColor.b/3f));
					} else {
						rbp.GetComponent<MeshRenderer>().material.color = profile.SecondaryColor;
						rbp.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.SecondaryColor.r/3f,profile.SecondaryColor.g/3f, profile.SecondaryColor.b/3f));
					}
					playerMats.Add(rbp.GetComponent<MeshRenderer>().material);
				}
			}
        }

        /// <summary>
        /// Assigning references
        /// </summary>
        protected virtual void InitializePlayerComponents()
        {
            // Add all components to manage
            life = GetComponent<Life>();
            parkour = GetComponent<Parkour>();
            archery = GetComponent<Archery>();

            // Tell all components this is their controller
            life.Controller = this;
            parkour.Controller = this;
            archery.Controller = this;
        }

		protected void Update()
		{
			if (transform.position.y < -30) 
			{
				LifeComponent.ModifyHealth(-100);
			}
			if(!justDamaged) {
				if(playerMats[0].GetColor("_EmissionColor") == Color.red) {
					for(int i = 0; i < playerMats.Count; i++) {
						if(playerMats[i].name.Equals("PlayerMat1 (Instance)")) {
							playerMats[i].SetColor("_EmissionColor", new Color(profile.PrimaryColor.r/3f,profile.PrimaryColor.g/3f, profile.PrimaryColor.b/3f));
						} else {
							playerMats[i].SetColor("_EmissionColor", new Color(profile.SecondaryColor.r/3f,profile.SecondaryColor.g/3f, profile.SecondaryColor.b/3f));
						}
					}
				}
			}
			if(justDamaged) justDamaged = false;
		}

        /// <summary>
        /// Disables the player
        /// </summary>
        public void Disable()
        {
			foreach(RobotBodyPart rbp in bodyParts) {
				rbp.DestroyBody();
			}
			GetComponent<Rigidbody>().detectCollisions = false;
			GetComponent<Rigidbody>().isKinematic = true;
			GetComponent<Animator>().enabled = false;
        }

        /// <summary>
        /// Reenables the player
        /// </summary>
        public void Enable()
        {
			GetComponent<Rigidbody>().detectCollisions = true;
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<Animator>().enabled = true;
			foreach(RobotBodyPart rbp in bodyParts) {
				rbp.RespawnBody();
			}
		}

		protected void GrabToken()
		{
			if (!ArcheryComponent.CanCollectToken()) return;
			Collider[] cols = Physics.OverlapSphere(transform.position + new Vector3(0, 0.75f, 0), 1f);
			for(int i = 0; i < cols.Length; i++)
			{
				ArrowToken t = cols[i].GetComponent<ArrowToken>();
				if(t != null)
				{
					if (!Util.Bitwise.IsBitOn(ArcheryComponent.ArrowTypes, (int)t.Type))
					{
						t.TokenCollected(this);
						return;
					}
				}
			}
		}

		#region Virtual Methods
		/// <summary>
		/// Checks if the controller is holding the jump button.
		/// </summary>
		/// <returns>Whether the controller is holding the jump button.</returns>
		internal virtual bool IsHoldingJump()
		{
			return false;
		}

		/// <summary>
		/// Determines whether this instance is holding down on the analog stick.
		/// </summary>
		/// <returns><c>true</c> if this instance is holding down; otherwise, <c>false</c>.</returns>
		internal virtual bool IsHoldingDown()
		{
			return false;
		}
		#endregion

        #region C# Properties
        /// <summary>
        /// Archery component of the player
        /// </summary>
        public Archery ArcheryComponent
        {
            get { return archery; }
        }
        /// <summary>
        /// Life component of the player
        /// </summary>
        public Life LifeComponent
        {
            get { return life; }
        }
        /// <summary>
        /// Parkour component of the player
        /// </summary>
        public Parkour ParkourComponent
        {
            get { return parkour; }
        }
        /// <summary>
        /// Profile component of the player
        /// </summary>
		public ProfileData ProfileComponent
        {
            get { return profile; }
			set { profile = value; }
        }
        /// <summary>
        /// ID of the player
        /// </summary>
        public PlayerID ID
        {
            get { return id; }
            set { id = value; }
        }

        public int InvincibleFrames
        {
            get { return invincibleFrames; }
            set { invincibleFrames = value; }
        }

        public bool Invincible
        {
            get { return invincibleFrames > 0; }
        }
        #endregion
    }
}
