using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A utility class for logic.

namespace YeggQuest.NS_Logic
{
    public static class Logic
    {
        // Utility method which evaluates a Logical object safely,
        // by returning the given default value if it doesn't exist.

        public static bool SafeEvaluate(Logical logical, bool defaultValue)
        {
            if (logical != null)
                return logical.Evaluate();
            else
                return defaultValue;
        }
        
        // Utility method which visualizes the connection between an owning object
        // and its single Logical input by drawing an arrow between them.

        public static void Visualize(Transform owner, Logical input)
        {
            if (owner != null && input != null)
                Yutil.DrawArrow(input.transform.position, owner.position, (input.Evaluate() ? Color.yellow : Color.black), 0.6f);
        }

        // Same as above, but for an owning object with multiple Logical inputs.

        public static void Visualize(Transform owner, Logical[] inputs)
        {
            if (inputs == null)
                return;
            foreach (Logical i in inputs)
                Visualize(owner, i);
        }
    }
}