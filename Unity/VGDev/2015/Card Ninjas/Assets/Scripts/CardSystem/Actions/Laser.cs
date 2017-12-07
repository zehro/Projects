using UnityEngine;
using Assets.Scripts.Player;
using System.Collections.Generic;
using Assets.Scripts.Grid;

namespace Assets.Scripts.CardSystem.Actions
{
    class Laser : Action
    {
        public override void useCard(Character actor)
        {
            List<GridNode> strikenNodes = new List<GridNode>();
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                strikenNodes.Add(actor.CurrentNode.Left);
                while (strikenNodes[strikenNodes.Count - 1].panelExists(Util.Enums.Direction.Left))
                {
                    strikenNodes.Add(strikenNodes[strikenNodes.Count - 1].Left);
                }
                foreach (GridNode node in strikenNodes)
                {
                    spawnObjectUsingPrefabAsModel(damage, 9, .4f, true, Util.Enums.Direction.None, 0, 0, true, node, actor);
                }
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                strikenNodes.Add(actor.CurrentNode.Right);
                while (strikenNodes[strikenNodes.Count - 1].panelExists(Util.Enums.Direction.Right))
                {
                    strikenNodes.Add(strikenNodes[strikenNodes.Count - 1].Right);
                }
                foreach (GridNode node in strikenNodes)
                {
                    spawnObjectUsingPrefabAsModel(damage, 9, .4f, true, Util.Enums.Direction.None, 0, 0, true, node, actor);
                }
            }
        }
    }
}
