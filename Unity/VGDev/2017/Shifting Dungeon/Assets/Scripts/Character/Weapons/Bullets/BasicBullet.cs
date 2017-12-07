namespace ShiftingDungeon.Character.Weapons.Bullets
{
    using UnityEngine;

    public class BasicBullet : Bullet
    {
        protected override void LocalUpdate()
        {
        }

        protected override void LocalInitialize()
        {
        }

        protected override void LocalReInitialize()
        {
        }
        protected override void LocalDeallocate()
        {
        }

        protected override void LocalDelete()
        {
        }

        protected override bool ShouldDestroyBullet(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<Bullet>() != null)
                return false;

            return true;
        }
    }
}
