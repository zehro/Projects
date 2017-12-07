using UnityEngine;
using Assets.Scripts.Grid;
using System;

namespace Assets.Scripts.Enemies
{
    class Met : Enemy
    {
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private MeshRenderer body;

        private Player.Player player;
        private float turn = 0;

        protected override void Initialize()
        {
            player = FindObjectOfType<Player.Player>();
        }

        protected override void Render(bool render)
        {
            body.enabled = render;
        }

        protected override void RunAI()
        {
            turn += Time.deltaTime;
            if(turn > 1f)
            {
                turn = 0;
                if(player.CurrentNode.Position.x < currentNode.Position.x)
                {
                    if (!currentNode.Up.Occupied)
                    {
                        currentNode.clearOccupied();
                        currentNode = currentNode.Up;
                        currentNode.Owner = (this);
                    }
                }
                else if (player.CurrentNode.Position.x > currentNode.Position.x)
                {
                    if (!currentNode.Down.Occupied)
                    {
                        currentNode.clearOccupied();
                        currentNode = currentNode.Down;
                        currentNode.Owner = (this);
                    }
                }
                else
                {
                    Weapons.Hitbox b = Instantiate(bullet).GetComponent<Weapons.Hitbox>();
                    b.transform.position = currentNode.Left.transform.position;
                    b.CurrentNode = currentNode.Left;
                }
            }
            transform.position = currentNode.transform.position;
        }
    }
}
