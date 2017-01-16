using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	public Texture normalTexture;
	public Texture selectedTexture;

	Material material;
	Vector3 oPosition;
	Quaternion oRotation;
	Vector3 oScale;
	float scale = 1;
	float scaleTarg = 1;
	float scaleDrag = 4;

	void Awake()
	{
		material = GetComponent<MeshRenderer>().material;
		oPosition = transform.position;
		oRotation = transform.rotation;
		oScale = transform.localScale;
	}

	void Update()
	{
		float hOff = oPosition.x * oPosition.y * oPosition.z;
		var rotHover = Quaternion.identity;
		rotHover *= Quaternion.Euler(Mathf.Sin(Time.time*2F+hOff)*2,0,0);
		rotHover *= Quaternion.Euler(0,Mathf.Cos(Time.time*1.5F+hOff)*3,0);
		rotHover *= Quaternion.Euler(0,0,Mathf.Sin(Time.time*2.5F+hOff)*1);
		var posHover = Vector3.zero;
		posHover.x += Mathf.Cos(Time.time*1.5F+hOff)*0.05F;
		posHover.y += Mathf.Sin(Time.time*2.5F+hOff)*0.05F;
		posHover.z += Mathf.Cos(Time.time*2F+hOff)*0.025F;
		scale += (scaleTarg-scale)/scaleDrag;

		transform.position = oPosition + posHover;
		transform.rotation = oRotation * rotHover;
		transform.localScale = oScale * scale;
	}

	public void select()
	{
		material.mainTexture = selectedTexture;
		scaleTarg = 1.2F;
		scale = 1.4F;

	}

	public void deselect()
	{
		material.mainTexture = normalTexture;
		scaleTarg = 1;
	}
}