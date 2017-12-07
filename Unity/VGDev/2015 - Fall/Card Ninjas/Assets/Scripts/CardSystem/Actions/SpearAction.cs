using UnityEngine;
using Assets.Scripts.Player;
using Assets.Scripts.Util;

namespace Assets.Scripts.CardSystem.Actions
{
    class SpearAction : Action
    {
        public override void useCard(Character actor)
        {
            if (actor.Direction == Enums.Direction.Left)
                spawnObjectUsingPrefabAsModel(damage, range, .2f, true, Util.Enums.Direction.Left, 10, 1, true, actor.CurrentNode.Left, actor);
            if (actor.Direction == Enums.Direction.Right)
                spawnObjectUsingPrefabAsModel(damage, range, .2f, true, Util.Enums.Direction.Right, 10, 1, true, actor.CurrentNode.Right, actor);
        }
    }
}