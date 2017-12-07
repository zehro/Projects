using UnityEngine;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Player
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        protected int health = 100;
        protected const int MAX_HEALTH = 100;
        [SerializeField]
        protected int rowStart = 1;
        [SerializeField]
        protected int colStart = 1;
        [SerializeField]
        protected Util.Enums.Direction direction = Util.Enums.Direction.Left;
        [SerializeField]
        private Util.Enums.FieldType type = Util.Enums.FieldType.Red;
        [SerializeField]
        protected Util.Enums.Element element;
        [SerializeField]
        protected float stunTime = 1f;

        protected bool stun = false;
        protected float stunTimer = 0f;

        protected GridNode[,] grid;
        protected GridNode currentNode;

        public GridNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public Util.Enums.Direction Direction
        {
            get { return direction; }
        }

        public Util.Enums.FieldType Type
        {
            get { return type; }
        }

        public Util.Enums.Element Element
        {
            get { return element; }
        }

        public int RowStart
        {
            set { rowStart = value; }
        }
        public int ColStart
        {
            set { colStart = value; }
        }

        public bool Stun
        {
            get { return stun; }
            set { stun = value; }
        }


        protected bool invincible = false;

        void Start()
        {
            grid = FindObjectOfType<GridManager>().Grid;
        }
        
        public virtual void TakeDamage(int damage, Util.Enums.Element incommingElement)
        {
            if (!invincible)
            {
                health = health - (int)(damage * Util.Elements.GetDamageMultiplier(element, incommingElement));
                if (health <= 0)
                {
                    currentNode.clearOccupied();
                    Destroy(this.gameObject);
                }
            }
        }

        public virtual void AddHealth(int health)
        {
            this.health = Mathf.Clamp(this.health + health, 0, MAX_HEALTH);
        }
    }
}