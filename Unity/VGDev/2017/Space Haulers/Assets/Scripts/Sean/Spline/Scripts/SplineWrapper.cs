using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

// SplineWrapper is an abstract class which automatically creates a Spline out of its
// transform children, and exposes both it and two functions - CustomSetup and CustomTeardown -
// so that a specific wrapper can be defined that does something with that spline in the editor.
// The functions are called so that the spline data is always correctly updated and trashed.

// TODO: better comments everywhere in this

namespace YeggQuest.NS_Spline
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class SplineWrapper : MonoBehaviour
    {
        public Spline spline;
        [Range(2, 20)]
        public int precision = 20;
        [Range(0f, 1f)]
        public float gap = 0f;

        // Listeners for changes

        void OnEnable()
        {
            Setup();
        }

        void OnDisable()
        {
            Teardown();
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                bool stale = false;
                foreach (SplineNode node in GetComponentsInChildren<SplineNode>())
                    stale |= node.CustomUpdate();
                if (stale)
                    Setup();
            }
        }

        void OnValidate()
        {
            if (!Application.isPlaying)
                Setup();
        }

        void OnTransformChildrenChanged()
        {
            if (!Application.isPlaying)
                Setup();
        }

        // Setup function

        public virtual void Setup()
        {
            Teardown();

            if (transform.childCount >= 2)
            {
                SplineNode[] nodes = new SplineNode[transform.childCount];

                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject child = transform.GetChild(i).gameObject;
                    child.name = "P" + i;

                    if (child.GetComponent<SplineNode>() == null)
                        child.AddComponent<SplineNode>();
                    nodes[i] = child.GetComponent<SplineNode>();
                }

                spline = new Spline(nodes, precision, gap);
            }
        }

        // Teardown function

        public virtual void Teardown()
        {
            spline = null;
        }

        void OnDrawGizmos()
        {
            if (spline != null)
                spline.Draw(transform);
        }

        // ======================================================================================================================== GETTERS

        internal SplineLerpResult Lerp(SplineLerpQuery query)
        {
            if (spline != null)
                return spline.Lerp(transform, query);
            return new SplineLerpResult();
        }

        internal Transform GetEntrance()
        {
            if (spline != null)
                return spline.GetEntrance();
            return null;
        }

        internal Transform GetExit()
        {
            if (spline != null)
                return spline.GetExit();
            return null;
        }

        internal float GetLength()
        {
            if (spline != null)
                return spline.GetLength();
            return 0;
        }
    }
}