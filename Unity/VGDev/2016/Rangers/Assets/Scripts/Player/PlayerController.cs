using UnityEngine;
using Assets.Scripts.Data;

namespace Assets.Scripts.Player
{
	/// <summary>
	/// Class that handles player specific components of the controller
	/// Uses input
	/// </summary>
	public class PlayerController : Controller
	{
		//has the player drawn the bow back, and is ready to fire?
		private bool fire;

		//should we play the drawArrow sound effect?
		private bool drawnArrow;

		//did the joystick overshoot the deadzone, triggering a fire?
		private bool clickFire;

		//used to help check for overshooting the joystick deadzone
		//Vector3 prevAim = Vector3.zero;

		//locking the maximum fire rate for anyone spamming the joystick or any accidental input
		private const float MAX_FIRE_RATE = 0.5f;
		private float fireRateTimer = 0;

		private new void Update()
		{
			base.Update();
			//updating fireRateTimer
			fireRateTimer += Time.deltaTime;

			if (life.Health > 0 && !GameManager.instance.IsPaused && !GameManager.instance.GameFinished)
			{
				//keeping track of this every frame to help prevent accidental fires or mis-aiming
				Vector3 aim = new Vector3(
					-ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.RightStickX, id),
					-ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.RightStickY, id),
					0) * distanceToPlayer;

				// Poll Button input
				if (ControllerManager.instance.GetButton(ControllerInputWrapper.Buttons.A,id)) parkour.Jump();
				if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.RightBumper, id)) parkour.SlideRight();
				if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.LeftBumper, id)) parkour.SlideLeft();
                if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.X, id)) GrabToken();

				if(Vector3.Magnitude(aim) > ControllerManager.CUSTOM_DEADZONE && !clickFire)
				{
					//if the joystick is pushed past the 50% mark in any direction, start aiming the bow
					archery.UpdateFirePoint(aim);
					fire = true;
					if (ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.RightStickClick, id)) {
						drawnArrow = false;
						archery.Fire();
						fire = false;
						fireRateTimer = 0;
						clickFire = true;
					}
					if(!drawnArrow) {
						SFXManager.instance.PlayArrowPull();
						drawnArrow = true;
					}
				} else if (fireRateTimer > MAX_FIRE_RATE && fire)
				{
					drawnArrow = false;
					archery.Fire();
					fire = false;
					fireRateTimer = 0;
				}
				else
				{
					//if the joystick isn't pushed in any direction then align the upper body with the legs
					archery.AimUpperBodyWithLegs();
					clickFire = false;
				}
			}
			//if (invincibleFrames > 0) invincibleFrames--;
		}

		void FixedUpdate() 
		{
			//This has to happen every fixed update as of now, can't think of a better way to handle it --kartik
			if(life.Health > 0 && !GameManager.instance.IsPaused && !GameManager.instance.GameFinished) 
			{
				parkour.Locomote(ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickX, id));
			}
		}

		/// <summary>
		/// Checks if the controller is holding the jump button.
		/// </summary>
		/// <returns>Whether the controller is holding the jump button.</returns>
		internal override bool IsHoldingJump() {
			return !GameManager.instance.IsPaused && !GameManager.instance.GameFinished && ControllerManager.instance.GetButton(ControllerInputWrapper.Buttons.A,ID);
		}

		/// <inheritdoc/>
		internal override bool IsHoldingDown()
		{
			return ControllerManager.instance.GetAxis(ControllerInputWrapper.Axis.LeftStickY,ID) < -3 * ControllerManager.CUSTOM_DEADZONE;
		}
	}
}
