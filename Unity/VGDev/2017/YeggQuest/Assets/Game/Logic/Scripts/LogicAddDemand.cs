using UnityEngine;

namespace YeggQuest.NS_Logic
{
    public class LogicAddDemand : Logical
    {
        public LogicalPaintDemand[] A;
        public LogicalPaintDemand[] B;
        public int requiredResult;

        public override bool Evaluate()
        {
            int a = 0;
            foreach(LogicalPaintDemand button in A)
            {
                bool val = button.Evaluate();
                a |= val ? 1 : 0;
                a = a << 1;
            }
            a = a >> 1;

            int b = 0;
            foreach (LogicalPaintDemand button in B)
            {
                bool val = button.Evaluate();
                b |= val ? 1 : 0;
                b = b << 1;
            }
            b = b >> 1;
            return a + b == requiredResult;
        }
    }
}
