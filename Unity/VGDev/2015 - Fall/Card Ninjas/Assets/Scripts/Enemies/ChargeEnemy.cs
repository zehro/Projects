using UnityEngine;
using System.Collections;


namespace Assets.Scripts.Enemies
{
    class ChargeEnemy : Enemy
    {
        [SerializeField]
        private GameObject bullet;
        [SerializeField]
        private SkinnedMeshRenderer[] body;
        [SerializeField]
        private Util.SoundPlayer sfx;

        private Player.Player player;
        private float turn = 0;
        public Animator mechAnima;
        bool Attacking;
        bool ResetingPosition;
        bool Hop;

        protected override void Initialize()
        {
            player = FindObjectOfType<Player.Player>();
            mechAnima.GetComponent<Animator>();
            mechAnima.SetBool("Stab", true);
        }

        protected override void RunAI()
        {
            if (Hop && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0 && (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveBegin") || (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveEnd"))))
            {
                Hop = false;
                mechAnima.SetBool("Hop", false);
                transform.position = currentNode.transform.position;

            }
            //We change turns each second
            turn += Time.deltaTime;
            if (!hit && (!Attacking && !ResetingPosition))
            {
                
                if (turn > 1f)
                {
                    //mechAnima.SetBool("Hop", false);

                    turn = 0;
                    //If player is above us
                    if (player.CurrentNode.Position.x < currentNode.Position.x)
                    {
                        //Check if we can move up.
                        if (!currentNode.Up.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Up;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;
                        }
                    }
                    //If player is above us
                    else if (player.CurrentNode.Position.x > currentNode.Position.x)
                    {
                        //Check if we can move up.
                        if (!currentNode.Down.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Down;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;
                        }
                    }
                    //If they are in front of us, ATTACK!.
                    else if (this.transform.position.z == currentNode.transform.position.z && this.transform.position.x == currentNode.transform.position.x)
                    {
                        AnimatorClipInfo[] temp = mechAnima.GetCurrentAnimatorClipInfo(0);
                        if (temp.Length > 0 && temp[0].clip.name.Equals("SamuraiWait1"))
                        {
                            mechAnima.SetBool("Attack", true);
                            sfx.PlaySong(0);
                            Attacking = true;
                        }
                    }
                }
            }
            else if (Attacking)
            {
                if (!mechAnima.GetBool("HoldAttack") && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0)
                {
                    if (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("SamuraiStabEnter"))
                    {
                        mechAnima.SetBool("HoldAttack", true);
                        mechAnima.SetBool("Attack", false);
                    }
                }



                if ((player.transform.position.z + 2f) < this.transform.position.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f * Time.deltaTime);
                }
                else
                {
                    ResetingPosition = true;
                    mechAnima.SetBool("Attack", false);
                    mechAnima.SetBool("HoldAttack", false);
                    mechAnima.SetBool("WithdrawAttack", true);
                    Attacking = false;

                    Grid.GridNode t = currentNode;

                    while (t.transform.position.z >= this.transform.position.z && t.Left != null)
                    {
                        t = t.Left;
                    }

                    Weapons.Hitbox b = Instantiate(bullet).GetComponent<Weapons.Hitbox>();

                    b.transform.position = t.transform.position;
                    b.CurrentNode = t;
                }
            }
            else if (ResetingPosition)
            {
                AnimatorClipInfo[] temp = mechAnima.GetCurrentAnimatorClipInfo(0);

                if (currentNode.transform.position.z > this.transform.position.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 6f * Time.deltaTime);
                }
                else if (temp.Length > 0 && (temp[0].clip.name.Equals("SamuraiStabExit") || temp[0].clip.name.Equals(("SamuraiWait1"))))
                {
                    mechAnima.SetBool("WithdrawAttack", false);
                    ResetingPosition = false;
                    transform.position = currentNode.transform.position;
                }


            }
            else
            {
                mechAnima.SetBool("Hurt", true);
                turn = 0;
            }

        }

        protected override void Render(bool render)
        {
            foreach (SkinnedMeshRenderer b in body)
                b.enabled = render;
        }
    }
}

