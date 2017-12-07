using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.CardSystem.Actions
{
    class Oil : Action
    {
        public override void useCard(Character actor)
        {
            int randHolder;
            bool repeater = false;
            List<GridNode> strikenNodes = new List<GridNode>();
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                if (range != 4)
                    spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left, actor);
                if (range == 2)
                {
                    if (actor.CurrentNode.Left.panelExists(Util.Enums.Direction.Left))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left.Left, actor);
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Up))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left.Left.Up, actor);
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Down))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left.Left.Down, actor);
                }
                else if (range == 3)
                {
                    if (actor.CurrentNode.Left.panelExists(Util.Enums.Direction.Left))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left.Left, actor);
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Left))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Left.Left.Left, actor);
                }
                else if (range == 4)
                {
                    GameObject[] enemyNodes = GameObject.FindGameObjectsWithTag("Red");
                    while (strikenNodes.Count < enemyNodes.Length/2)
                    {
                        randHolder = (int)Random.Range(0, enemyNodes.Length);
                        foreach (GridNode node in strikenNodes)
                            repeater = repeater || (node == (enemyNodes[randHolder].GetComponent<GridNode>()));
                        if (repeater == false)
                            strikenNodes.Add(enemyNodes[randHolder].GetComponent<GridNode>());
                        else
                            repeater = false;
                    }
                    foreach(GridNode node in strikenNodes)
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, node , actor);
                }
            }

            if (actor.Direction == Util.Enums.Direction.Right)
            {
                if (range != 4)
                    spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right, actor);
                if (range == 2)
                {
                    if (actor.CurrentNode.Right.panelExists(Util.Enums.Direction.Right))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right.Right, actor);
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Up))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right.Right.Up, actor);
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Down))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right.Right.Down, actor);
                }
                else if (range == 3)
                {
                    if (actor.CurrentNode.Right.panelExists(Util.Enums.Direction.Right))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right.Right, actor);
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Right))
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, actor.CurrentNode.Right.Right.Right, actor);
                }
                else if (range == 4)
                {
                    GameObject[] enemyNodes = GameObject.FindGameObjectsWithTag("Blue");
                    while (strikenNodes.Count < enemyNodes.Length / 2)
                    {
                        randHolder = (int)Random.Range(0, enemyNodes.Length);
                        foreach (GridNode node in strikenNodes)
                            repeater = repeater || (node == (enemyNodes[randHolder].GetComponent<GridNode>()));
                        if (repeater == false)
                            strikenNodes.Add(enemyNodes[randHolder].GetComponent<GridNode>());
                        else
                            repeater = false;
                    }
                    foreach (GridNode node in strikenNodes)
                        spawnObjectUsingPrefabAsModel(damage, 9, 5f, false, Util.Enums.Direction.None, 0, 0, false, node, actor);
                }
            }
        }
    }
}
