using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Weapons.Projectiles
{
    class GroundDestroyer : Hitbox
    {
        public override void Update()
        {

            if (Managers.GameManager.State == Enums.GameStates.Battle)
            {
                if (moveCompleted)
                {
                    switch (direction)
                    {
                        case Enums.Direction.Up:
                            if (currentNode.panelExists(Enums.Direction.Up))
                                target = currentNode.Up;
                            else
                                dead = true;
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Down:
                            if (currentNode.panelExists(Enums.Direction.Down))
                                target = currentNode.Down;
                            else
                                dead = true;
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Left:
                            if (currentNode.panelExists(Enums.Direction.Left))
                                target = currentNode.Left;
                            else
                                dead = true;
                            moveCompleted = false;
                            break;
                        case Enums.Direction.Right:
                            if (currentNode.panelExists(Enums.Direction.Right))
                                target = currentNode.Right;
                            else
                                dead = true;
                            moveCompleted = false;
                            break;
                        default: deathTime -= Time.deltaTime; break;
                    }
                }
                if (deathTime < 0 || dead || distance == 0)
                {
                    if (!currentNode.Occupied)
                        currentNode.Type = Enums.FieldType.Destroyed;
                    Destroy(this.gameObject);
                }
                Move();
            }
        }
    }
}
