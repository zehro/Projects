using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Weapons.Projectiles
{
    class ZigZag : Hitbox
    {
        private Enums.Direction upOrDown = Enums.Direction.Down;

        public override void Update()
        {
            if (Managers.GameManager.State == Enums.GameStates.Battle)
            {
                if (moveCompleted)
                {
                    switch (direction)
                    {
                        case Enums.Direction.Left:
                            if (upOrDown == Enums.Direction.Down && !currentNode.panelExists(Enums.Direction.Down))
                                upOrDown = Enums.Direction.Up;
                            else if (upOrDown == Enums.Direction.Up && !currentNode.panelExists(Enums.Direction.Up))
                                upOrDown = Enums.Direction.Down;
                            if (currentNode.panelExists(Enums.Direction.Left) && currentNode.Left.panelExists(Enums.Direction.Down) && upOrDown == Enums.Direction.Down)
                                target = currentNode.Left.Down;
                            else if (currentNode.panelExists(Enums.Direction.Left) && currentNode.Left.panelExists(Enums.Direction.Up) && upOrDown == Enums.Direction.Up)
                                target = currentNode.Left.Up;
                            else
                            {
                                dead = true;
                            }
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Right:
                            if (upOrDown == Enums.Direction.Down && !currentNode.panelExists(Enums.Direction.Down))
                                upOrDown = Enums.Direction.Up;
                            else if (upOrDown == Enums.Direction.Up && !currentNode.panelExists(Enums.Direction.Up))
                                upOrDown = Enums.Direction.Down;
                            if (currentNode.panelExists(Enums.Direction.Right) && currentNode.Right.panelExists(Enums.Direction.Down) && upOrDown == Enums.Direction.Down)
                                target = currentNode.Right.Down;
                            else if (currentNode.panelExists(Enums.Direction.Right) && currentNode.Right.panelExists(Enums.Direction.Up) && upOrDown == Enums.Direction.Up)
                                target = currentNode.Right.Up;
                            else
                            {
                                dead = true;
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
