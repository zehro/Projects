using UnityEngine;
using System.Collections;

public class CheckpointController : MonoBehaviour
{
	public GameObject squid;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == squid)
		{
			// Find out whether or not the squid should boost (close enough to inner ring)
			// and boost them if they should

			Vector3 squidPos = transform.InverseTransformPoint(squid.transform.position);
			squidPos.z = 0;
			bool boost = (squidPos.magnitude < 1.45);
			if (boost)
				squid.GetComponent<SquidController>().boost();

			// Advance the squid
			
			int index = squid.GetComponent<SquidController>().playerIndex;
			transform.parent.parent.parent.GetComponent<GameController>().AdvanceSquid(index, boost);
		}
	}
}