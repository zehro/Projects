using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple Logical object which represents a NOT gate
// (returning the opposite of its input.)

namespace YeggQuest.NS_Logic
{
    public class LogicalNOT : Logical
    {
        public Logical input;

        public override bool Evaluate()
        {
            return !Logic.SafeEvaluate(input, true);
        }

        void OnDrawGizmos()
        {
            Logic.Visualize(transform, input);
            Gizmos.DrawIcon(transform.position, "LogicalNOT.png");
        }
    }
}