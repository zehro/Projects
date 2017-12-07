using UnityEngine;
using System.Collections;
using Assets.Scripts.Weapons;
using Assets.Scripts.Player;
using Assets.Scripts.Weapons.Projectiles;
using System.Collections.Generic;

namespace Assets.Scripts.Enemies
{
	class TenguArmorEnemy : Enemy
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
		private bool isHurt = false;

		[SerializeField]
		private int tornadoTimerMax = 4;

		[SerializeField]
		private Tornado tornado;

		[SerializeField]
		private Player.Player player;

		protected override void Initialize()
		{
			//meleeHitbox.Owner = this.gameObject;
			//meleeHitbox.DeathTime = .5f;

			//meleeEffect.transform.Rotate(Vector3.up, 180f);

			transform.position = currentNode.transform.position;
			player = FindObjectOfType<Player.Player>();

			tornadoTimerMax = Random.Range(2, 4);
	}

		protected override void RunAI()
		{

			if (animDone)
			{
				if (shouldAttack)
				{
					//If we should attack, then go ahead and attack!
					Attack();
				}
				else if (isHurt)
				{
					isHurt = false;
					shouldAttack = false;
					//We've been hurt! We need to switch spots!
					randoMove();
				}
				else
				{
					if (turn < tornadoTimerMax)
					{
						//Just wait here until we should attack.
						turn++;
					}
					else
					{
						shouldAttack = true;//Set us up to attack
						tornadoTimerMax = Random.Range(2, 4);//Set our new timer
						turn = 0;//Reset our turn timer
					}
				}
			}


		}

		protected override void OnTriggerEnterAI(Collider col)
		{
            anim.SetTrigger("Hurt");
			isHurt = true;
			animDone = false;
		}

		protected override void Render(bool render)
		{
			GetComponentInChildren<SkinnedMeshRenderer>().enabled = render;
		}

		public void Attack()
		{
			//print("ATTACK");
			anim.SetTrigger("FanSwipe");
			//GameObject.Instantiate(tornado, new Vector3(currentNode.Left.transform.position.x, currentNode.Left.transform.position.y, currentNode.Left.transform.position.z), Quaternion.identity);
			Weapons.Hitbox b = Instantiate(tornado).GetComponent<Weapons.Hitbox>();
			b.transform.position = currentNode.Left.transform.position;
			b.CurrentNode = currentNode.Left;
			//GameObject.Instantiate(meleeEffect, new Vector3(currentNode.Left.transform.position.x, currentNode.Left.transform.position.y + 2, currentNode.Left.transform.position.z), Quaternion.identity);
			shouldAttack = false;
			sfx.PlaySong(0);
		}

		public void changeSpot()
		{
			//anim.SetTrigger("MoveBegin");
			mdec = (Util.Enums.Direction)Random.Range(0, 3);

			if (mdec == Util.Enums.Direction.Up)
			{
				//If we're randomly going up
				if (currentNode.panelAllowed(Util.Enums.Direction.Up, Type) || currentNode.Right.Occupied)
				{
					currentNode.clearOccupied();//Say we aren't here
					currentNode = currentNode.Up;//Say we're there
					currentNode.Owner = (this);//Tell the place we own it.
				}
			}

			else if (currentNode.panelAllowed(Util.Enums.Direction.Right, Type) || currentNode.Right.Occupied)
			{
				currentNode.clearOccupied();//Say we aren't here
				currentNode = currentNode.Right;//Say we're there
				currentNode.Owner = (this);//Tell the place we own it.
			}

			else if (currentNode.panelAllowed(Util.Enums.Direction.Left, Type) || currentNode.Right.Occupied)
			{
				currentNode.clearOccupied();//Say we aren't here
				currentNode = currentNode.Left;//Say we're there
				currentNode.Owner = (this);//Tell the place we own it.
				
			}

			else if (currentNode.panelAllowed(Util.Enums.Direction.Down, Type) || currentNode.Right.Occupied)
			{
				//If we're randomly going down
				if (currentNode.Up.Occupied == false)
				{
					currentNode.clearOccupied();//Say we aren't here
					currentNode = currentNode.Down;//Say we're there
					currentNode.Owner = (this);//Tell the place we own it.
				}
			}
			anim.SetTrigger("MoveEnd");
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
			anim.SetTrigger("MoveEnd");
		}
	}
}