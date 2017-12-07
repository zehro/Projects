using UnityEngine;
using Assets.Scripts.CardSystem;
using Assets.Scripts.Util;

namespace Assets.Scripts.UI
{
    public class DeckTransfer : MonoBehaviour
    {
        private Deck deck;

        private Enums.Element element;

        private bool accessedDeck = false, accessedElement = false;

        public Deck Deck
        {
            get
            {
                accessedDeck = true;
                if (accessedElement)
                    Destroy(this.gameObject);
                return deck;
            }
            set { deck = value; }
        }

        public Enums.Element Element
        {
            get
            {
                accessedElement = true;
                if (accessedDeck)
                    Destroy(this.gameObject);
                return element;
            }
            set { element = value; }
        }
    }
}