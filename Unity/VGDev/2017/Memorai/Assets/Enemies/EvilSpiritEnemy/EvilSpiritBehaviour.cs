using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSpiritBehaviour : MonoBehaviour {
    public GameObject IK;
    GameObject player;
    Rigidbody2D rig;
    Animator animator;
    Collider2D trigger;
    public GameObject attackOrb;
    public Transform attackOrbPos;
    public int health = 50;

    [Range(0, 1000)]
    public int attackProb =100;

    //States:
    // 0 - IDLE
    // 1 - ATTACK
    // 2 - DEAD
    int state = 0;

    public GameObject curOrb;
	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (Collider2D col in GetComponents<Collider2D>()) {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }

        animator = GetComponent<Animator>();
        trigger = GetComponent<Collider2D>();
        rig = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < 10) {
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, 5, transform.position.z), 0.01f);
        }
        float distX = player.transform.position.x - transform.position.x;
        if (distX < -0.5 && transform.rotation.eulerAngles.y != 0) {
            //transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.2f);
        } else if (distX > 0.5 && transform.rotation.eulerAngles.y != 180) {
            //transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), 0.2f);
        }
        IK.transform.position = player.transform.position;

        if (Input.GetButtonDown("Fire1")) {
            dodge();
        }

        if (Random.Range(0, attackProb) == 1 && curOrb == null && !animator.GetBool("Awakening")) {
            //Debug.Break();
            animator.SetTrigger("Attack");
        }
	}

    void dodge() {
        if (Vector3.Distance(player.transform.position, transform.position) < 8) {
            if (Random.Range(1, 3) == 1) {
                animator.SetTrigger("idle");
                rig.velocity = new Vector3(Mathf.Sign(player.transform.position.x - transform.position.x) * 40, rig.velocity.y + Random.Range(-10,10));
            }
        }
    }
    public void hurt() {
        health -= 10;
        if (health <= 0) {
            death();
        } else {
            animator.SetBool("Hurt", true);
        }
    }

    public void death() {
        animator.SetBool("Death", true);
    }

    public void destroySelf() {
        Destroy(gameObject);
    }

    public void throwAttack() {
        curOrb = Instantiate(attackOrb, attackOrbPos.position, Quaternion.identity);
        float dir = -Mathf.Cos(transform.rotation.eulerAngles.y);
        curOrb.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(50, 100) * dir, Random.Range(-10, 10));
        animator.ResetTrigger("Attack");
    }
}
