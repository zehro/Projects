using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Enemies
{
    class Wave : MonoBehaviour
    {
        [SerializeField]
        private Enemy[] enemies;
        [SerializeField]
        private Vector2[] spawnPositions;

        internal GameObject[] SpawnWave()
        {
            List<GameObject> enemyList = new List<GameObject>();
            Enemy temp;
            for( int i = 0; i< enemies.Length; i++)
            {
                temp = Instantiate(enemies[i]);
                temp.RowStart = (int)spawnPositions[i].x;
                temp.ColStart = (int)spawnPositions[i].y;
                enemyList.Add(temp.gameObject);
            }
            return enemyList.ToArray();
        }
    }
}
