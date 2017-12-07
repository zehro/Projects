using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Util;

namespace Assets.Scripts.Tokens
{
    /// <summary>
    /// Allows the characters to collect different types of arrows
    /// </summary>
    public class ArrowToken : Token
    {
        [SerializeField]
        private Enums.Arrows type;

		[HideInInspector]
		public bool collected;

		private SpriteRenderer collectKey;

        /// <summary>
        ///  Override the TokenCollected method and tell the Archery component to collect the token
        /// </summary>
        /// <param name="controller">The controller that is doing the collecting</param>
        public override void TokenCollected(Controller controller)
        {
            if (controller.ArcheryComponent.CanCollectToken() && !Util.Bitwise.IsBitOn(controller.ArcheryComponent.ArrowTypes, (int)type))
            {
                controller.ArcheryComponent.CollectToken(this);
                // Set inactive since we are pooling
				collected = true;
				GetComponent<Collider>().enabled = false;
            }
        }

		void Start() {
			collectKey = transform.FindChild("CollectKey").GetComponent<SpriteRenderer>();
		}

		void Update() {
			if(collected) {
				transform.localScale += new Vector3(Time.deltaTime*transform.localScale.x,-Time.deltaTime*transform.localScale.y*10f,0f);
				if(transform.localScale.y <= 0.1f) {
					transform.localScale = Vector3.one;
					collected = false;
					GetComponent<Collider>().enabled = true;
					gameObject.SetActive(false);
				}
			}
		}

		void OnTriggerEnter(Collider other) {
			if(other.transform.root.GetComponent<Controller>()) {
				collectKey.GetComponent<AutoKeyUI>().id = other.transform.root.GetComponent<Controller>().ID;
			}
		}

		void OnTriggerExit(Collider other) {
			if(other.transform.root.GetComponent<Controller>()) {
				collectKey.GetComponent<AutoKeyUI>().id = PlayerID.None;
			}
		}

        #region C# Properties
        /// <summary>
        /// Type of arrow associated with the token
        /// </summary>
        public Enums.Arrows Type
        {
            get { return type; }
        }
        #endregion
    }
}
