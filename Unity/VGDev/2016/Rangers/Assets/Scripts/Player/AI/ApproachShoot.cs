using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Tokens;
using System.Collections.Generic;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Approaches the opponent and shoots at it.
	/// </summary>
	public class ApproachShoot : IPolicy
	{
		/// <summary> The distance that the AI wants to be from the target. </summary>
		private const float TARGETDISTANCE = 4;

		/// <summary> The policy the AI is using to approach an enemy. </summary>
		private RushEnemy rushPolicy = new RushEnemy(TARGETDISTANCE);
		/// <summary> The policy the AI is using to shoot at an enemy. </summary>
		private Shoot shootPolicy = new Shoot(0.5f, 0.4f, -Mathf.PI / 4);

		/// <summary> The opponent that the AI is currently shooting at. </summary>
		private Controller shootTarget;

		/// <summary> The token the AI is headed for. </summary>
		private GameObject targetToken;

		/// <summary>
		/// Initializes a new AI.
		/// </summary>
		internal ApproachShoot()
		{
		}

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			if (shootTarget == null)
			{
				// Find a new target to attack.
				Controller closestOpponent = null;
				float closestDistance = Mathf.Infinity;
				foreach (Controller opponent in GameManager.instance.AllPlayers)
				{
					if (opponent != controller && opponent.LifeComponent.Health > 0)
					{
						float opponentDistance = Vector3.Distance(opponent.transform.position, controller.transform.position);
						if (opponentDistance < closestDistance)
						{
							closestDistance = opponentDistance;
							closestOpponent = opponent;
						}
					}
				}
				if (controller.ArcheryComponent.CanCollectToken())
				{
					foreach (GameObject token in TokenSpawner.instance.Tokens)
					{
						// See if any tokens are close enough to bother with.
						if (token.activeSelf)
						{
							float tokenDistance = Vector3.Distance(token.transform.position, controller.transform.position);
							if (tokenDistance < closestDistance && !Util.Bitwise.IsBitOn(controller.ArcheryComponent.ArrowTypes, (int)token.GetComponent<ArrowToken>().Type))
							{
								closestDistance = tokenDistance;
								targetToken = token;
								rushPolicy.target = token.gameObject;
								rushPolicy.targetDistance = 0;
							}
						}
					}
				}
				if (targetToken == null && closestOpponent != null)
				{
					rushPolicy.target = closestOpponent.gameObject;
					rushPolicy.targetDistance = TARGETDISTANCE;
				}
				shootPolicy.target = closestOpponent;
				shootTarget = closestOpponent;
			}

			rushPolicy.ChooseAction(controller);
			shootPolicy.ChooseAction(controller);

			if (!controller.aiming || (shootTarget != null && shootTarget.LifeComponent.Health <= 0))
			{
				shootTarget = null;
			}
			if (targetToken != null && !targetToken.activeSelf)
			{
				targetToken = null;
				if (shootPolicy.target != null) {
					rushPolicy.target = shootPolicy.target.gameObject;
				}
			}
		}
	}
}