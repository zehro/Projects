using UnityEngine;

namespace YeggQuest.NS_Dialog
{
    class DialogTester : MonoBehaviour
    {
        public DialogRoot graph;
        public DialogEdge edge;

        private void Update()
        {
            if (Input.GetKey(KeyCode.P))
                graph.GetCurrText();
            if (Input.GetKey(KeyCode.O))
                graph.Transition();
            if (Input.GetKey(KeyCode.I))
                edge.SetTrigger(true);
        }
    }
}
