using UnityEngine;
using Assets.Scripts.Grid;

namespace Assets.Scripts.Enemies
{
    abstract class Enemy : Player.Character
    {
        protected abstract void Initialize();
        protected abstract void RunAI();
        protected abstract void Render(bool render);

        protected bool hit = false;
        protected bool animDone = false;
        private bool paused = false;
        private float animSpeed = 0;
        private float invulerability = 0;
        private float invulerabilityTime = 1f;
        private bool render = true;

        void Start()
        {
            grid = FindObjectOfType<GridManager>().Grid;
            currentNode = grid[rowStart, colStart];
            currentNode.Owner = this;
            transform.position = currentNode.transform.position;
            Initialize();
        }

        void Update()
        {
            if (Managers.GameManager.State == Util.Enums.GameStates.Battle && !stun)
            {
                if (paused)
                {
                    paused = false;
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().speed = animSpeed;
                }
                RunAI();
                if (hit)
                    hit = false;
                if (invulerability > 0)
                {
                    render = !render;
                    Render(render);
                    invulerability -= Time.deltaTime;
                }
                else if (!render)
                {
                    render = true;
                    Render(true);
                }
                if (animDone)
                    animDone = false;
            }
            else
            {
                if (!paused)
                {
                    if (GetComponent<Animator>() != null)
                    {
                        animSpeed = GetComponent<Animator>().speed;
                        GetComponent<Animator>().speed = 0.0000001f;
                    }
                    paused = true;
                }
                if (stun)
                {
                    if ((stunTimer += Time.deltaTime) > stunTime)
                    {
                        stunTimer = 0f;
                        stun = false;
                    }
                }
            }
        }

        public void AnimDetector()
        {
            animDone = true;
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Enemy")
                Physics.IgnoreCollision(this.GetComponent<Collider>(), col);
            Weapons.Hitbox hitbox = col.gameObject.GetComponent<Weapons.Hitbox>();
            if (hitbox != null)
            {
                if (invulerability <= 0)
                {
                    hit = true;
                    TakeDamage(hitbox.Damage, hitbox.Element);
                    invulerability = invulerabilityTime;
                }
            }
            Weapons.Projectiles.Stun s = col.gameObject.GetComponent<Weapons.Projectiles.Stun>();
            if (s != null)
                Stun = true;
            OnTriggerEnterAI(col);
        }

        protected virtual void OnTriggerEnterAI(Collider col)
        {

        }
    }
}
