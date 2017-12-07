using UnityEngine;
using System.Collections;


namespace Assets.Scripts.Enemies
{
    class Tornado_Samurai : Enemy
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


        Weapons.TornadoHitbox bulletHolster;



        protected override void Initialize()
        {
            player = FindObjectOfType<Player.Player>();
            mechAnima.GetComponent<Animator>();
            mechAnima.SetBool("VertiSlash", true);
            bulletHolster = bullet.GetComponent<Weapons.TornadoHitbox>();
        }

        protected override void RunAI()
        {
            if (Attacking && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0 && mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("SamuraiVerticalSlash"))
            {
                Attacking = false;
                mechAnima.SetBool("Attack", false);

                Weapons.TornadoHitbox b = Instantiate(bullet).GetComponent<Weapons.TornadoHitbox>();
                b.transform.position = bulletHolster.transform.position;
                b.CurrentNode = bulletHolster.CurrentNode;
                b.zTargetDistance =  bulletHolster.zTargetDistance;
                b.xTargetDistance = bulletHolster.xTargetDistance;
                b.zStartingPoint = bulletHolster.zStartingPoint;
                b.xStartingPoint = bulletHolster.xStartingPoint;
            }

            if(Hop && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0 && (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveBegin") || (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveEnd"))))
            {
                Hop = false;
                mechAnima.SetBool("Hop", false);
                transform.position = currentNode.transform.position;

            }
            //We change turns each second
            turn += Time.deltaTime;
            if (!hit)
            {
                if (turn > 3f)
                {
                    mechAnima.SetBool("Attack", false);

                    turn = 0;

                    if (currentNode.Left.Type == Util.Enums.FieldType.Blue)
                    {
                        if (!currentNode.Left.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Left;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;

                        }
                        else if (currentNode.Left.Down != null && !currentNode.Left.Down.Occupied && !currentNode.Down.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Down;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;

                        }
                        else if (currentNode.Left.Up != null && !currentNode.Left.Up.Occupied && !currentNode.Up.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Up;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;

                        }
                    }
                    else if(currentNode.Up != null && !currentNode.Up.Occupied)
                    {
                        currentNode.clearOccupied();//Say we aren't here
                        currentNode = currentNode.Up;//Say we're there
                        currentNode.Owner = (this);//Tell the place we own it.
                        mechAnima.SetBool("Hop", true);
                        Hop = true;

                    }
                    //If they are in front of us, ATTACK!.
                    else
                    {
                        AnimatorClipInfo[] temp = mechAnima.GetCurrentAnimatorClipInfo(0);
                        if (temp.Length > 0 && temp[0].clip.name.Equals("SamuraiWait1"))
                        {
                            Attacking = true;
                            mechAnima.SetBool("Attack", true);
                            sfx.PlaySong(0);

                            Grid.GridNode t = currentNode;

                            do
                            {
                                t = t.Left;
                            } while (t.Left != null && t.Left.Type != Util.Enums.FieldType.Red);

                            while (t.Up != null)
                            {
                                t = t.Up;
                            }

                            t = t.Right;

                            if(t == currentNode)
                            {
                                t = t.Left;
                            }

                            Grid.GridNode s = t;

                            while(s.Down != null)
                            {
                                s = s.Down;
                            }


                            bulletHolster.transform.position = t.transform.position;
                            bulletHolster.CurrentNode = t;
                            bulletHolster.zTargetDistance = player.transform.position.z;
                            bulletHolster.xTargetDistance = s.transform.position.x;
                            bulletHolster.zStartingPoint = t.transform.position.z;
                            bulletHolster.xStartingPoint = t.transform.position.x;

                        }
                    }
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

