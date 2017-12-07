using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostGuyBehaviour : MonoBehaviour {
	Rigidbody2D rig;
	GameObject player;
	public int health = 20;
	public float relativePos;
	private bool lookingLeft = false;
	[Range(0, 1000)]
	public int attackProb = 100;

	[Range(-1,3)]
	public int state = -1;
	/**
     * States:
     * -1 = awakening
     *  0 = Idle
     *  1 = Attack
     *  2 = Attacking
     *  3 = Dead
     */

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		rig = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () {
		float distX = player.transform.position.x - transform.position.x;
		if (health <= 0) {
			state = 3;
			Death ();
		}

		if (state == 0) {
			if (lookingLeft && distX > 0) {
				lookingLeft = false;
				transform.localScale = new Vector3 (Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), 1);
			} else if (!lookingLeft && distX < 0) {
				lookingLeft = true;
				transform.localScale = new Vector3 (-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), 1);
			}		}
		if (state == -1) {
			state = 0;
		} else if (state == 1) {
			Attack ();
		}

		if (Random.Range(attackProb, 0) == 1) {
			state = 1;

		}
//		if (Mathf.Abs (distX) > 2) {
//			rig.velocity = new Vector2 (Mathf.Sign (distX), 0);
//		} else {
//			rig.velocity = new Vector2 (0, rig.velocity.x / 10);
//		}


			
	}
	float timeout = 0;
	void Attack () {
		float distX = player.transform.position.x - transform.position.x;
		if (Mathf.Abs(distX) > 2) {
			rig.velocity = new Vector2(Mathf.Sign(distX) * 5, 0);
		} else {
			rig.velocity = new Vector2(0, rig.velocity.x);
			state = 2;
			StartCoroutine(attack());
			return;
		}
		if (timeout > 12 && distX > 5) {
			state = 0;
			timeout = 0;
		}
		timeout += Time.deltaTime;
	}
	IEnumerator attack() {

		yield return new WaitForSeconds(3);
		state = 0;
	}

	public void hurt() {
		health -= 10;
		state = 0;
	}

	public void Death() {
		player.GetComponent<SamuraiController> ().Invert = false;
        Animator animator = GetComponent<Animator>();
        Rigidbody2D rig = GetComponent<Rigidbody2D>();
        rig.constraints = RigidbodyConstraints2D.None;
        animator.SetBool("Death", true);
	}

    public void die() {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            Animator playerAnim = other.gameObject.GetComponent<Animator>();
            if (playerAnim.GetBool("Hurt")) {
                float distX = player.transform.position.x - transform.position.x;
                float distY = player.transform.position.y - transform.position.y;
                if (Mathf.Abs(distX) < 5 && Mathf.Abs(distY) < 5) {
                    player.GetComponent<SamuraiController>().Invert = true;
                }
            }
        }
    }

}
