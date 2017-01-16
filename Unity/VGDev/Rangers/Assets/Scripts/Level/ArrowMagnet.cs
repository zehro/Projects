using UnityEngine;

namespace Assets.Scripts
{
	public class ArrowMagnet : MonoBehaviour
	{
		private enum MagnetModes { RepelVariable = -2, RepelConstant, Inert, AttractConstant, AttractVariable }

		[SerializeField]
		private MagnetModes mode = MagnetModes.AttractVariable;
		[SerializeField]
		private float effectRadius = 10;
		[SerializeField]
		private float strength = 0.5f;

		private int mask = 0;

		void Start()
		{
			mask = LayerMask.GetMask("Arrow");
			//maybe some sort of effect to display the zone of effect?
		}

		void FixedUpdate()
		{
			Collider[] cols = Physics.OverlapSphere(transform.position, effectRadius, mask);
			foreach(Collider col in cols)
			{
				Rigidbody rb = col.GetComponent<Rigidbody>();
				if(rb == null)
				{
					rb = col.gameObject.GetComponentInParent<Rigidbody>();
				}

				if(rb != null && !col.isTrigger)
				{
					float relativeDist = Mathf.Max(0.1f, Vector3.Distance(transform.position, rb.position) / effectRadius);
					if(abs((int) mode) % 2 == 1)
					{
						relativeDist = 0.5f;
					}
					rb.AddForce(Vector3.Normalize(transform.position - rb.position) * sign((int) mode) * strength / (10 * relativeDist * relativeDist));
				}
			}
		}

		private int abs(int i)
		{
			if(i < 0)
			{
				i *= -1;
			}
			return i;
		}

		private int sign(int i)
		{
			if(i == 0) 
			{
				return 0;
			}
			return i / abs(i);
		}
	}
}

