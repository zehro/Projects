using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;
using System.Collections.Generic;

namespace Assets.Scripts.CardSystem.Actions
{
    class GrowTree : Action
    {
        public override void useCard(Character actor)
        {
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                if (actor.CurrentNode.Left.Left.panelExists(Util.Enums.Direction.Left))
                {
                    if (!(actor.CurrentNode.Left.Left.Left.Occupied))
                    {
                        actor.CurrentNode.Left.Left.Left.Type = Util.Enums.FieldType.Blue;
                        spawnObjectUsingPrefabAsModel(0, 0, 6f, true, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Left.Left.Left, actor);
                    }
                    else
                    {
                        spawnObjectUsingPrefabAsModel(damage, 0, 6f, false, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Left.Left.Left, actor);
                    }
                }
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                if (actor.CurrentNode.Right.Right.panelExists(Util.Enums.Direction.Right))
                {
                    if (!(actor.CurrentNode.Right.Right.Right.Occupied))
                    {
                        actor.CurrentNode.Right.Right.Right.Type = Util.Enums.FieldType.Red;
                        spawnObjectUsingPrefabAsModel(0, 0, 6f, true, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Right.Right.Right, actor);
                    }
                    spawnObjectUsingPrefabAsModel(damage, 0, 6f, false, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Right.Right.Right, actor);
                }
            }
        }
    }
}