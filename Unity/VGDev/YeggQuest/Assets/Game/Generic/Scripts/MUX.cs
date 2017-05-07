using UnityEngine;
using YeggQuest.NS_Logic;

namespace YeggQuest.NS_Generic
{
    public class MUX : MonoBehaviour
    {
        [Header("Input")]
        public LogicalPaintDemand firstDigit;
        public LogicalPaintDemand secondDigit;

        [Space(2)]
        [Header("Output")]
        public LogicalConstant switch1;
        public LogicalConstant switch2;
        public LogicalConstant switch3;

        public void Update()
        {
            bool a = false, b = false, c = false;
            int val = firstDigit.Evaluate() ? 2 : 0;
            val |= secondDigit.Evaluate() ? 1 : 0;
            switch(val)
            {
                case 1: a = true; break;
                case 2: b = true; break;
                case 3: c = true; break;
            }
            switch1.on = a;
            switch2.on = b;
            switch3.on = c;
        }
    }
}
