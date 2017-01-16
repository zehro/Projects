﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goal : MonoBehaviour {
	private static List<Goal> goals = new List<Goal>();
	private static GameObject combineEffect;
	private static int nextID = 0;

	private int id;
	private float combineCooldown = 0;

	public Vector3 hydrogenScale = new Vector3(1f, 1f, 1f);
	public int numElementsCombined = 1;
	public Goal childPrefab;
	
	private Stack<Goal> children = new Stack<Goal>();
	public Stack<Goal> Children {
		get { return children; }
		set { children = value; }
	}
	
	private bool combined = false;
	public bool Combined {
		get { return combined; }
	}

	void Awake() {
		id = nextID;
		nextID++;
		if(!goals.Contains(this)) {
			goals.Add(this);
		}
	}

	// Use this for initialization
	void Start () {
		if(combineEffect == null) {
			combineEffect = Resources.Load<GameObject>("CombineEffect");
		}

		if(numElementsCombined > 1 && children.Count <= 0) {
			AddChildren();

			PhysicsModifyable pM = GetComponent<PhysicsModifyable>();
			while(LevelManager.instance.stateStacks.ContainsKey(pM)) {
				LevelManager.instance.stateStacks.Remove(pM);
			}
			
			Stack initStackState = new Stack();
			State initState = State.GetState(pM);
			initStackState.Push(initState);
			LevelManager.instance.stateStacks.Add (pM, initStackState);
		}

		transform.localScale = hydrogenScale * Mathf.Sqrt(numElementsCombined);
	}

	public void AddChildren() {
		if(children.Count == 0 && numElementsCombined > 1) {
			for(int i = 1; i < numElementsCombined; i++) {
				Goal child = Instantiate<Goal>(childPrefab);
				child.numElementsCombined = i;
				child.hydrogenScale = hydrogenScale;
				child.transform.localScale = hydrogenScale * Mathf.Sqrt(i);
				child.childPrefab = childPrefab;
				child.Combine();

				children.Push(child);

				PhysicsModifyable pM = child.GetComponent<PhysicsModifyable>();
				while(LevelManager.instance.stateStacks.ContainsKey(pM)) {
					LevelManager.instance.stateStacks.Remove(pM);
				}
				Stack initStackState = new Stack();	
				State initState = State.GetState(pM);
				initStackState.Push(initState);
				LevelManager.instance.stateStacks.Add (pM, initStackState);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if(numElementsCombined > 1 && children.Count != 1) Debug.Log(children.Count);
		if(!combined) {
			if(Player.instance.timeScale > 0) {
				combineCooldown = Mathf.Max(0, combineCooldown - Time.deltaTime);
				foreach(Goal g in goals) {
					if(combineCooldown <= 0 && Player.instance.timeScale > 0) {
						if(g != null && g.gameObject != null && g != this && !g.combined && touching(g)) {
							float myCharge = GetComponent<PhysicsModifyable>().charge;
							float otherCharge = g.GetComponent<PhysicsModifyable>().charge;
							if(g.numElementsCombined == numElementsCombined && (myCharge == 0 || otherCharge != myCharge)) {
								Goal child = null;
								Goal parent = null;
								
								if((GetComponent<PhysicsAffected>() == null && g.GetComponent<PhysicsAffected>() != null) || g.id < id) {
									child = this;
									parent = g;
								} else {
									child = g;
									parent = this;
								}
								
								PhysicsAffected parentPA = parent.GetComponent<PhysicsAffected>();
								PhysicsAffected childPA = child.GetComponent<PhysicsAffected>();
								if(parentPA != null && childPA != null) {
									parentPA.Velocity = (parentPA.Velocity + childPA.Velocity) / 2f;
								} else if(parentPA != null) {
									parentPA.Velocity /= 2f;
								}

								PhysicsModifyable parentPM = parent.GetComponent<PhysicsModifyable>();
								PhysicsModifyable childPM = child.GetComponent<PhysicsModifyable>();
								if(parentPM.Entangled == null && childPM.Entangled != null) {
									parentPM.Entangled = childPM.Entangled;
									if (parentPM.entangled != null) {
										parentPM.entangled.Entangled = parentPM;
									}
									childPM.Entangled = null;
								}

								parentPM.Mass = 0;
								childPM.Mass = 0;
								parentPM.Charge = 0;
								childPM.Charge = 0;
								
								parent.children.Push(child);
								parent.numElementsCombined++;
								GameObject.Instantiate(combineEffect, parent.transform.position, Quaternion.identity);
								child.Combine();
							}
						}
					}
				}
			} else {
				combineCooldown = 0;
			}

			transform.localScale = hydrogenScale * Mathf.Sqrt(numElementsCombined);
			transform.GetChild(0).GetComponent<TextMesh>().text = System.Enum.GetNames(typeof(Element))[numElementsCombined-1];
			if(GetComponent<PhysicsAffected>() != null) {
				GetComponent<PhysicsAffected>().Inertia = numElementsCombined / 2f;
			}
			
			if(!AntiMatterExplosion.exists && (int)LevelManager.instance.goalElement <= (numElementsCombined-1)) {
				Player.instance.LoadNextLevel();
			}
		}
	}

	private bool touching(Goal g) {
		float myRadius = GetComponent<SphereCollider>().radius * transform.localScale.x;
		float gRadius = GetComponent<SphereCollider>().radius * g.transform.localScale.x;
		if (g.gameObject.layer == LayerMask.NameToLayer ("Wireframe")) {
			return false;
		}
		return Vector3.Distance(transform.position, g.transform.position) <= myRadius + gRadius + 0.1f;
	}

	public void Combine() {
		if (!combined) {
			PhysicsSFXManager.instance.PlayElementCombineSFX();
			combined = true;
			gameObject.SetActive (false);
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<PhysicsModifyable> ().mass = 0;
			GetComponent<PhysicsModifyable> ().enabled = false;
			GetComponent<Collider> ().enabled = false;
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).gameObject.SetActive (false);
			}
		}
	}

	public void UnCombine() {
		if (combined) {
			combined = false;
			gameObject.SetActive (true);
			GetComponent<MeshRenderer> ().enabled = true;
			GetComponent<PhysicsModifyable> ().enabled = true;
			GetComponent<Collider> ().enabled = true;
			for (int i = 0; i < transform.childCount; i++) {
				transform.GetChild (i).gameObject.SetActive (true);
			}
		}
	}

	public void Split() {
		if(numElementsCombined > 1) {
			Goal child = children.Pop();
			child.combineCooldown = 0.5f;
			combineCooldown = 0.5f;
			numElementsCombined--;
			child.UnCombine();

			if(child.GetComponent<PhysicsAffected>() != null) {
				child.transform.position = transform.position + Random.insideUnitSphere;
				Vector3 midPoint = (transform.position + child.transform.position) / 2f;
				float distance = Vector3.Distance(transform.position, child.transform.position) / 2f;

				child.GetComponent<Rigidbody>().AddExplosionForce(25, midPoint, distance, 0, ForceMode.Impulse);
				if(GetComponent<PhysicsAffected>() != null) {
					GetComponent<Rigidbody>().AddExplosionForce(25, midPoint, distance, 0, ForceMode.Impulse);
				}
			}
		}
	}

	public static void AddGoal(Goal g) {
		goals.Add(g);
	}
}
