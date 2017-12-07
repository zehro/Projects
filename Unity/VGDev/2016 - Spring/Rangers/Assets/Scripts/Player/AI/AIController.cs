using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Interface for AI to control a ranger.
	/// </summary>
	public class AIController : Controller
	{
		/// <summary> The policy used to decide the AI's actions. </summary>
		private IPolicy policy;

		/// <summary> The speed that the ranger is moving at. </summary>
		[SerializeField]
		[Tooltip("The speed that the ranger is moving at.")]
		internal float runSpeed = 0;

		/// <summary> Whether the ranger is jumping. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is jumping.")]
		internal bool jump = false;
		/// <summary> Whether the ranger is sliding. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is sliding.")]
		internal bool slide = false;
		/// <summary> Whether the ranger is fast-falling. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is fast-falling.")]
		internal bool fastFall = false;

		/// <summary> Whether the ranger is grabbing a token. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is grabbing a token")]
		internal bool grabToken = true;

		/// <summary> Whether the ranger is aiming a shot. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is aiming a shot.")]
		internal bool aiming = false;
		/// <summary> Whether the ranger was aiming its shot on the last tick. </summary>
		private bool wasAiming = false;
		/// <summary> Vector in the direction that the ranger is aiming. </summary>
		[SerializeField]
		[Tooltip("Vector in the direction that the ranger is aiming.")]
		internal Vector3 aim = Vector3.zero;

		/// <summary> The default movement speed of the ranger. </summmary>
		[SerializeField]
		[Tooltip("The default movement speed of the ranger.")]
		private float defaultMoveSpeed = 1;

		/// <summary> Layer mask for raycasting platforms. </summary>
		internal const int LAYERMASK = 1 << 13 | 1 << 15;

		/// <summary>
		/// Initializes the AI policy to use.
		/// </summary>
		private new void Start()
		{
			policy = new ApproachShoot();
			base.Start();
		}

		/// <summary>
		/// Moves the ranger every tick.
		/// </summary>
		private new void Update()
		{
			base.Update();
			if(GameManager.instance.IsPaused) return;
			if (life.Health <= 0 || GameManager.instance.GameFinished)
			{
				aiming = false;
				return;
			}
			else
			{
				policy.ChooseAction(this);
			}

			if (jump)
			{
				parkour.Jump();
			}

			if (slide)
			{
				parkour.SlideOn();
			}
			else
			{
				parkour.SlideOff();
			}

			if (grabToken)
			{
				GrabToken();
			}

			if (aiming)
			{
				archery.UpdateFirePoint(Vector3.Normalize(aim));
				if (!wasAiming)
				{
					SFXManager.instance.PlayArrowPull();
				}
				wasAiming = true;
			}
			else if (wasAiming)
			{
				wasAiming = false;
				archery.Fire();
			}
			else
			{
				archery.AimUpperBodyWithLegs();
			}
		}

		/// <summary>
		/// Updates the ranger's running movement.
		/// </summary>
		private void FixedUpdate()
		{
			if (life.Health > 0 && !GameManager.instance.IsPaused && !GameManager.instance.GameFinished)
			{
				parkour.Locomote(runSpeed);
			}
		}

		/// <summary>
		/// Sets the ranger's run speed in the direction of the given number.
		/// </summary>
		/// <param name="direction">The direction to set the run speed to.</param>
		internal void SetRunInDirection(float direction)
		{
			if (Mathf.Abs(direction) < defaultMoveSpeed * Time.deltaTime * 4)
			{
				runSpeed = 0;
			}
			else if (direction > 0)
			{
				runSpeed = defaultMoveSpeed;
			}
			else if (direction < 0)
			{
				runSpeed = -defaultMoveSpeed;
			}
		}

		/// <summary>
		/// Checks if the controller is holding the jump button.
		/// </summary>
		/// <returns>Whether the controller is holding the jump button.</returns>
		internal override bool IsHoldingJump() {
			return jump;
		}

		/// <inheritdoc/>
		internal override bool IsHoldingDown()
		{
			return fastFall;
		}

		/// <summary>
		/// Checks if there is an unobstructed shot between the ranger and a target.
		/// </summary>
		/// <returns>Whether there is an unobstructed shot between the ranger and a target.</returns>
		/// <param name="positionOffset">The vector from the ranger to its target.</param>
		/// <param name="hit">The raycast information for the shot.</param> 
		internal bool HasClearShot(Vector3 positionOffset, out RaycastHit hit) {
			//Debug.DrawRay(transform.position + Vector3.up - Vector3.Normalize(positionOffset), positionOffset, Color.white);
			hit = new RaycastHit();
			return Bitwise.IsBitOn(ArcheryComponent.ArrowTypes, (int)Enums.Arrows.Ghost) || !Physics.Raycast(transform.position + Vector3.up - Vector3.Normalize(positionOffset), positionOffset, out hit, Vector3.Magnitude(positionOffset) + 1, LAYERMASK);
		}
	}
}