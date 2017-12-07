using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.CardSystem.Actions
{
    class Heal : Action
    {
        public override void useCard(Character actor)
        {
            actor.AddHealth(damage);
        }
    }
}
