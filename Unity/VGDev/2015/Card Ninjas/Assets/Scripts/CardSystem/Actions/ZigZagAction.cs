using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.CardSystem.Actions
{
    class ZigZagAction : Action
    {
        public override void useCard(Character actor)
        {
            Weapons.Projectiles.ZigZag temp = MonoBehaviour.Instantiate(prefab).GetComponent<Weapons.Projectiles.ZigZag>();
            temp.Damage = damage;
            temp.TimesCanPierce = range - 1;
            temp.Owner = actor.gameObject;
            Util.AddElement.AddElementByEnum(temp.gameObject, element, true);
            if (actor.Direction == Util.Enums.Direction.Left)
            {
                temp.Direction = Util.Enums.Direction.Left;
                temp.transform.position = actor.CurrentNode.Left.transform.position;
                temp.CurrentNode = actor.CurrentNode.Left;
                Transform model = temp.GetComponentInChildren<Transform>();
                model.localScale = new Vector3(model.localScale.x, -model.localScale.y, model.localScale.z);
            }

            if (actor.Direction == Util.Enums.Direction.Right)
            {
                temp.Direction = Util.Enums.Direction.Right;
                temp.transform.position = actor.CurrentNode.Right.transform.position;
                temp.CurrentNode = actor.CurrentNode.Right;
            }
        }
    }
}
