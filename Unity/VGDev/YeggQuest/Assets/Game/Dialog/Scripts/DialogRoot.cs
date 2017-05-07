using UnityEngine;
using System.Collections.Generic;

namespace YeggQuest.NS_Dialog
{
    public class DialogRoot : MonoBehaviour
    {
        /// <summary> The node the graph starts at. </summary>
        [SerializeField]
        private DialogNode startNode;

        /// <summary> Holds the graph so we can navigate it quickly. </summary>
        private Dictionary<DialogNode, Node> graph;
        /// <summary> The current node the graph is on. </summary>
        private Node curr;

        private void Start()
        {
            graph = new Dictionary<DialogNode, Node>();
            DialogNode[] nodes = GetComponentsInChildren<DialogNode>() as DialogNode[];
            DialogEdge[] edges = GetComponentsInChildren<DialogEdge>() as DialogEdge[];
            foreach (DialogNode node in nodes)
            {
                Node n = new Node();
                n.node = node;
                n.edges = new List<DialogEdge>();
                graph.Add(node, n);
            }
            foreach (DialogEdge edge in edges)
                graph[edge.StartNode].edges.Add(edge);
            foreach (Node n in graph.Values)
                n.edges.Sort((x, y) => -1 * x.Priority.CompareTo(y.Priority));
            curr = graph[startNode];
#if UNITY_EDITOR
            curr.node.isSelected = true;
#endif
        }

        /// <summary> 
        ///     Checks if a transition out of this node is possible. 
        ///     If it is it takes the highest priority edge that has a valid transition.
        /// </summary>
        /// <returns> True if a transtion was made. </returns>
        public bool Transition()
        {
            if (curr.node.requireRead && !curr.node.hasBeenRead)
                return false;
            foreach (DialogEdge e in curr.edges)
            {
                if (e.trigger)
                {
#if UNITY_EDITOR
                    curr.node.isSelected = false;
#endif
                    curr = graph[e.EndNode];
#if UNITY_EDITOR
                    curr.node.isSelected = true;
#endif
                    return true;
                }
            }
            return false;
        }

        /// <summary> Gets the dialog from the current node. </summary>
        /// <returns> The string dialog from the current node. </returns>
        public string GetCurrText()
        {
            return curr.node.Dialog;
        }

        /// <summary> Set the current node of the graph. </summary>
        /// <param name="node"> The node in the graph to set it to. </param>
        public void SetCurrentNode(DialogNode node)
        {
#if UNITY_EDITOR
            curr.node.isSelected = false;
#endif
            if (graph.ContainsKey(node))
                curr = graph[node];
            else
                Debug.LogError("Tried to set graph " + name + " to non-existant node.");
#if UNITY_EDITOR
            curr.node.isSelected = true;
#endif
        }
    }

    public class Node
    {
        /// <summary> The Dialog node for this node. </summary>
        public DialogNode node;
        /// <summary> The edges coming out of this node sorted by priority. </summary>
        public List<DialogEdge> edges;
    }
}
