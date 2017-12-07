using UnityEngine;

namespace Assets.Scripts.Weapons.Projectiles
{
    class Tornado : Hitbox
    {
        public void OnCollisionEnter(Collision col)
        {
            Hitbox h = col.gameObject.GetComponent<Hitbox>();
            if (h != null)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), col.collider);
                return;
            }
            if (col.gameObject.tag == "Enemy")
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), col.collider);
                return;
            }
        }
        public override void OnTriggerEnter(Collider collider)
        {
            Hitbox h = collider.gameObject.GetComponent<Hitbox>();
            if (h != null)
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), collider);
                return;
            }
            if (collider.tag == "Enemy")
            {
                Physics.IgnoreCollision(this.GetComponent<Collider>(), collider);
                return;
            }
        }
    }
}
