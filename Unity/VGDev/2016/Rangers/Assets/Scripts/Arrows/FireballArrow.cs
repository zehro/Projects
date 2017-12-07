using Assets.Scripts.Attacks;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Arrow property that spawns an explosion on impact.
    /// </summary>
    public class FireballArrow : SpawnerProperty
    {
        public override void Init()
        {
            base.Init();
        }

        public override void Effect(PlayerID hitPlayer)
        {
            base.Effect(hitPlayer);
			spawnedReference.GetComponent<FireballAttack>().UpdatePlayerInfo(fromPlayer, hitPlayer);
        }
    }
}