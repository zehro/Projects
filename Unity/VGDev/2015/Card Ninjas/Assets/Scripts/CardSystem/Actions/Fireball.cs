using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.CardSystem.Actions
{
    class Fireball : Action
    {
        public override void useCard(Character actor)
        {
            if (actor.Direction == Util.Enums.Direction.Left)
                if (actor.CurrentNode.panelExists(Util.Enums.Direction.Left))
                    spawnObjectUsingPrefabAsModel(damage, 9, .2f, true, Util.Enums.Direction.Left, 10, range, true, actor.CurrentNode.Left, actor, true);

            if (actor.Direction == Util.Enums.Direction.Right)
                if (actor.CurrentNode.panelExists(Util.Enums.Direction.Right))
                    spawnObjectUsingPrefabAsModel(damage, 9, .2f, true, Util.Enums.Direction.Right, 10, range, true, actor.CurrentNode.Right, actor, true);
        }
    }
}