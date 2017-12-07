using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Enemies
{
    class TomahawkSamuraiMan : Enemy
    {
        [SerializeField]
        private Weapons.Hitbox bullet;
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
            mechAnima.SetBool("HoriSlash", true);
        }

        protected override void RunAI()
        {
            if (Attacking && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0 && mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("SamuraiHorizontalSlash"))
            {
                Attacking = false;
                mechAnima.SetBool("Attack", false);
            }
            if (Hop && mechAnima.GetCurrentAnimatorClipInfo(0).Length > 0 && (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveBegin") || (mechAnima.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals("MoveEnd"))))
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

                    mechAnima.SetBool("Hurt", false);
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
                        } else if(currentNode.Left.Down != null && !currentNode.Left.Down.Occupied && !currentNode.Down.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Down;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;
                        }
                        else if(currentNode.Left.Up != null && !currentNode.Left.Up.Occupied && !currentNode.Up.Occupied)
                        {
                            currentNode.clearOccupied();//Say we aren't here
                            currentNode = currentNode.Up;//Say we're there
                            currentNode.Owner = (this);//Tell the place we own it.
                            mechAnima.SetBool("Hop", true);
                            Hop = true;
                        }
                    }
                    //If player is above us
                    else if (player.CurrentNode.Position.x < currentNode.Position.x)
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
                    else
                    {
                        AnimatorClipInfo[] temp = mechAnima.GetCurrentAnimatorClipInfo(0);
                        if (temp.Length > 0 && temp[0].clip.name.Equals("SamuraiWait1"))
                        {
                            Attacking = true;
                            mechAnima.SetBool("Attack", true);
                            sfx.PlaySong(0);
                            Weapons.Hitbox b = Instantiate(bullet).GetComponent<Weapons.Hitbox>();
                            b.transform.position = currentNode.Left.transform.position;
                            b.CurrentNode = currentNode.Left;
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
