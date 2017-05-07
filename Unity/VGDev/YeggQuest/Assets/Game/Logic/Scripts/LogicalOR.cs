using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple Logical object which represents an OR gate
// (returning true iff any one of its inputs are true.)

namespace YeggQuest.NS_Logic
{
    public class LogicalOR : Logical
    {
        public Logical[] inputs;
        public bool inverted;

        public override bool Evaluate()
        {
            bool result = false;

            if (inputs != null)
            {
                foreach (Logical i in inputs)
                {
                    if (i != this)
                        result |= Logic.SafeEvaluate(i, false);
                }
            }

            return (inverted ? !result : result);
        }

        void OnDrawGizmos()
        {
            Logic.Visualize(transform, inputs);
            Gizmos.DrawIcon(transform.position, inverted ? "LogicalNOR.png" : "LogicalOR.png");
        }
    }
}