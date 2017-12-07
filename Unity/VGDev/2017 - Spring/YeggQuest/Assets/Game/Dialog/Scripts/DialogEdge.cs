using UnityEngine;

namespace YeggQuest.NS_Dialog
{
    /// <summary>
    /// Edge class for the dialoge graph.  Set up to be immutable at runtime.
    /// </summary>
    public class DialogEdge : MonoBehaviour
    {
        /// <summary> Callback function for edges.  Its called when the edge is taken during a transition. </summary>
        public delegate void Callback();

        /// <summary> Callback function for this edge.  Its called when this edge is taken during a transition. </summary>
        internal Callback onTransition;

        /// <summary> The start point of this edge. </summary>
        [SerializeField]
        [Tooltip("The start point of this edge.")]
        private DialogNode startNode;

        /// <summary> The start point of this edge. </summary>
        public DialogNode StartNode { get { return startNode; } }

        /// <summary> The end point of this edge. </summary>
        [SerializeField]
        [Tooltip("The end point of this edge.")]
        private DialogNode endNode;

        /// <summary> The end point of this edge. </summary>
        public DialogNode EndNode { get { return endNode; } }

        /// <summary> The priority level of this edge.  Used to sort the edges from highest to lowest. </summary>
        [SerializeField]
        [Tooltip("The priority level of this edge.  Used to sort the edges from highest to lowest.")]
        private int priority;

        /// <summary> The priority level of this edge.  Used to sort the edges from highest to lowest. </summary>
        public int Priority { get { return priority; } }

        /// <summary> The initial state of the trigger for this edge. </summary>
        [SerializeField]
        [Tooltip("The initial state of the trigger for this edge.")]
        private bool defaultTriggerState;

        /// <summary> The trigger for this edge. </summary>
        internal bool trigger;

        /// <summary> Sets this edges trigger to the specified value. </summary>
        /// <param name="triggerVal"> The value to set. </param>
        public void SetTrigger(bool triggerVal)
        {
            trigger = triggerVal;
        }

        /// <summary> Sets the OnTransition function for this edge. </summary>
        /// <param name="func"> The function you want called during a transition. </param>
        public void SetCallback(Callback func)
        {
            onTransition = func;
        }

        private void Start()
        {
            trigger = defaultTriggerState;
        }

#if UNITY_EDITOR
        /// <summary> 
        /// Draws an arrow from the start to the end node. 
        /// The arror is green if this edge has been triggered, white otherwise. 
        /// </summary>
        private void OnDrawGizmos()
        {
            if (startNode != null && endNode != null)
            {
                Vector3 start = startNode.transform.position;
                Vector3 end = endNode.transform.position;
                Vector3 es = Vector3.Normalize(start - end);
                Vector3 left = Vector3.Cross(es, es.x == 0 ? Vector3.right : Vector3.up);
                Vector3 leftDiagonal = Vector3.Normalize(es + left / 2f) / 3f;
                Vector3 rightDiagonal = Vector3.Normalize(es - left / 2f) / 3f;
                transform.position = (start + end) / 2f;
                Gizmos.color = trigger ? Color.green : Color.white;
                Gizmos.DrawLine(start, end);
                Gizmos.DrawLine(end, end + leftDiagonal);
                Gizmos.DrawLine(end, end + rightDiagonal);
            }
        }
#endif
    }
}
