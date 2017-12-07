using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyEnemyBehaviour : MonoBehaviour {
    Rigidbody2D rig;
    Animator animator;
    public int health = 20;
    [Range(1, 1000)]
    public int attackProb = 300;
    // Use this for initialization
	void Start () {
        rig = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        foreach (Collider2D col in GetComponents<Collider2D>()) {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 dist = Vector3.down * (GetComponent<CircleCollider2D>().bounds.extents.y + 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (GetComponent<CircleCollider2D>().bounds.extents.y + 0.1f));

        animator.SetBool("Grounded", (hit && rig.velocity.y == 0));
        animator.SetBool("Upright", Mathf.Abs(transform.rotation.eulerAngles.z) < 0.05f);
        if (animator.GetBool("Grounded") && transform.rotation.eulerAngles.z != 0) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, 0.3f);
        }

        if (Random.Range(0, attackProb) == 1 && health > 0) {
            if (hit) {
                animator.SetTrigger("jump");
            }
        }
	}

    public void jump() {
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, (GetComponent<CircleCollider2D>().bounds.extents.y + 0.1f));
        if (hit) {
            rig.velocity = new Vector2(Random.Range(25, 70) * -Mathf.Sign(transform.position.x - GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position.x), 30);
        }
    }

    public void death() {
        rig.velocity = new Vector2(0, rig.velocity.y);
        rig.gravityScale = 5f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        rig.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator.SetBool("Death", true);
    }

    public void destroy() {
        Destroy(gameObject);
    }
    public void hurt() {
        health -= 10;
        if (health <= 0) {
            death();
        }
    }
}
