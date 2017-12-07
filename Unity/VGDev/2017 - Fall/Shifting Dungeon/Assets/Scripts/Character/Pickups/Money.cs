﻿namespace ShiftingDungeon.Character.Pickups
{
    using UnityEngine;
    using Hero;
    using ObjectPooling;

    public class Money : MonoBehaviour, IPoolable
    {
        public int Value {
            get {
                if (this.transform.localScale.x == .25f)
                    return 1;
                else if (this.transform.localScale.x == .5f)
                    return 5;
                else
                    return 10;
            }
        }

        private Transform hero;
        private int referenceIndex;

        private void Update()
        {
            if (this.hero != null && !this.hero.GetComponent<HeroBehavior>().IsDead)
            {
                Vector3 dir = this.hero.position - this.transform.position;
                this.transform.Translate(dir.normalized * Time.deltaTime * (1f / dir.magnitude) * 2f);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == Util.Enums.Tags.Hero.ToString())
            {
                this.hero = collision.gameObject.transform;
            }
        }
        
        public IPoolable SpawnCopy(int referenceIndex)
        {
            Money gold = Instantiate<Money>(this);
            gold.referenceIndex = referenceIndex;
            return gold;
        }

        public int GetReferenceIndex()
        {
            return this.referenceIndex;
        }

        public GameObject GetGameObject()
        {
            return this.gameObject;
        }

        public void Initialize()
        {
            this.hero = null;
        }

        public void ReInitialize()
        {
            this.hero = null;
            this.gameObject.SetActive(true);
        }

        public void Deallocate()
        {
            this.hero = null;
            this.gameObject.SetActive(false);
        }

        public void Delete()
        {
            Destroy(this.gameObject);
        }
    }
}
