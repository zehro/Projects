using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple Logical object which represents an AND gate
// (returning true iff all of its inputs are true.)

namespace YeggQuest.NS_Logic
{
    public class LogicalAND : Logical
    {
        public Logical[] inputs;
        public bool inverted;

        public override bool Evaluate()
        {
            bool result = true;

            if (inputs != null)
            {
                foreach (Logical i in inputs)
                {
                    if (i != this)
                        result &= Logic.SafeEvaluate(i, true);
                }
            }

            return (inverted ? !result : result);
        }

        void OnDrawGizmos()
        {
            Logic.Visualize(transform, inputs);
            Gizmos.DrawIcon(transform.position, inverted ? "LogicalNAND.png" : "LogicalAND.png");
        }
    }
}