using UnityEngine;

namespace Assets.Scripts.Level
{
	public class Bouncy : MonoBehaviour
	{
		[SerializeField]
		private float strength = 10;

		void OnCollisionEnter(Collision col)
		{
			Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();

			if(rb != null)
			{
				Vector3 orthogonal = Vector3.Normalize(transform.localToWorldMatrix * Vector3.up);
				Vector3 velProjection = rb.velocity - (Vector3.Dot(rb.velocity, orthogonal) / Vector3.Dot(orthogonal, orthogonal)) * orthogonal;
				rb.velocity = velProjection + orthogonal * strength;
			}
		}
	}
}
