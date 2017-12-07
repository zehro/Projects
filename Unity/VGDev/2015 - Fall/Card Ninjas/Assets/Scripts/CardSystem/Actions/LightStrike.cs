using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.CardSystem.Actions
{
    class LightStrike : Action
    {
        public override void useCard(Character actor)
        {
            List<GridNode> strikenNodes = new List<GridNode>();
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                if (actor.CurrentNode.Left.panelExists(Util.Enums.Direction.Left))
                {
                    strikenNodes.Add(actor.CurrentNode.Left.Left);
                }
                if (range == 4)
                {
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Left.Up);
                    }
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Left.Down);
                    }
                    if (actor.CurrentNode.panelExists(Util.Enums.Direction.Left))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left);
                    }
                    if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Left))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Left.Left);
                    }
                }
                if (range == 5)
                {
                    if (actor.CurrentNode.Left.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Up);
                    }
                    if (actor.CurrentNode.Left.Left.Left.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Left.Left.Down);
                    }
                    if (actor.CurrentNode.Left.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Down);
                    }
                    if (actor.CurrentNode.Left.Left.Left.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Left.Left.Left.Up);
                    }
                }
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                if (actor.CurrentNode.Right.panelExists(Util.Enums.Direction.Right))
                {
                    strikenNodes.Add(actor.CurrentNode.Right.Right);
                }
                if (range == 4)
                {
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Right.Up);
                    }
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Right.Down);
                    }
                    if (actor.CurrentNode.panelExists(Util.Enums.Direction.Right))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right);
                    }
                    if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Right))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Right.Right);
                    }
                }
                if (range == 5)
                {
                    if (actor.CurrentNode.Right.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Up);
                    }
                    if (actor.CurrentNode.Right.Right.Right.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Right.Right.Down);
                    }
                    if (actor.CurrentNode.Right.panelExists(Util.Enums.Direction.Down))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Down);
                    }
                    if (actor.CurrentNode.Right.Right.Right.panelExists(Util.Enums.Direction.Up))
                    {
                        strikenNodes.Add(actor.CurrentNode.Right.Right.Right.Up);
                    }
                }
            }
            foreach (GridNode node in strikenNodes)
            {
                spawnObjectUsingPrefabAsModel(damage, 9, .2f, false, Util.Enums.Direction.None, 10, 0, true, node, actor);
            }
        }
    }
}