using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Used to control behaviour of standard Winged-Enemy
 */
public class BadGuyBehaviour : MonoBehaviour {
    GameObject player;
    Rigidbody2D rig;
    public int health = 30;
    public Collider2D damageBox;
    Animator animator;

    bool dead;

    /* Enemy States
     * 0 - IDLE
     * 1 - ATTACK
     * 2 - DEAD
     */
    [Range(0, 4)]
    public int state = 0;

    /*
     *Use this to define any objects this enemy needs to function.
     * In particular, you'll want to have the animator to control its state machine
     */
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (Collider2D col in GetComponents<Collider2D>()) {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }
        rig = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageBox.enabled = false;
	}
	
    /**
     * Use this to update the enemy's state information
     */
	void Update () {
        if (state == 1 && !damageBox.enabled) {
            damageBox.enabled = true;
        }
		if (rig.velocity.x < -0.01) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (rig.velocity.x > 0.01) {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        if (health <= 0) {
            if (dead == false) {
                deathBump();
                dead = true;
            }
        }

        if (state != 1 && state != 2 && !animator.GetBool("Awakening") && Random.Range(0, 200) == 1) {
            attackTarget = player.transform.position;
            state = 1;
            animator.SetBool("Attack", true);
        }

        if (state == 1) {
            attackMode();
        }
        
        if (state == 2 && animator.GetBool("Hurt")) {
            animator.SetBool("Hurt", false);
        }
			
	}

    /*
     * Function that is executed when enemy dies.
     */
    void deathBump() {
        try {
            Spawner spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
            CameraFuncs camfuncs = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
            if (spawner != null && camfuncs != null && spawner.getCurArray().Count == 1 && (object)spawner.getCurArray()[spawner.getCurArray().Count - 1] == gameObject) {
                camfuncs.startSlowMo();
            }
        } catch { }
        state = 2;
        rig.gravityScale = 3;
        animator.SetBool("Death", true);
        //GetComponent<Collider2D>().isTrigger = false;
        rig.velocity = new Vector2(rig.velocity.x, 25);
        rig.constraints = RigidbodyConstraints2D.None;
        StartCoroutine(destroyOverTime());
    }

    /*
     * Triggered when an enemy dies. Discards of object after 2 seconds.
     */
    IEnumerator destroyOverTime() {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

    /*
     * When enemy wants to attack enemy, sets animation state information
     */
    Vector2 attackTarget = Vector2.zero;
    Vector3 pastPos;
    float stateTime = 0;
    Vector2 relVel;
    void attackMode() {
        if (relVel.x < -0.01) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (relVel.x > 0.01) {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (!animator.GetBool("Hurt") && !animator.GetBool("Death") && Time.timeScale != 0) {
            //transform.position = Vector2.MoveTowards(transform.position, attackTarget, 0.2f);
            transform.position = Vector2.SmoothDamp(transform.position, attackTarget, ref relVel, 2, 3, 0.1f);
        }
        stateTime += Time.deltaTime;
        if (stateTime > 2) {
            state = 0;
            stateTime = 0;
            animator.SetBool("Attack", false);
        }
        pastPos = transform.position; 
    }

    public void endAttackMode() {
        stateTime = 0;
        animator.SetBool("Attack", false);
        state = 0;
    }

    //Procedure for getting hurt
    public void getHurt() {
        if (!animator.GetBool("Death")) {
            GameManager manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            health -= 10;
            endAttackMode();
        }
    }

    void OnDestroy() {
        if (GameObject.FindGameObjectWithTag("spawner")) {
            GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>().removeCurrent(gameObject);
        }
    }
}
