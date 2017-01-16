using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Assets.Scripts.Tokens;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Rushes the opponent to fire at close range.
	/// </summary>
	public class RushEnemy : IPolicy
	{
		/// <summary> The desired horizontal distance between the AI and the target. </summary>
		internal float targetDistance;

		/// <summary> The object that the AI is targeting. </summary>
		internal GameObject target;
		/// <summary> The object that the AI's path is headed towards. </summary>
		private GameObject pathTarget;


		/// <summary> The AI's speed on the previous tick.</summary>
		private float lastSpeed;

		/// <summary> Timer for allowing the AI to turn. </summary>
		private float turnTimer;
		/// <summary> Tick cooldown for the AI turning. </summary>
		private const float TURNCOOLDOWN = 1;

		/// <summary> The distance away from a ledge that the AI will tolerate. </summary>
		private const float LEDGEGRABDISTANCE = 0.6f;

		/// <summary> Timer for the AI to replan its ledge path. </summary>
		private float replanTimer;
		/// <summary> The time interval used for the AI to replan its ledge path. </summary>
		private const float REPLANTIME = 2;
		/// <summary> The current ledge path that the AI is taking. </summary>
		private LinkedList<LedgeNode> currentPath;
		/// <summary> The ledge node that the AI is currently headed for. </summary>
		private LedgeNode currentNode;

		/// <summary> The ledges in the scene. </summary>
		private static LedgeNode[] ledges;
		/// <summary> Stores the next node between two nodes. </summary>
		private static LedgeNode[,] next;
		/// <summary> The name of the level that ledges are currently cached for. </summary>
		private static string levelName;

		/// <summary> Whether to draw debug lines for the ledge network. </summary>
		private const bool DEBUGLINES = false;

		/// <summary>
		/// Initializes a new AI.
		/// </summary>
		/// <param name="targetDistance">The desired horizontal distance between the AI and the opponent.</param>
		internal RushEnemy(float targetDistance) {
			this.targetDistance = targetDistance;
			string sceneName = SceneManager.GetActiveScene().name;
			if (ledges == null || levelName != sceneName)
			{
				// Create the level edge network with Floyd-Warshall.
				ledges = GameObject.FindObjectsOfType<LedgeNode>();
				next = new LedgeNode[ledges.Length, ledges.Length];
				float[,] distance = new float[ledges.Length, ledges.Length];
				for (int i = 0; i < ledges.Length; i++)
				{
					ledges[i].index = i;
				}
				for (int i = 0; i < ledges.Length; i++)
				{
					for (int j = 0; j < ledges.Length; j++)
					{
						distance[i, j] = Mathf.Infinity;
					}
				}
				for (int i = 0; i < ledges.Length; i++)
				{
					foreach (LedgeNode otherLedge in ledges[i].adjacentEdges)
					{
						distance[i, otherLedge.index] = Vector3.Distance(ledges[i].transform.position, otherLedge.transform.position);
						next[i, otherLedge.index] = otherLedge;
					}
				}
				for (int k = 0; k < ledges.Length; k++)
				{
					for (int i = 0; i < ledges.Length; i++)
					{
						for (int j = 0; j < ledges.Length; j++)
						{
							if (distance[i, k] + distance[k, j] < distance[i, j])
							{
								distance[i, j] = distance[i, k] + distance[k, j];
								next[i, j] = next[i, k];
							}
						}
					}
				}
				levelName = sceneName;
			}
		}

		/// <summary>
		/// Constructs a path from a start and end node.
		/// </summary>
		/// <returns>A path from the start to the end node.</returns>
		/// <param name="start">The node to start from.</param>
		/// <param name="end">The node to end at.</param>
		private LinkedList<LedgeNode> GetPath(LedgeNode start, LedgeNode end)
		{
			LinkedList<LedgeNode> path = new LinkedList<LedgeNode>();
			if (start == null || end == null || next[start.index, end.index] == null)
			{
				return path;
			}
			else
			{
				LedgeNode current = start;
				path.AddLast(start);
				while (current != end)
				{
					LedgeNode previous = current;
					current = next[current.index, end.index];
					float offset = current.transform.position.y - previous.transform.position.y;
					if (Mathf.Abs(offset) > 0.2f)
					{
						if (offset > 0)
						{
							current.YOffset = offset;
							if (previous == start && offset < 5)
							{
								path.RemoveFirst();
							}
						}
						else if (offset < 0)
						{
							previous.YOffset = offset;
						}
					}
					if (current != end || GetLedgePlatform(current) != GetLedgePlatform(previous))
					{
						path.AddLast(current);
					}
				}
				return path;
			}
		}


		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			if (DEBUGLINES)
			{
				foreach (LedgeNode ledge in ledges)
				{
					foreach (LedgeNode adjacent in ledge.adjacentEdges)
					{
						Debug.DrawLine(ledge.transform.position, adjacent.transform.position, Color.yellow);
					}
				}
			}
			if (target == null) {
				controller.SetRunInDirection(0);
				return;
			}
			Controller targetController = target.GetComponent<Controller>();
			if (targetController != null && targetController.LifeComponent.Health <= 0)
			{
				controller.SetRunInDirection(0);
				return;
			}

			lastSpeed = controller.runSpeed;

			controller.jump = false;
			float currentTargetDistance = targetDistance;
			Vector3 opponentOffset = target.transform.position - controller.transform.position;
			Vector3 targetOffset = opponentOffset;
			float distanceTolerance = targetDistance - 1;
			Vector3 playerCenter = controller.transform.position + Vector3.up * 0.75f;

			RaycastHit under;
			Physics.Raycast(playerCenter, Vector3.down, out under, 30, AIController.LAYERMASK);

			// Check if there is a platform in the way of shooting.
			RaycastHit hit;
			// Find a ledge path to get to the target.
			replanTimer -= Time.deltaTime;
			if (pathTarget != target && currentNode == null)
			{
				replanTimer = 0;
			}
			if (OnSamePlatform(controller.transform, target.transform))
			{
				if (Mathf.Abs(opponentOffset.y) < 5 && (Mathf.Abs(opponentOffset.y) < 0.1f || under.distance < 1))
				{
					currentNode = null;
				}
			}
			else if (replanTimer <= 0)
			{
				replanTimer = REPLANTIME;
				pathTarget = target;
				currentPath = GetPath(GetClosestLedgeNode(controller.transform), GetClosestLedgeNode(target.transform));
				if (currentPath.Count > 0)
				{
					currentPath.Last.Value.YOffset = target.transform.position.y - currentPath.Last.Value.transform.position.y;
					LoadNextLedge();
					currentNode.YOffset = currentNode.transform.position.y - controller.transform.position.y;
					if (currentPath.Count > 0)
					{
						if (GetLedgePlatform(currentPath.First.Value) == GetPlatformUnderneath(controller.transform))
						{
							LoadNextLedge();
						}
						else if (currentPath.Count == 1 && Mathf.Abs(currentNode.transform.position.y - currentPath.First.Value.transform.position.y) < 0.2f)
						{
							currentNode = null;
						}
					}
				}
				else
				{
					currentNode = null;
				}
			}

			if (currentNode != null)
			{
				// Move towards the nearest ledge, jumping if needed.
				Vector3 currentOffset = currentNode.transform.position - controller.transform.position;

				// Go to the next ledge node if possible.
				if (currentNode.YOffset >= 0 && Math.Abs(currentOffset.x) <= LEDGEGRABDISTANCE / 3 && currentOffset.y <= 0  ||
					currentNode.YOffset < 0 && currentOffset.y >= 1.5f)
				{
					LoadNextLedge();
				}
				if (currentNode != null)
				{
					currentOffset = currentNode.transform.position - controller.transform.position;

					currentTargetDistance = 0;
					distanceTolerance = 0.1f;

					if (Mathf.Abs(currentOffset.y) < distanceTolerance)
					{
						currentOffset.y = 0;
					}

					Vector3 platformOffset = currentNode.transform.position - GetLedgePlatform(currentNode).transform.position;

					float ledgeOffset = -0.1f;
					if (currentNode.YOffset > 0 && currentOffset.y > -0.5f ||
						currentNode.YOffset < 0)
					{
						ledgeOffset = LEDGEGRABDISTANCE;
					}
					if (platformOffset.x > 0)
					{
						currentOffset.x += ledgeOffset;
					}
					else
					{
						currentOffset.x -= ledgeOffset;
					}

					controller.jump = currentNode.YOffset >= 0 && Math.Abs(currentOffset.x) <= LEDGEGRABDISTANCE / 2;
					 
					targetOffset = currentOffset;
				}
			}

			if (DEBUGLINES)
			{
				Debug.DrawRay(controller.transform.position, targetOffset, Color.red);
				if (currentNode != null)
				{
					LedgeNode c = currentNode;
					foreach (LedgeNode l in currentPath)
					{
						Debug.DrawLine(c.transform.position, l.transform.position, Color.red);
						c = l;
					}
					Debug.DrawLine(c.transform.position, target.transform.position, Color.red);
				}
			}
				
			LedgeNode closestLedge = null;

			// Check if the AI is falling to its death.
			if (under.collider == null)
			{
				// Find the closest ledge to go to.
				float closestLedgeDistance = Mathf.Infinity;
				foreach (LedgeNode ledge in ledges)
				{
                    if (ledge != null)
                    {
                        float currentDistance = Mathf.Abs(ledge.transform.position.x - controller.transform.position.x);
                        if (currentDistance < closestLedgeDistance && ledge.transform.position.y < controller.transform.position.y + 1)
                        {
                            closestLedge = ledge;
                            closestLedgeDistance = currentDistance;
                        }
                    }
				}
				bool awayFromLedge = false;
				if (closestLedge == null)
				{
					controller.SetRunInDirection(-controller.transform.position.x);
				}
				else {
					float ledgeOffsetX = closestLedge.transform.position.x - controller.transform.position.x;
					if (Mathf.Abs(ledgeOffsetX) > LEDGEGRABDISTANCE)
					{
						controller.SetRunInDirection(ledgeOffsetX);
						awayFromLedge = true;
					}
				}
				controller.jump = true;
				if (awayFromLedge)
				{
					return;
				}
			}

			// Move towards the opponent.
			float horizontalDistance = Mathf.Abs(targetOffset.x);
			if (horizontalDistance > currentTargetDistance)
			{
				controller.SetRunInDirection(targetOffset.x);
			}
			else if (horizontalDistance < currentTargetDistance - distanceTolerance)
			{
				controller.SetRunInDirection(-targetOffset.x);
			}
			else if (opponentOffset == targetOffset && under.collider != null && (controller.ParkourComponent.FacingRight ^ opponentOffset.x > 0))
			{
				controller.ParkourComponent.FacingRight = opponentOffset.x > 0;
			}
			else
			{
				controller.runSpeed = 0;
			}
			if (controller.runSpeed != 0)
			{
				// Don't chase an opponent off the map.
				Vector3 offsetPosition = controller.transform.position;
				offsetPosition.x += controller.runSpeed / 3;
				offsetPosition.y += 0.5f;
				Vector3 offsetPosition3 = offsetPosition;
				offsetPosition3.x += controller.runSpeed;
				if (!Physics.Raycast(offsetPosition, Vector3.down, out hit, 30, AIController.LAYERMASK) &&
					!Physics.Raycast(offsetPosition3, Vector3.down, out hit, 30, AIController.LAYERMASK))
				{
					if (controller.ParkourComponent.Sliding)
					{
						controller.SetRunInDirection(-opponentOffset.x);
					}
					else if (closestLedge == null)
					{
						controller.runSpeed = 0;
					}
					controller.slide = false;
				}
				else
				{
					// Slide if the opponent is far enough away for sliding to be useful.
					controller.ParkourComponent.FacingRight = opponentOffset.x > 0;
					controller.slide = horizontalDistance > 10 && currentNode == null && OnSamePlatform(controller.transform, target.transform);
				}
			}

			if (controller.runSpeed == 0 && Mathf.Abs(opponentOffset.x) < 1 && opponentOffset.y < 0 && target.GetComponent<Controller>() && controller.GetComponent<Rigidbody>().velocity.y <= Mathf.Epsilon)
			{
				// Don't sit on top of the opponent.
				controller.SetRunInDirection(-controller.transform.position.x);
			}

			if (controller.runSpeed > 0 && lastSpeed < 0 || controller.runSpeed < 0 && lastSpeed > 0)
			{
				// Check if the AI turned very recently to avoid thrashing.
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0) {
					turnTimer = TURNCOOLDOWN;
				} else {
					controller.runSpeed = 0;
				}
			}

			// Jump to reach some tokens.
			if (targetDistance == 0 && controller.runSpeed == 0 && target.GetComponent<ArrowToken>() && opponentOffset == targetOffset) {
				controller.jump = true;
			}

			controller.fastFall = targetOffset.y < -1f;
		}

		/// <summary>
		/// Gets the closest ledge node from a target.
		/// </summary>
		/// <returns>The closest node.</returns>
		/// <param name="target">Target.</param>
		private LedgeNode GetClosestLedgeNode(Transform target)
		{
			RaycastHit hit;
			LedgeNode[] currentLedges = ledges;
			if (Physics.Raycast(target.position + Vector3.up * 0.5f, Vector3.down, out hit, 30, AIController.LAYERMASK))
			{
				currentLedges = GetPlatformLedges(hit);
			}
			LedgeNode closestLedge = null;
			float closestDistance = Mathf.Infinity;
			foreach (LedgeNode ledge in currentLedges)
			{
				float currentDistance = Vector3.Distance(target.position, ledge.transform.position);
				if (ledge.transform.position.y < target.position.y + 1.5f &&
					currentDistance < closestDistance)
				{
					closestLedge = ledge;
					closestDistance = currentDistance;
				}
			}
			return closestLedge;
		}

		/// <summary>
		/// Loads the next ledge node into the current node.
		/// </summary>
		private void LoadNextLedge()
		{
			if (currentPath == null || currentPath.Count == 0)
			{
				currentNode = null;
				replanTimer = 0;
			}
			else
			{
				currentNode = currentPath.First.Value;
				currentPath.RemoveFirst();
			}
		}

		/// <summary>
		/// Gets the platform that a ledge is a part of.
		/// </summary>
		/// <returns>The platform that a ledge is a part of.</returns>
		/// <param name="ledge">The ledge to get a platform for.</param>
		private GameObject GetLedgePlatform(LedgeNode ledge)
		{
			if (ledge.transform.parent.parent != null)
			{
				return ledge.transform.parent.parent.gameObject;
			}
			else
			{
				return ledge.transform.parent.gameObject;
			}
		}

		/// <summary>
		/// Gets the ledges attached to a platform.
		/// </summary>
		/// <returns>The ledges attached to the platform.</returns>
		/// <param name="platformHit">The raycast that hit the platform.</param>
		private LedgeNode[] GetPlatformLedges(RaycastHit platformHit)
		{
			LedgeNode[] currentLedges;
			if (platformHit.collider.tag == "Ledge")
			{
				currentLedges = platformHit.collider.transform.parent.GetComponentsInChildren<LedgeNode>();
			}
			else
			{
				currentLedges = platformHit.collider.GetComponentsInChildren<LedgeNode>();
			}
			if (currentLedges.Length == 0 && platformHit.collider.transform.parent != null)
			{
				currentLedges = platformHit.collider.transform.parent.GetComponentsInChildren<LedgeNode>();
			}
			return currentLedges;
		}

		/// <summary>
		/// Checks if two objects are on the same platform.
		/// </summary>
		/// <param name="position1">The position of the first object. </param>
		/// <param name="position2">The position of the second object.</param>
		private bool OnSamePlatform(Transform position1, Transform position2)
		{
			GameObject platform1 = GetPlatformUnderneath(position1);
			GameObject platform2 = GetPlatformUnderneath(position2);
			return platform1 != null && platform2 != null && platform1 == platform2;
		}

		/// <summary>
		/// Gets the platform underneath a position.
		/// </summary>
		/// <returns>The platform underneath the position.</returns>
		/// <param name="target">The position to get a platform from.</param>
		private GameObject GetPlatformUnderneath(Transform target)
		{
			RaycastHit hit;
			if (Physics.Raycast(target.position + Vector3.up * 0.5f, Vector3.down, out hit, 30, AIController.LAYERMASK))
			{
				Transform current = hit.collider.transform;
				while (current != null)
				{
					if (current.tag == "Ground")
					{
						return current.gameObject;
					}
					current = current.parent;
				}
			}
			return null;
		}
	}
}