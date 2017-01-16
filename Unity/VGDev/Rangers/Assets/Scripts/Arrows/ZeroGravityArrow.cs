using Assets.Scripts.Attacks;

namespace Assets.Scripts.Arrows
{
    /// <summary>
    /// Arrow property that creates an area where there is no gravity.
    /// </summary>
    public class ZeroGravityArrow : SpawnerProperty
    {
        public override void Init()
        {
            base.Init();
        }

        public override void Effect(PlayerID hitPlayer)
        {
            base.Effect(hitPlayer);
            spawnedReference.GetComponent<ZeroGravityAttack>().UpdatePlayerInfo(fromPlayer, hitPlayer);
        }
    } 
}
