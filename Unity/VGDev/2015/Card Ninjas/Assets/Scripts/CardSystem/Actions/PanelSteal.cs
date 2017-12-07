using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.CardSystem.Actions
{
    class PanelSteal : Action
    {
        public override void useCard(Character actor)
        {
            GridNode currentNode = actor.CurrentNode;
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                if (range == 1)
                {
                    while (currentNode.Left.Type == Util.Enums.FieldType.Blue)
                    {
                        currentNode = currentNode.Left;
                    }
                    if (!(currentNode.Occupied))
                    {
                        currentNode.Left.Type = Util.Enums.FieldType.Blue;
                    }
                }
                if (range == 3)
                {
                    if (!(currentNode.panelExists(Util.Enums.Direction.Up)))
                    {
                        currentNode = currentNode.Down;
                    }
                    if (!(currentNode.panelExists(Util.Enums.Direction.Down)))
                    {
                        currentNode = currentNode.Up;
                    }
                    GridNode[] stolenPanels = new GridNode[3];
                    while (currentNode.Left.Type == Util.Enums.FieldType.Blue && currentNode.Left.Up.Type == Util.Enums.FieldType.Blue && currentNode.Left.Down.Type == Util.Enums.FieldType.Blue)
                    {
                        currentNode = currentNode.Left;
                    }
                    stolenPanels[0] = currentNode.Left;
                    stolenPanels[1] = currentNode.Left.Up;
                    stolenPanels[2] = currentNode.Left.Down;
                    foreach (GridNode node in stolenPanels)
                    {
                        if (!(node.Occupied))
                        {
                            node.Type = Util.Enums.FieldType.Blue;
                        }
                    }
                }
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                if (range == 1)
                {
                    while (currentNode.Right.Type == Util.Enums.FieldType.Red)
                    {
                        currentNode = currentNode.Right;
                    }
                    if (!(currentNode.Occupied))
                    {
                        currentNode.Right.Type = Util.Enums.FieldType.Red;
                    }
                }
                if (range == 3)
                {
                    if (!(currentNode.panelExists(Util.Enums.Direction.Up)))
                    {
                        currentNode = currentNode.Down;
                    }
                    if (!(currentNode.panelExists(Util.Enums.Direction.Down)))
                    {
                        currentNode = currentNode.Up;
                    }
                    GridNode[] stolenPanels = new GridNode[3];
                    while (currentNode.Right.Type == Util.Enums.FieldType.Red && currentNode.Right.Up.Type == Util.Enums.FieldType.Red && currentNode.Right.Down.Type == Util.Enums.FieldType.Red)
                    {
                        currentNode = currentNode.Right;
                    }
                    stolenPanels[0] = currentNode.Right;
                    stolenPanels[1] = currentNode.Right.Up;
                    stolenPanels[2] = currentNode.Right.Down;
                    foreach (GridNode node in stolenPanels)
                    {
                        if (!(node.Occupied))
                        {
                            node.Type = Util.Enums.FieldType.Red;
                        }
                    }
                }
            }
        }
    }
}