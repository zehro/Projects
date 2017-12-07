﻿namespace ShiftingDungeon.Dungeon.Spawners
{
    using UnityEngine;
    using ObjectPooling;
    using Util;

    public class EnemySpawner : Spawner
    {
        [SerializeField]
        private Enums.EnemyTypes type;

        private GameObject enemy;

        public override void Spawn()
        {
            if (this.enemy == null)
            {
                this.enemy = EnemyPool.Instance.GetEnemy(this.type);
                this.enemy.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, Constants.ROOM_PART_Z_DEPTH - 2);
                enemy.transform.rotation = this.transform.rotation;
            }
        }

        public override void Return()
        {
            if(this.enemy != null)
            {
                EnemyPool.Instance.ReturnEnemy(this.type, this.enemy);
                this.enemy = null;
            }
        }

        public override bool isAlive()
        {
            return this.enemy != null && this.enemy.activeInHierarchy;
        }
    }
}
