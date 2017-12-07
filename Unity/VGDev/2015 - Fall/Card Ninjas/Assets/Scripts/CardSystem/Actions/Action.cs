using UnityEngine;

namespace Assets.Scripts.CardSystem.Actions
{
    public abstract class Action
    {
        protected Weapons.Hitbox hitbox;
        protected int range, damage;
        protected GameObject prefab;
        protected Util.Enums.Element element;

        public Weapons.Hitbox HitBox
        {
            set { hitbox = value; }
        }

        public GameObject Prefab
        {
            set { prefab = value; }
        }

        public Util.Enums.Element Element
        {
            set { element = value; }
        }

        public int Range
        {
            get { return range; }
            set { range = value; }
        }
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public abstract void useCard(Player.Character actor);

        protected void spawnObjectUsingPrefabAsModel(int damage, int distance, float deathTime, bool piercing, Util.Enums.Direction direction, float speed, int timesCanPierce, bool isFlying, Grid.GridNode spawnPosition, Player.Character actor, bool changeMaterial = false)
        {
            Weapons.Hitbox temp = MonoBehaviour.Instantiate(hitbox);
            GameObject model = MonoBehaviour.Instantiate(prefab);
            if(actor.Direction == Util.Enums.Direction.Left)
                model.transform.localScale = new Vector3(model.transform.localScale.x, -model.transform.localScale.y, model.transform.localScale.z);
            model.transform.parent = temp.transform;
            temp.Damage = damage;
            temp.Distance = distance == 0 ? 1 : distance;
            temp.DeathTime = deathTime;
            temp.Piercing = piercing;
            temp.Direction = direction;
            temp.Speed = speed;
            temp.TimesCanPierce = timesCanPierce;
            temp.IsFlying = isFlying;
            temp.Owner = actor.gameObject;
            temp.Element = element;
            Util.AddElement.AddElementByEnum(temp.gameObject, element, changeMaterial);
            temp.CurrentNode = spawnPosition;
            temp.transform.position = spawnPosition.transform.position;
        }
    }
}
