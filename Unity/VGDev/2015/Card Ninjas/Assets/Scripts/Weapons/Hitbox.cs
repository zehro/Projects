using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Weapons
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField]
        protected int damage = 10;
        [SerializeField]
        protected int distance = 3;
        [SerializeField]
        protected float deathTime = 3;
        [SerializeField]
        protected bool piercing = true;
        [SerializeField]
        protected Enums.Direction direction = Enums.Direction.None;
        [SerializeField]
        protected float speed = 20;
        [SerializeField]
        protected int timesCanPierce = 2;
        [SerializeField]
        protected bool isFlying = true;
        [SerializeField]
        private Util.Enums.Element element;
        protected GameObject owner;

        protected bool dead = false;
        protected bool moveCompleted = true;
        protected float pos = 0;
        protected GridNode currentNode;
        protected GridNode target;

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public float DeathTime
        {
            get { return deathTime; }
            set { deathTime = value; }
        }

        public bool Piercing
        {
            get { return piercing; }
            set { piercing = value; }
        }

        public Enums.Direction Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        
        public int TimesCanPierce
        {
            get { return timesCanPierce; }
            set { timesCanPierce = value; }
        }

        public bool IsFlying
        {
            get { return isFlying; }
            set { isFlying = value; }
        }

        public GridNode CurrentNode
        {
            get { return currentNode; }
            set { currentNode = value; }
        }

        public GameObject Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public Util.Enums.Element Element
        {
            get { return element; }
            set { element = value; }
        }

        public virtual void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.Battle)
            {
                if (moveCompleted)
                {
                    switch (direction)
                    {
                        case Enums.Direction.Up:
                            if (isFlying)
                            {
                                if (currentNode.panelExists(Enums.Direction.Up))
                                    target = currentNode.Up;
                                else
                                    dead = true;
                            }
                            else
                            {
                                if (currentNode.panelNotDestroyed(Enums.Direction.Up))
                                    target = currentNode.Up;
                                else
                                    dead = true;
                            }
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Down:
                            if (isFlying)
                            {
                                if (currentNode.panelExists(Enums.Direction.Down))
                                    target = currentNode.Down;
                                else
                                    dead = true;
                            }
                            else
                            {
                                if (currentNode.panelNotDestroyed(Enums.Direction.Down))
                                    target = currentNode.Down;
                                else
                                    dead = true;
                            }
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Left:
                            if (isFlying)
                            {
                                if (currentNode.panelExists(Enums.Direction.Left))
                                    target = currentNode.Left;
                                else
                                    dead = true;
                            }
                            else
                            {
                                if (currentNode.panelNotDestroyed(Enums.Direction.Left))
                                    target = currentNode.Left;
                                else
                                    dead = true;
                            }
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Right:
                            if (isFlying)
                            {
                                if (currentNode.panelExists(Enums.Direction.Right))
                                    target = currentNode.Right;
                                else
                                    dead = true;
                            }
                            else
                            {
                                if (currentNode.panelNotDestroyed(Enums.Direction.Right))
                                    target = currentNode.Right;
                                else
                                    dead = true;
                            }
                            moveCompleted = false;
                            break;
                        default: deathTime -= Time.deltaTime; break;
                    }
                }
                if (deathTime < 0 || dead || distance == 0)
                    Destroy(this.gameObject);
                Move();
            }
        }

        public virtual void OnTriggerEnter(Collider collider)
        {
            Hitbox h = collider.gameObject.GetComponent<Hitbox>();
            if (h != null)
            {
                if(h.owner == owner)
                {
                    Physics.IgnoreCollision(this.GetComponent<Collider>(), collider);
                    return;
                }
            }
            if (!piercing || timesCanPierce == 0)
                dead = true;
            else
                timesCanPierce--;
        }

        protected void Move()
        {
            if(!moveCompleted)
                transform.position = Vector3.Lerp(currentNode.transform.position, target.transform.position, pos = pos + Time.deltaTime * speed);
            if(pos > 1)
            {
                moveCompleted = true;
                transform.position = target.transform.position;
                currentNode = target;
                distance--;
                pos = 0;
            }
        }
    }
}
