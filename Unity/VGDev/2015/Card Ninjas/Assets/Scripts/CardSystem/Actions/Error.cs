using UnityEngine;

namespace Assets.Scripts.CardSystem.Actions
{
    class Error : Action
    {
        public override void useCard(Player.Character actor)
        {
            Debug.Log("I am Error.");
        }
    }
}
