using UnityEngine;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// A node in the ledge network.
	/// </summary>
	public class LedgeNode : MonoBehaviour {
		/// <summary> The edges adjacent to this edge. </summary>
		[Tooltip("The edges adjacent to this edge.")]
		public LedgeNode[] adjacentEdges;
		/// <summary> The index of the node. </summary>
		internal int index;
		/// <summary> The y difference from the previous node. </summary>
		private float yOffset;
		/// <summary> The y difference from the previous node. </summary>
		internal float YOffset {
			get { return yOffset; }
			set { yOffset = Mathf.Abs(value) < 0.2f ? 0 : value; }
		}
	}
}