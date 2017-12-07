using UnityEngine;
using System.Collections;
using Assets.Scripts.Data;
using Assets.Scripts.Util;

namespace Assets.Scripts.Arrows
{
	/// <summary>
	/// Arrow property that gives life to the player who shot it when it hits another player.
	/// </summary>
	public class SplittingArrow : ArrowProperty
	{
		[SerializeField]
		private float delay = 0.25f;
		[SerializeField]
		private float spread = 0.25f;

		public override void Init()
		{
			base.Init();
			StartCoroutine(InitHelper());
		}

		private IEnumerator InitHelper()
		{
			yield return new WaitForSeconds(delay);

			if(GetComponent<Rigidbody>() != null)
			{
				Vector3 velocityCopy = GetComponent<Rigidbody>().velocity;
				Vector3 backVect = Vector3.back;
				Vector3 orthogonal = Vector3.up;
				Vector3.OrthoNormalize(ref velocityCopy, ref backVect, ref orthogonal);

				GameObject upArrow = NewArrow();
				upArrow.transform.position += orthogonal * spread;
				GameObject downArrow = NewArrow();
				downArrow.transform.position -= orthogonal * spread;
			}
		}

		private GameObject NewArrow()
		{
			GameObject newArrow = (GameObject) Instantiate(gameObject, transform.position, transform.rotation);
			newArrow.GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity;
			int types = GameManager.instance.GetPlayer(fromPlayer).ArcheryComponent.ArrowTypes;
			types = Bitwise.ClearBit(types, (int) Enums.Arrows.Splitting);
			newArrow.GetComponent<Arrows.ArrowController>().InitArrow(types, fromPlayer);
			return newArrow;
		}

		public override void Effect(PlayerID hitPlayer) { }
	}
}

