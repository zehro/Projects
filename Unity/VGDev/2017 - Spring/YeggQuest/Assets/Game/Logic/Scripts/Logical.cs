using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The most generic definition of a Logical object.
// It is attached to a GameObject and can be evaluated to retrieve a boolean.

namespace YeggQuest.NS_Logic
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public abstract class Logical : MonoBehaviour
    {
        public abstract bool Evaluate();

        void Update()
        {
            gameObject.name = GetType().Name;
        }
    }
}