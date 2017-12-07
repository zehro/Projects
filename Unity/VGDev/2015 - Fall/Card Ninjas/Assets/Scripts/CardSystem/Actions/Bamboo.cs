using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Grid;

namespace Assets.Scripts.CardSystem.Actions
{
    class Bamboo : Action
    {
        public override void useCard(Character actor)
        {
            GridNode currentNode = actor.CurrentNode;
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                while (currentNode.panelExists(Util.Enums.Direction.Left))
                {
                    currentNode = currentNode.Left;
                }
                spawnObjectUsingPrefabAsModel(damage, 9, .5f, false, Util.Enums.Direction.None, 10, 0, true, currentNode, actor);
            }
            if (actor.Direction == Util.Enums.Direction.Right)
            {
                while (currentNode.panelExists(Util.Enums.Direction.Right))
                {
                    currentNode = currentNode.Right;
                }
                spawnObjectUsingPrefabAsModel(damage, 9, .5f, false, Util.Enums.Direction.None, 10, 0, true, currentNode, actor);
            }
        }
    }
}