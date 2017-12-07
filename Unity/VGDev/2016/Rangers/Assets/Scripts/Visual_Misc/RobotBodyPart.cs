using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;

public class RobotBodyPart : MonoBehaviour 
{

	public GameObject copy;
	private Rigidbody rb;
	private BoxCollider bc;
	public PlayerID pid;
	private bool respawning;

	/// <summary> The speed that the body part is moving at. </summary>
	private float moveSpeed;

	// Use this for initialization
	void Start () 
	{
		//gets playerID from parent
		if(transform.root.GetComponent<Controller>()) {
			pid = transform.root.GetComponent<Controller>().ID;

			//creates copy and disables and removes children
			copy = (GameObject)GameObject.Instantiate(this.gameObject,transform.position,transform.rotation);
			for (int i = 0; i < copy.transform.childCount; i++) {
				Destroy(copy.transform.GetChild(i).gameObject);
			}
			DestroyImmediate(copy.GetComponent<RobotBodyPart>());
			rb = copy.AddComponent<Rigidbody>();
			bc = copy.AddComponent<BoxCollider>();

			ProfileData profile = transform.root.GetComponent<Controller>().ProfileComponent;
			if(profile != null) {
				if(copy.GetComponent<MeshRenderer>().material.name.Equals("PlayerMat1 (Instance)")) {
					copy.GetComponent<MeshRenderer>().material.color = profile.PrimaryColor;
					copy.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.PrimaryColor.r/3f,profile.PrimaryColor.g/3f, profile.PrimaryColor.b/3f));
				} else {
					copy.GetComponent<MeshRenderer>().material.color = profile.SecondaryColor;
					copy.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.SecondaryColor.r/3f,profile.SecondaryColor.g/3f, profile.SecondaryColor.b/3f));
				}
			}

			copy.gameObject.SetActive(false);
		} else if(transform.root.GetComponent<CosmeticPlayer>()) {
			ProfileData profile = ProfileManager.instance.GetProfile(transform.root.GetComponent<CosmeticPlayer>().id);
			if(profile != null) {
				if(GetComponent<MeshRenderer>().material.name.Equals("PlayerMat1 (Instance)")) {
					GetComponent<MeshRenderer>().material.color = profile.PrimaryColor;
					GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.PrimaryColor.r/3f,profile.PrimaryColor.g/3f, profile.PrimaryColor.b/3f));
				} else {
					GetComponent<MeshRenderer>().material.color = profile.SecondaryColor;
					GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Color(profile.SecondaryColor.r/3f,profile.SecondaryColor.g/3f, profile.SecondaryColor.b/3f));
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(respawning)
		{
			copy.transform.position = Vector3.MoveTowards(copy.transform.position, transform.position, Time.deltaTime*(moveSpeed+Vector3.Distance(copy.transform.position,transform.position)));
			copy.transform.rotation = Quaternion.RotateTowards(copy.transform.rotation, transform.rotation, Time.deltaTime*90f);
			moveSpeed += 0.3f;
			if(Vector3.Distance(copy.transform.position,transform.position) < 0.01f)
			{ 
				respawning = false;
				GetComponent<MeshRenderer>().enabled = true;
				copy.gameObject.SetActive(false);
			}
		}
	}

	public void DestroyBody() 
	{
		respawning = false;
		//move the body parts to where the players body is/was
		//I don't know why I have to check to see if copy is null but for some reason some of them are null and idk why
		if(this != null && copy != null) 
		{
			copy.gameObject.SetActive(true);
			bc.enabled = true;
			rb.isKinematic = false;
			GetComponent<MeshRenderer>().enabled = false;
			copy.transform.position = transform.position;
			copy.transform.rotation = transform.rotation;
		}
	}

	public void RespawnBody()
	{
		if(this != null && copy != null)
		{
			bc.enabled = false;
			rb.isKinematic = true;
			respawning = true;
			moveSpeed = 1;
		}
	}
}
