using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A LogicalConstant is the simplest Logical object, consisting of an
// exposed toggleable boolean that can hook into any logic. It shows up
// as a battery in the editor.

namespace YeggQuest.NS_Logic
{
    public class LogicalConstant : Logical
    {
        public bool on = true;

        public override bool Evaluate()
        {
            return on;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, on ? "LogicalConstantOn.png" : "LogicalConstantOff.png");
        }
    }
}