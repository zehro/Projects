using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Weapons.Projectiles
{
    class BackNForth : Hitbox
    {
        [SerializeField]
        protected int bouncesAllowed;

        private int bounceCount = 0;

        public override void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.Battle)
            {
                if (moveCompleted)
                {
                    switch (direction)
                    {
                        case Enums.Direction.Left:
                            if (currentNode.panelExists(Enums.Direction.Left))
                                target = currentNode.Left;
                            else
                            {
                                if (bounceCount >= bouncesAllowed)
                                {
                                    dead = true;
                                }
                                direction = Enums.Direction.Right;
                                bounceCount++;
                            }
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Right:
                            if (currentNode.panelExists(Enums.Direction.Right))
                                target = currentNode.Right;
                            else
                            {
                                if (bounceCount >= bouncesAllowed)
                                {
                                    dead = true;
                                }
                                direction = Enums.Direction.Left;
                                bounceCount++;
                            }
                            moveCompleted = false;
                            break;
                    }
                }
                if (dead)
                {
                    Destroy(this.gameObject);
                }
                Move();
            }
        }
    }
}
