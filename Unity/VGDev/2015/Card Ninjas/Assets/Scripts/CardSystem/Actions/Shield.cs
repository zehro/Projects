using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.CardSystem.Actions
{
    class Shield : Action
    {
        public override void useCard(Character actor)
        {
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                if (actor.CurrentNode.panelExists(Util.Enums.Direction.Left))
                    spawnObjectUsingPrefabAsModel(0, 0, 5f, true, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Left, actor);
            }

            if (actor.Direction == Util.Enums.Direction.Right)
            {
                if (actor.CurrentNode.panelExists(Util.Enums.Direction.Right))
                    spawnObjectUsingPrefabAsModel(0, 0, 5f, true, Util.Enums.Direction.None, 0, damage, true, actor.CurrentNode.Right, actor);
            }
        }
    }
}
