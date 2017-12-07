using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.CardSystem.Actions
{
    class Lightning : Action
    {
        public override void useCard(Character actor)
        {
            List<GridNode> strikenNodes = new List<GridNode>();
            int randHolder;
            bool repeater = false;
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                GameObject[] enemyNodes = GameObject.FindGameObjectsWithTag("Red");
                while (strikenNodes.Count < range)
                {
                    randHolder = (int)Random.Range(0, enemyNodes.Length);
                    foreach(GridNode node in strikenNodes)
                        repeater = repeater||(node == (enemyNodes[randHolder].GetComponent<GridNode>()));
                    if (repeater == false)
                        strikenNodes.Add(enemyNodes[randHolder].GetComponent<GridNode>());
                    else
                        repeater = false;
                }
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                GameObject[] enemyNodes = GameObject.FindGameObjectsWithTag("Blue");
                while (strikenNodes.Count < range)
                {
                    randHolder = (int)Random.Range(0, enemyNodes.Length);
                    foreach (GridNode node in strikenNodes)
                        repeater = repeater||(node == enemyNodes[randHolder]);
                    if (repeater == false)
                        strikenNodes.Add(enemyNodes[randHolder].GetComponent<GridNode>());
                    else
                        repeater = false;
                }
            }
            foreach (GridNode node in strikenNodes)
            {
                spawnObjectUsingPrefabAsModel(damage, 9, .2f, false, Util.Enums.Direction.None, 10, 0, true, node, actor);
            }
        }
    }
}