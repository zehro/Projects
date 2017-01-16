using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
	float scale = 0;
	float scaleTarg = 0;
	float scaleDrag = 16;

	void Update()
	{
		scale += (scaleTarg-scale)/scaleDrag;
		transform.localScale = new Vector3(scale,scale,scale);
	}

	public void expand()
	{
		scaleTarg = 1;
	}

	public void pointAt(Vector3 targ, Vector3 up)
	{
		transform.LookAt(targ, up);
	}
}