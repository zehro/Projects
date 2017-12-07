using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using YeggQuest.NS_Bird;

namespace YeggQuest.NS_Generic
{
    public class RespawnPlane : MonoBehaviour
    {
        private GameCoordinator coordinator;    // The game coordinator.
        private Bird bird;                      // The bird.

        void Awake()
        {
            coordinator = FindObjectOfType<GameCoordinator>();
            bird = FindObjectOfType<Bird>();
        }

        void Update()
        {
            if (bird.GetPosition().y < transform.position.y)
                coordinator.RespawnInScene();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.Lerp(Color.red, Color.black, 0.5f);

            int incr = 50;
            int size = 500;
            float y = transform.position.y;

            for (int i = -size; i <= size; i += incr)
            {
                Gizmos.DrawLine(new Vector3(i, y, -size), new Vector3(i, y, +size));
                Gizmos.DrawLine(new Vector3(-size, y, i), new Vector3(+size, y, i));
            }
        }
    }
}