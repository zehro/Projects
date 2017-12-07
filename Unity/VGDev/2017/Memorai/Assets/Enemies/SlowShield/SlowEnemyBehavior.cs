using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEnemyBehavior : MonoBehaviour {
	Rigidbody2D rig;
	GameObject player;
	public int health = 30;
	public float stun = 0;
	private float distX = 0;
	private bool lookingLeft;
	private float hurtTimer = 0f;
    Animator animator;

	[Range(0, 100)]
	public float speed;

	[Range(-1, 3)]
	public int state;
	/**
     * States:
     * -1 = awakening
     *  0 = Idle (doesnt really happen with this character.
     *  1 = Attack
     *  2 = Attacking
     *  3 = Dead
     */

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		rig = GetComponent<Rigidbody2D>();
		Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
		lookingLeft = false;
        animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    bool stunRot = false;
	void Update () {
		distX = player.transform.position.x - transform.position.x;
		float distY = player.transform.position.y - transform.position.y;
        int dir = 0;
        if (distX < 0) dir = 1;

        if (health <= 0) {
			state = 3;
            animator.SetBool("Death", true);
		}
		if (hurtTimer > 0) {
			hurtTimer -= Time.deltaTime;
		}
		if (state == -1) {
			state = 0;
		}

		if (stun > 0) {
			stun -= Time.deltaTime;
		} else if (animator.GetBool("Awakening") == false) {
			state = 1;
		}
		if (!animator.GetBool("Awakening") && state == 1 && stun <= 0 && hurtTimer <= 0) {
            stunRot = false;
			rig.velocity = new Vector2(Mathf.Sign(distX) * speed, rig.velocity.y);
            transform.rotation = Quaternion.Euler(Vector3.Slerp(transform.rotation.eulerAngles, new Vector3(0, 180 * dir, 0), 0.5f));
		}
        if (stun > 0 && !stunRot) {
            int ver = 0;
            stunRot = true;
            if (transform.rotation.eulerAngles.y >= 90) {
                ver = 180;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, ver, 0));
        }
		if (Mathf.Abs (distX) < 1.4f && distY < 2f) {
			stun = 3f;
		}
        animator.SetBool("Moving", Mathf.Abs(rig.velocity.x) > 0.2f);
	}

    public void stunKnockback() {

    }
	public void hurt() {
		if (hurtTimer <= 0) {
            int ver = 0;
            stunRot = true;
            if (transform.rotation.eulerAngles.y >= 90) {
                ver = 180;
            }
            transform.rotation = Quaternion.Euler(new Vector3(0, ver, 0));
            health -= 10;
			stun = -1;
			state = 0;
			hurtTimer = 1f;
		}
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player" && state == 1 && stun <= 0 && hurtTimer <= 0 && !other.gameObject.GetComponent<Animator>().GetBool("dodge")) {
            stun = -1;
            state = 0;
            hurtTimer = 0.5f;
            rig.velocity = new Vector2(-12f, rig.velocity.y);
        }
    }

	public void Death() {
		Destroy (this.gameObject);
	}
}
