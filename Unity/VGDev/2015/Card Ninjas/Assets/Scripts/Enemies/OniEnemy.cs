using UnityEngine;
using Assets.Scripts.Weapons;
using System.Collections.Generic;

namespace Assets.Scripts.Enemies
{
    class OniEnemy : Enemy
    {
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private Util.SoundPlayer sfx;
        [SerializeField]
        private Util.Enums.Direction mdec = Util.Enums.Direction.Left;
        [SerializeField]
        private float turn = 0;
        [SerializeField]
        private bool shouldAttack = false;
        [SerializeField]
        private enum attackType { Melee, Projectile }
        [SerializeField]
        private attackType attackStyle = attackType.Melee;

		[SerializeField]
		private int stepTurn = 3;

		[SerializeField]
		private Hitbox meleeHitbox;
		[SerializeField]
		private GameObject meleeEffect;

        private Player.Player player;

        protected override void Initialize()
        {
            meleeHitbox.Owner = this.gameObject;
            meleeHitbox.DeathTime = .5f;

            meleeEffect.transform.Rotate(Vector3.up, 180f);

            transform.position = currentNode.transform.position;
            player = FindObjectOfType<Player.Player>();
            stepTurn = Random.Range(1, 4);
		}

        protected override void RunAI()
        {

            if (animDone)
            {
                if (shouldAttack)
                {
                    Attack();
                }
                else if (currentNode.Type != Type)
                {
                    randoMove();
                }
                else
                {
                    if (turn < stepTurn)
                    {
                        changeSpot();
                    }
                    else
                    {
                        stepToPlayer();
                        shouldAttack = true;
                        turn = 0;
                        stepTurn = Random.Range(2, 4);
					}
                }
            }


        }

        protected override void Render(bool render)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = render;
        }

        public void Attack()
        {
            anim.SetTrigger("AttackTrigger");
            GameObject.Instantiate(meleeHitbox, new Vector3(currentNode.Left.transform.position.x, currentNode.Left.transform.position.y, currentNode.Left.transform.position.z - 1.5f), Quaternion.identity);

            GameObject.Instantiate(meleeEffect, new Vector3(currentNode.Left.transform.position.x, currentNode.Left.transform.position.y + 2, currentNode.Left.transform.position.z), Quaternion.identity);
            shouldAttack = false;
            sfx.PlaySong(0);
        }

        public void changeSpot()
        {
            anim.SetTrigger("MoveBeginTrigger");
            mdec = (Util.Enums.Direction)Random.Range(0, 3);

            if (mdec == Util.Enums.Direction.Up)
            {
                //If we're randomly going up
                if (currentNode.panelAllowed(Util.Enums.Direction.Up, Type) && !currentNode.Up.Occupied)
                {
                    currentNode.clearOccupied();//Say we aren't here
                    currentNode = currentNode.Up;//Say we're there
                    currentNode.Owner = (this);//Tell the place we own it.
                }
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Right, Type) && !currentNode.Right.Occupied)
            {
                currentNode.clearOccupied();//Say we aren't here
                currentNode = currentNode.Right;//Say we're there
                currentNode.Owner = (this);//Tell the place we own it.
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Left, Type) && !currentNode.Left.Occupied)
            {
                currentNode.clearOccupied();//Say we aren't here
                currentNode = currentNode.Left;//Say we're there
                currentNode.Owner = (this);//Tell the place we own it.
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Down, Type) && !currentNode.Down.Occupied)
            {
                    currentNode.clearOccupied();//Say we aren't here
                    currentNode = currentNode.Down;//Say we're there
                    currentNode.Owner = (this);//Tell the place we own it.  
            }
            anim.SetTrigger("MoveEndTrigger");
            transform.position = currentNode.transform.position;
            turn++;
        }

		public void randoMove()
		{
			GameObject[] tiles = GameObject.FindGameObjectsWithTag("Blue");
			List<Grid.GridNode> usableTiles = new List<Grid.GridNode>();
			Grid.GridNode n;
			foreach (GameObject g in tiles)
			{
				n = g.GetComponent<Grid.GridNode>();
				if (!n.Occupied)
					usableTiles.Add(n);
			}
			int tile = usableTiles.Count > 0 ? Random.Range(0, usableTiles.Count - 1) : -1;
			if (tile == -1)
			{

			}
			else
			{
				currentNode.clearOccupied();
				currentNode = usableTiles[tile];
				currentNode.Owner = (this);
				transform.position = currentNode.transform.position;
			}
			anim.SetTrigger("MoveEndTrigger");
		}

		public void stepToPlayer()
        {

            currentNode = player.CurrentNode.Right;
            anim.SetTrigger("MoveEndTrigger");
            transform.position = currentNode.transform.position;
        }

        public void returnToField()
        {
            currentNode = grid[rowStart, colStart];
            mdec = (Util.Enums.Direction)Random.Range(0, 3);

            if (mdec == Util.Enums.Direction.Up)
            {
                //If we're randomly going up
                if (currentNode.panelAllowed(Util.Enums.Direction.Up, Type))
                {
                    currentNode.clearOccupied();//Say we aren't here
                    currentNode = currentNode.Up;//Say we're there
                    currentNode.Owner = (this);//Tell the place we own it.
                }
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Right, Type))
            {
                currentNode.clearOccupied();//Say we aren't here
                currentNode = currentNode.Right;//Say we're there
                currentNode.Owner = (this);//Tell the place we own it.
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Left, Type))
            {
                currentNode.clearOccupied();//Say we aren't here
                currentNode = currentNode.Left;//Say we're there
                currentNode.Owner = (this);//Tell the place we own it.
            }

            else if (currentNode.panelAllowed(Util.Enums.Direction.Down, Type))
            {
                //If we're randomly going down
                if (currentNode.Up.Occupied == false)
                {
                    currentNode.clearOccupied();//Say we aren't here
                    currentNode = currentNode.Down;//Say we're there
                    currentNode.Owner = (this);//Tell the place we own it.
                }
            }
            transform.position = currentNode.transform.position;
        }
    }
}