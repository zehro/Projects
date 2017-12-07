using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Player;

namespace Assets.Scripts.Arrows
{
	/// <summary>
	/// Arrow property that gives life to the player who shot it when it hits another player.
	/// </summary>
	public class LifestealArrow : ArrowProperty
	{
		public override void Effect(PlayerID hitPlayer)
		{
			// If a player was hit
			if (hitPlayer != 0)
			{

				Controller sourceController = GameManager.instance.GetPlayer(fromPlayer);
				float damage = GetComponent<ArrowController>().Damage;
				sourceController.LifeComponent.ModifyHealth(damage);
			}
		}
	}
}

