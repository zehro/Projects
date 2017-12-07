using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Attacks
{
    public class GravityAttackManager : MonoBehaviour
    {
        public static GravityAttackManager instance;

        public delegate void Calculation();
        public static event Calculation Recalculate;

        private List<GameObject> affectedObjects;

        void Awake()
        {
            instance = this;
            affectedObjects = new List<GameObject>();
        }

        internal void AddObject(GameObject g)
        {
            affectedObjects.Add(g);
        }

        internal void Reset()
        {
            affectedObjects.Clear();
            if (Recalculate != null) Recalculate();
        }

        internal bool AlreadyInList(GameObject g)
        {
            return affectedObjects.Contains(g);
        }

        internal List<GameObject> AffectedObjects
        {
            get { return affectedObjects; }
        }
    }
}
