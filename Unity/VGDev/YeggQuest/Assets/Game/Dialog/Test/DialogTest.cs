/*using UnityEngine;

namespace YeggQuest.NS_Dialog.Test
{
    class DialogTest : MonoBehaviour
    {
        public bool isNear, isIn;
        public DialogRoot graph;
        public DialogNode root1, root2;
        public UnityEngine.UI.Text text;
        public GameObject image;

        private bool temp = true;

        private void Update()
        {
            if(temp != isNear)
            {
                graph.SetCurrentNode(isNear ? root1 : root2);
                temp = isNear;
                text.text = "";
                image.SetActive(false);
            }
            else if(isIn)
            {
                text.text = graph.GetCurrText();
                image.SetActive(true);
                if (Input.GetKeyUp(KeyCode.T))
                {
                    graph.Transition();
                }
            }
        }
    }
}*/