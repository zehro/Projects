/*using UnityEngine;

namespace YeggQuest.NS_Dialog.Test
{
    class Trigger : MonoBehaviour
    {
        public DialogTest test;
        public bool isNear;

        private void OnTriggerEnter(Collider other)
        {
            test.isIn = true;
            test.isNear = isNear;
        }

        private void OnTriggerStay(Collider other)
        {
            test.isIn = true;
            test.isNear = isNear;
        }

        private void OnTriggerExit(Collider other)
        {
            test.isIn = false;
            test.isNear = !isNear;
        }
    }
}*/