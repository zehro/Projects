using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Level;
using Assets.Scripts.Player;
using Assets.Scripts.Util;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Controls the different components of the arrow and activates all the effects upon impact.
    /// </summary>
    public class ArrowController : MonoBehaviour
    {
        // Init and Effect events for the different types of arrows
        private delegate void ArrowEvent();
        private delegate void ArrowHitEvent(PlayerID hitPlayer);
        private event ArrowEvent Init;
        private event ArrowHitEvent Effect;

        // Layers that should not activate the arrow's effects
        [SerializeField]
        private LayerMask doNotActivate;

        // Damage to be dealt when hit by an arrow
        private float damage = 10f;
        // Player IDs for passing along information
        private PlayerID fromPlayer, hitPlayer;
        // For ricochet arrows
        private float bounciness = 0;
        // For tracking arrows
        private float trackingTime = 0f;
        private Controller trackingTarget;

		// To prevent self-shooting immediately after firing
		private float avoidPlayerTimer;

        // Reference to arrowhead
        [SerializeField]
        private Transform arrowhead;

        // Caching the rigidbody, collider, and collision info
        new private Rigidbody rigidbody;
        private Vector3 prevVelocity;
        new private Collider collider;
        private CollisionInfo colInfo;

        // Initialze all necessary components
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
			collider = GetComponentInChildren<Collider>();
            colInfo = GetComponent<CollisionInfo>();
        }

        /// <summary>
        /// Initializes the arrow by adding all the necesary components.
        /// </summary>
        /// <param name="types">The types of arrow components to be added. Comes from the different tokens the player has collected.</param>
        /// <param name="fromPlayer">ID of the player shooting the arrow.</param>
        public void InitArrow(int types, PlayerID fromPlayer)
        {
            // Update the player info
            this.fromPlayer = fromPlayer;
            // Initializing this arrow
            GenerateArrowProperties(types);
            // Call the init event for all arrow componenets
            if (Init != null) Init();
        }

        void Update()
        {
			if(!collider.enabled) {
				avoidPlayerTimer += Time.deltaTime;
				if(avoidPlayerTimer > 0.1f) {
					collider.enabled = true;
				}
			}
            // Rotate the arrow towards the closest enemy if it is tracking
            if (trackingTime > 0)
            {
                // Find closest enemy player
                if (trackingTarget == null || !trackingTarget.GetComponent<Rigidbody>().isKinematic)
                {
                    trackingTarget = null;
                    float minDistance = 0f;
                    foreach (Controller player in GameManager.instance.AllPlayers)
                    {
                        float distance = Vector3.Distance(transform.position, player.transform.position);
                        if (!player.GetComponent<Rigidbody>().isKinematic 
                            && !player.ID.Equals(fromPlayer) && (trackingTarget == null || distance < minDistance))
                        {
                            trackingTarget = player;
                            minDistance = distance;
                        }
                    }
                }
                // Changes arrow direction
                if (trackingTarget != null)
                {
                    Vector3 direction = trackingTarget.transform.position - transform.position + new Vector3(0, 1, 0);
                    direction /= direction.magnitude;
                    float magnitude = rigidbody.velocity.magnitude;
                    // Ceiling used to prevent excess time from accumulating
                    //int iterations = Mathf.CeilToInt((trackingTime + Time.deltaTime) * 10) - Mathf.CeilToInt(trackingTime * 10);
                    //for (int i = 0; i < iterations; i++)
                    //{
                    // Alters direction - Modify last value to change tracking intensity
                    //rigidbody.velocity += direction * Mathf.Pow(magnitude, 2) / 25;
                    rigidbody.AddForce(direction * 200);
                    if (rigidbody.velocity.magnitude > 25) rigidbody.velocity /= magnitude * Time.deltaTime;
                        // Scales magnitude back to original
                        //rigidbody.velocity /= rigidbody.velocity.magnitude / magnitude;
                    //}
                }
                trackingTime -= Time.deltaTime;
                if (trackingTime <= 0)
                {
                    rigidbody.useGravity = true;
                }
            }
            // Point the arrow the direction it is travelling
            if (rigidbody != null && rigidbody.velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(rigidbody.velocity);
                // Cache the previous velocity
                prevVelocity = rigidbody.velocity;
            }
			if (transform.position.y < -30) 
			{
				GameObject.Destroy(gameObject);
			}
        }

        // Arrow hits something
        void OnCollisionEnter(Collision col)
        {
            // Check to see a layer of the object should not activate the effects
            if ((doNotActivate.value & (1 << col.gameObject.layer)) != 0) return;
            // If the arrow his a player
			if (col.transform.root.tag.Equals("Player"))
            {
                // Damage the player hit
                Controller controller = col.transform.GetComponent<Controller>();
				SFXManager.instance.PlayArrowHit();
                controller.LifeComponent.ModifyHealth(-damage, fromPlayer);
                hitPlayer = controller.ID;
            }
			else if(col.transform.root.tag.Equals("Target"))
            {
                col.gameObject.GetComponent<Target>().TargetHit(fromPlayer);
            }
            // Update collision info for arrow components to use
            colInfo.HitPosition = arrowhead.position;
            colInfo.HitRotation = arrowhead.rotation;
            colInfo.IsTrigger = false;

            // Call the effect of each arrow component
            if (Effect != null) Effect(hitPlayer);
            // Check bounciness for ricochet arrows
            if (--bounciness <= 0)
            {
                Destroy(rigidbody);
                Destroy(collider);
				Destroy(transform.FindChild("Model").GetComponent<TrailRenderer>());
				GameObject g = new GameObject();
				transform.position += transform.forward*0.2f;
				transform.parent = g.transform;
				g.transform.parent = col.transform;
				Destroy(g, 1f);
                Destroy(this);
            }
            else
            {
                // Reflect the arrow to bounce off object
                rigidbody.velocity = Vector3.Reflect(prevVelocity, col.contacts[0].normal);
                rigidbody.velocity = Vector3.Scale(rigidbody.velocity, new Vector3(1, 1, 0));
            }
        }

        // Arrow goes through something
        void OnTriggerEnter(Collider col)
        {
            // Check to see a layer of the object should not activate the effects
            if ((doNotActivate.value & (1 << col.gameObject.layer)) != 0) return;
            // If the arrow his a player
			if (col.transform.root.tag.Equals("Player"))
            {
				Controller controller = col.transform.root.GetComponent<Controller>();
				SFXManager.instance.PlayArrowHit();
                controller.LifeComponent.ModifyHealth(-damage, fromPlayer);
                hitPlayer = controller.ID;
				trackingTime = 0f;
            }
			else if (col.transform.root.tag.Equals("Target"))
            {
                col.gameObject.GetComponent<Target>().TargetHit(fromPlayer);
            }
            // Update collision info for arrow components to use
            colInfo.HitPosition = arrowhead.position;
            colInfo.HitRotation = arrowhead.rotation;
            colInfo.IsTrigger = false;

            // Call the effect of each arrow component
            if (Effect != null) Effect(hitPlayer);
        }

        /// <summary>
        /// Add all the arrow types and setup the appropriate delegates.
        /// </summary>
        /// <param name="types">The types of components to initialize.</param>
        private void GenerateArrowProperties(int types)
        {
            // Somehow the arrow is comprised of nothing
            if (types == 0) return;
            for(int i = 0; i < (int)Enums.Arrows.NumTypes; i++)
            {
                // Check to see if the type exists in the current arrow
                if(Bitwise.IsBitOn(types, i))
                {
                    // Add an arrow property and update the delegates
                    ArrowProperty temp = GetArrowProperty((Enums.Arrows)i);
                    temp.Type = (Enums.Arrows)i;
                    temp.FromPlayer = fromPlayer;        
                    Init += temp.Init;
                    Effect += temp.Effect;
                }
            }
        }

        /// Used to add the appropriate script to the gameobject
        private ArrowProperty GetArrowProperty(Enums.Arrows type)
        {
            switch(type)
            {
                case Enums.Arrows.Fireball:
                    return gameObject.AddComponent<FireballArrow>();
                case Enums.Arrows.Ice:
                    return gameObject.AddComponent<IceArrow>();
                case Enums.Arrows.Thunder:
                    return gameObject.AddComponent<ThunderArrow>();
                case Enums.Arrows.Acid:
                    return gameObject.AddComponent<AcidArrow>();
                case Enums.Arrows.Ricochet:
                    bounciness = RicochetArrow.bounces;
                    return gameObject.AddComponent<RicochetArrow>();
                case Enums.Arrows.Ghost:
                    return gameObject.AddComponent<GhostArrow>();
                case Enums.Arrows.ZeroGravity:
                    return gameObject.AddComponent<ZeroGravityArrow>();
				case Enums.Arrows.Lifesteal:
					return gameObject.AddComponent<LifestealArrow>();
                case Enums.Arrows.Tracking:
                    trackingTime = TrackingArrow.trackingTime;
                    return gameObject.AddComponent<TrackingArrow>();
				case Enums.Arrows.Virus:
					return gameObject.AddComponent<VirusArrow>();
				case Enums.Arrows.Splitting:
					return gameObject.AddComponent<SplittingArrow>();
                case Enums.Arrows.HeavyKnockback:
                    return gameObject.AddComponent<HeavyKnockbackArrow>();
                case Enums.Arrows.RapidFire:
                    return gameObject.AddComponent<RapidFireArrow>();
                case Enums.Arrows.Grappling:
                    return gameObject.AddComponent<GrapplingArrow>();
                case Enums.Arrows.Teleporting:
                    return gameObject.AddComponent<TeleportArrow>();
                default:
                    return gameObject.AddComponent<NormalArrow>();
            }
        }

		/// <summary>
		/// Damage dealt by the arrow
		/// </summary>
		public float Damage 
		{
			get { return damage; }
		}
    }
}
