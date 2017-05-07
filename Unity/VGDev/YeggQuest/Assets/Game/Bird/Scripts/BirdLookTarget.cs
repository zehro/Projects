using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YeggQuest.NS_Bird
{
    public class BirdLookTarget : MonoBehaviour
    {
        public float radius = 5;

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}