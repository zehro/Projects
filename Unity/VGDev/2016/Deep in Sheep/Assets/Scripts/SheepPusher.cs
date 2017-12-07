using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class SheepPusher : MonoBehaviour {

	public float appliedForce = 40f; //Amount of force applied to the sheep, scaled by player velocity.
	public float targetDistance = 2.0f; //Distance in front of the character that the sheep will attempt to keep.
	public float holdTime = 2.0f; //Time, in seconds, that the force will be applied to the sheep after it is touched.
	public float dampenRadius = 1.0f; //If the sheep is less than this distance to the point, the force will be reduced.
	public float cancelRadius = 4.0f; //If the sheep is more than this distance to the point, it will be released.

	Rigidbody targetSheep;
	float lastPushTime;


	// Use this for initialization
	void Start () {
		this.targetSheep = null;
		this.lastPushTime = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(targetSheep == null)
			return;
		if(Time.fixedTime > lastPushTime + holdTime)
		{
			targetSheep = null;
			return;
		}
		Vector3 forward = this.transform.forward;
		Vector3 pos = this.transform.position;
		Vector3 targetPoint = pos + (forward * this.transform.localScale.magnitude * targetDistance); //Point a bit in front of the player
		if(Vector3.Distance(targetSheep.position, targetPoint) > this.cancelRadius)
		{
			targetSheep = null;
			return;
		}
		//float dampenFactor = 1.0f;
		//if(this.dampenRadius > 0) //Prevents divide by zero
		//	dampenFactor = Mathf.Clamp01(Vector3.Distance(targetSheep.position, targetPoint) / this.dampenRadius); //Reduces force when sheep is near target point
		Vector3 dir = (targetPoint - targetSheep.position).normalized;
		float mag = this.appliedForce * this.GetComponent<Rigidbody>().velocity.magnitude;// * Time.fixedDeltaTime;
		this.targetSheep.AddForce(dir * mag);
	}

	void OnCollisionEnter (Collision col)
	{
		if(!col.gameObject.GetComponent(typeof(Sheep)))
			return;
		Vector3 forward = this.transform.forward;
		Vector3 pos = this.transform.position;
		Vector3 sheepDirection = col.contacts[0].point - pos; //Vector from the player to the sheep
		if(Vector3.Dot(sheepDirection.normalized, forward) <= 0) //If the sheep didn't hit us from the front
		{
			if(col.rigidbody == this.targetSheep)
				this.targetSheep = null; //Prevents a rare case where the sheep being pushed ends up behind the player, and the force causes the sheep to push the player.
			return;
		}
		this.targetSheep = col.rigidbody;
		this.lastPushTime = Time.fixedTime;
	}
}
