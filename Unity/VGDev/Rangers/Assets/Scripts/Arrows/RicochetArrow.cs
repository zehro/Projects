
namespace Assets.Scripts.Arrows
{
    public class RicochetArrow : ArrowProperty
    {
        /// <summary>
        /// Number of bounces an arrow can undergo before stopping
        /// </summary>
        public static int bounces = 4;

        public override void Effect(PlayerID hitPlayer) { }
    } 
}
