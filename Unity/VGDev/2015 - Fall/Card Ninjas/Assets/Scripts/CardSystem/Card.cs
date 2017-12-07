using System;
using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.CardSystem.Actions;

namespace Assets.Scripts.CardSystem
{
    public class Card
    {
        private Sprite image;
        private Enums.CardTypes type;
        private Enums.Element element;
        private Actions.Action action;
        private string name;
        private string description;

        public Enums.Element Element
        {
            get { return element; }
        }

        public Enums.CardTypes Type
        {
            get { return type; }
        }

        public Actions.Action Action
        {
            get { return action; }
        }

        public string Name
        {
            get { return name; }
        }

        public string Description
        {
            get { return description; }
        }

        public Sprite Image
        {
            get { return image; }
        }

        public Card(string name, Weapons.Hitbox hitbox, string element, string type, int range, int damage, string actionType, GameObject prefab, string description, Sprite image)
        {
            this.name = name;
            SetElement(element);
            SetType(type);
            SetAction(hitbox, actionType, range, damage, prefab);
            this.description = description;
            this.image = image;
        }

        private void SetElement(string type)
        {
            try
            {
                Enums.Element element = (Enums.Element)Enum.Parse(typeof(Enums.Element), type);
                if (Enum.IsDefined(typeof(Enums.Element), element) | element.ToString().Contains(","))
                    this.element = element;
                else
                {
                    this.element = Enums.Element.None;
                    Debug.LogError("Unable to resolve " + type + " to a type.  Setting " + name + " to type None.");
                }
            }
            catch (ArgumentException)
            {
                this.type = Enums.CardTypes.Error;
                Debug.LogError("Unable to resolve " + type + " to a type.  Setting " + name + " to type Error.");
            }
        }

        private void SetType(string type)
        {
            try
            {
                Enums.CardTypes cardType = (Enums.CardTypes)Enum.Parse(typeof(Enums.CardTypes), type);
                if (Enum.IsDefined(typeof(Enums.CardTypes), cardType) | cardType.ToString().Contains(","))
                    this.type = cardType;
                else
                {
                    this.type = Enums.CardTypes.Error;
                    Debug.LogError("Unable to resolve " + type + " to a type.  Setting " + name + " to type Error.");
                }
            }
            catch (ArgumentException)
            {
                this.type = Enums.CardTypes.Error;
                Debug.LogError("Unable to resolve " + type + " to a type.  Setting " + name + " to type Error.");
            }
        }

        private void SetAction(Weapons.Hitbox hitbox, string actionType, int range, int damage, GameObject prefab)
        {
            try
            {
                action = (Actions.Action)Activator.CreateInstance(null, "Assets.Scripts.CardSystem.Actions." + actionType).Unwrap();
                action.HitBox = hitbox;
                action.Range = range;
                action.Damage = damage;
                action.Prefab = prefab;
                action.Element = this.element;
            }
            catch (Exception e)
            {
                action = new Error();
                Debug.LogError(e.Message + ": for " + actionType + ".  Setting " + name + "'s action to Error.");
            }
        }
    }
}
