using UnityEngine;
using Assets.Scripts.Util;
using Assets.Scripts.Data;
using Assets.Scripts.Player;
using Assets.Scripts.Attacks;

namespace Assets.Scripts.Arrows
{
	/// <summary>
	/// Arrow property that transfers all enabled arrow properties from player that is
	/// hit to the player who shot the arrow
	/// </summary>
	public class VirusArrow : SpawnerProperty
	{
		public override void Effect(PlayerID hitPlayer)
		{
			// If a player was hit
			if (hitPlayer != 0)
			{
				base.Effect(hitPlayer);
				spawnedReference.GetComponent<VirusAttack>().UpdatePlayerInfo(fromPlayer, hitPlayer);
			}
		}
	}
}