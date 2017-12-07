using UnityEngine;
using Assets.Scripts.Tokens;
using System.Collections;

namespace Assets.Scripts.Player
{
	public class Parkour : ControllerObject 
	{
		private const float FALL_THRESHOLD = 0.05f;

		private bool facingRight = true;
		//private bool ledgeGrabbing = false;
        private bool grappling = false;
		private bool doubleJump = false;

		private GameObject IKThingy;

		private float fallingTime;
		private float slidingTime;
		private float slidingCooldown;
		private float lastMotion;
		private float jumpingTimeOffset;

		/// <summary> The animator attached to the player. </summary>
		private Animator animator;
        /// <summary> The rigidbody attached to the player. </summary>
        private new Rigidbody rigidbody;

		private Collider hipL, hipR, kneeL, kneeR;
		private float legColliderReEnabler;

        private void Start()
		{
			animator = GetComponent<Animator>();
			rigidbody = GetComponent<Rigidbody>();

			hipL = transform.FindChild("U").FindChild("joint_Char").FindChild("joint_Pelvis").FindChild("joint_HipMaster").FindChild("joint_HipLT").GetComponent<Collider>();
			hipR = transform.FindChild("U").FindChild("joint_Char").FindChild("joint_Pelvis").FindChild("joint_HipMaster").FindChild("joint_HipRT").GetComponent<Collider>();
			kneeL = transform.FindChild("U").FindChild("joint_Char").FindChild("joint_Pelvis").FindChild("joint_HipMaster").FindChild("joint_HipLT").FindChild("joint_KneeLT").GetComponent<Collider>();
			kneeR = transform.FindChild("U").FindChild("joint_Char").FindChild("joint_Pelvis").FindChild("joint_HipMaster").FindChild("joint_HipRT").FindChild("joint_KneeRT").GetComponent<Collider>();
		}

		public void Locomote(float motion) 
		{
			if(slidingTime <= 0)
			{
				transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
				if(motion > 0) 
				{
					facingRight = true;
				} 
				else if (motion < 0) 
				{
					facingRight = false;
				}
				else
				{
					facingRight = !animator.GetCurrentAnimatorStateInfo(0).IsTag("Left");
				}
				if (animator.GetBool("CanMove"))
				{
	   				if(facingRight && Physics.Raycast(new Ray(transform.position + new Vector3(0.2f, 0, 0), Vector3.up),2.5f, (1 << LayerMask.NameToLayer("Ground")))) 
					{
						animator.SetFloat("RunSpeed", Mathf.Min(0,motion));
					} 
					else if(!facingRight && Physics.Raycast(new Ray(transform.position - new Vector3(0.2f, 0, 0), Vector3.up), 2.5f, (1 << LayerMask.NameToLayer("Ground"))))
					{
						animator.SetFloat("RunSpeed", Mathf.Max(0,motion));
					} 
					else 
					{
						animator.SetFloat("RunSpeed", motion);
						rigidbody.MovePosition(transform.position + transform.forward*motion*Time.deltaTime*6);
						if (controller.IsHoldingDown()) {
							rigidbody.AddForce(0,-1,0,ForceMode.VelocityChange);
						}
					}
				}
				lastMotion = motion;
			}
		}

		public void Jump()
		{
			rigidbody.useGravity = true;
            if(grappling)
            {
                GetComponent<Grapple>().Ungrapple();
            }
			else if(fallingTime < FALL_THRESHOLD || doubleJump) 
			{
				animator.ResetTrigger("Land");
				if(facingRight)
				{
					animator.SetTrigger("Jump");
				} 
				else if (!facingRight) 
				{
					animator.SetTrigger("JumpLeft");
				}
				jumpingTimeOffset = 0.1f;
				doubleJump = false;
				SFXManager.instance.PlayJump();
			}
		}

		void Update() 
		{
			if(!kneeL.enabled) {
				legColliderReEnabler -= Time.deltaTime;
				if(legColliderReEnabler <= 0) {
					kneeL.enabled = true;
					kneeR.enabled = true;
					hipL.enabled = true;
					hipR.enabled = true;
				}
			}
			jumpingTimeOffset -= Time.deltaTime;


			animator.SetBool("Falling", fallingTime >= FALL_THRESHOLD);
			if(fallingTime < FALL_THRESHOLD)
			{
				bool onGround = false;
				RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.1f, -transform.up, 0.2f);
				foreach(RaycastHit hit in hits)
				{
					if(hit.collider.gameObject.tag.Equals("Ground")) {
						onGround = true;
						break;
					}
				}
				if(!onGround)
				{
					fallingTime += Time.deltaTime;
				}
			}


			slidingTime = Mathf.Max(0, slidingTime - Time.deltaTime);
			slidingCooldown = Mathf.Max(0, slidingCooldown - Time.deltaTime);
			if(slidingTime <= 0) 
			{
				SlideOff();
			}
		}

		public void SlideVelocity() 
		{
			float direction = facingRight ? 1 : -1;
			rigidbody.velocity = Vector3.right*direction*10f;
		}

		public void SlideOn() 
		{
			if(facingRight) {
				SlideRight();
			} else {
				SlideLeft();
			}
		}

		public void SlideRight() 
		{
			if(slidingCooldown <= 0 && fallingTime < FALL_THRESHOLD) {
				animator.SetFloat("RunSpeed", 1);
				facingRight = true;
				slidingTime = 1;
				slidingCooldown = 1.2f*slidingTime;
				animator.SetBool("Slide", true);
				animator.SetBool("CanMove", false);
				SlideVelocity();
			}
		}

		public void SlideLeft() 
		{
			if(slidingCooldown <= 0 && fallingTime < FALL_THRESHOLD) {
				animator.SetFloat("RunSpeed", -1);
				facingRight = false;
				slidingTime = 1;
				slidingCooldown = 1.2f*slidingTime;
				animator.SetBool("Slide", true);
				animator.SetBool("CanMove", false);
				SlideVelocity();
			}
		}

		public void SlideOff()
		{
			animator.SetBool("CanMove", true);
			animator.SetFloat("RunSpeed", 0);
			animator.SetBool("Slide", false);
			rigidbody.velocity.Set(0, rigidbody.velocity.y, 0);
		}

		void OnAnimatorIK() 
		{
			Collider[] overlaps = Physics.OverlapSphere(transform.position + Vector3.up,1f);
			foreach(Collider c in overlaps) 
			{
				if(c.gameObject.tag.Equals("Ledge")) 
				{
					IKThingy = c.gameObject;
					break;
				}
				IKThingy = null;
			}
			if(IKThingy != null) 
			{
				animator.SetIKPosition(AvatarIKGoal.RightHand,IKThingy.transform.position + new Vector3(-0.5f,0,0));
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKPosition(AvatarIKGoal.LeftHand,IKThingy.transform.position + new Vector3(0.5f,0,0));
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
				if(!state.IsName("Jump") && !state.IsName("JumpLeft"))
				{
					doubleJump = true;
				}
				//				if(!ledgeGrabbing) 
				//				{
				//					rigidbody.velocity = Vector3.zero;
				//				}
				//				ledgeGrabbing = true;
			} 
			else 
			{
				//				ledgeGrabbing = false;
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				doubleJump = false;
			}
		}


		// Called by animation event, it is time to jump, handles full vs short hop
		public void JumpVelocity() 
		{
			if(IKThingy) {
				kneeL.enabled = false;
				kneeR.enabled = false;
				hipL.enabled = false;
				hipR.enabled = false;
				legColliderReEnabler = 0.2f;
			}

			// If the jump button is still being held, full hop
			if (controller.IsHoldingJump())
				rigidbody.velocity = Vector3.up*8f;
			// else short hop
			else
				rigidbody.velocity = Vector3.up*5f;
		}

		void OnCollisionStay(Collision other) 
		{
			if((animator.GetCurrentAnimatorStateInfo(0).IsName("Airtime") || animator.GetCurrentAnimatorStateInfo(0).IsName("AirtimeLeft")) && other.gameObject.tag.Equals("Ground")) 
			{
				animator.SetTrigger("Land");
				fallingTime = 0;
			}
		}

		public bool FacingRight 
		{
			get { return facingRight; }
			set 
			{
				if (animator.GetFloat("RunSpeed") == 0)
				{
					facingRight = value;
					float motion = 0.01f;
					if (!value)
					{
						motion = -motion;
					}
					animator.SetFloat("RunSpeed", motion);
				}
			}
		}

        public bool Grappling
        {
            get { return grappling; }
            set { grappling = value; }
        }

		/// <summary>
		/// Overriding the collect token method from player controller object
		/// </summary>
		/// <param name="token">The token that was collected</param>
		public override void CollectToken(Token token) { }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Assets.Scripts.Player.Parkour"/> is sliding.
		/// </summary>
		/// <value><c>true</c> if sliding; otherwise, <c>false</c>.</value>
		public bool Sliding
		{
			get { return slidingTime > 0; }
		}
	}
}