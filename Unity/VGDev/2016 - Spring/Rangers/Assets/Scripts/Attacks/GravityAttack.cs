using Assets.Scripts.Timers;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    public abstract class GravityAttack : SpawnAttack
    {
        public static GravityAttack instance;

        [SerializeField]
        protected GameObject gravityAttackManager;
        [SerializeField]
        protected bool world;
        [SerializeField]
        private float aliveTime = 7f;
        [SerializeField]
        LayerMask mask;

        void OnEnable()
        {
            GravityAttackManager.Recalculate += CalculateObjects;
        }
        void OnDisable()
        {
            GravityAttackManager.Recalculate -= CalculateObjects;
        }

        void Start()
        {
            Timer t = gameObject.AddComponent<Timer>();
            t.TimeOut += new Timer.TimerEvent(Final);
            t.Initialize(aliveTime, "Gravity Effect");

            CalculateObjects();
        }

        private void CalculateObjects()
        {
            if (GravityAttackManager.instance == null) Instantiate(gravityAttackManager);
            Collider[] cols = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius, mask);
            for(int i = 0; i < cols.Length; i++)
            {
                GameObject g = cols[i].transform.root.gameObject;
                if (!GravityAttackManager.instance.AlreadyInList(g))
                {
                    GravityAttackManager.instance.AddObject(g);
                    Affect(g);
                }
            }
        }

        private void ReleaseObjects()
        {
            if (GravityAttackManager.instance == null) Instantiate(gravityAttackManager);
            for(int i = 0; i < GravityAttackManager.instance.AffectedObjects.Count; i++)
            {
                Unaffect(GravityAttackManager.instance.AffectedObjects[i]);
            }
        }

        protected abstract void Affect(GameObject g);
        protected abstract void Unaffect(GameObject g);

        protected void Final(Timer t)
        {
            ReleaseObjects();
            DestroyImmediate(gameObject);
            GravityAttackManager.instance.Reset();
        }

        void OnTriggerEnter(Collider col)
        {
            CalculateObjects();
        }
    }
}
