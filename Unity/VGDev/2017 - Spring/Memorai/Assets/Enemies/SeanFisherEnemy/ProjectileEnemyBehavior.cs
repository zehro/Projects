using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Used to control behaviour of standard Winged-Enemy
 */
public class ProjectileEnemyBehavior : MonoBehaviour {
    GameObject player;
    Rigidbody2D rig;
    public int health = 30;
    public Collider2D damageBox;
    Animator animator;
    public GameObject pipe;
    public GameObject projectile;

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
        //damageBox.enabled = false;
	}
	
    /**
     * Use this to update the enemy's state information
     */
	void Update () {

		if (rig.velocity.x < -0.01) {
            //transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (rig.velocity.x > 0.01) {
            //transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        if (health <= 0) {
            if (dead == false) {
                deathBump();
                dead = true;
            }
        }

        if (state != 1 && state != 2 && Random.Range(0, 200) == 1) {
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

        // Orient the dart pipe
        attackTarget = player.transform.position;
        Vector3 direction = (attackTarget - pipe.transform.position).normalized;
        direction = new Vector3(direction.x, direction.y, direction.z);

        Quaternion qua = Quaternion.LookRotation(direction);
        int c = 0;
        if ((transform.position - attackTarget).x < 0)
        {
            // target is to the right 
            c = 180;
        }
        pipe.transform.rotation = new Quaternion(qua.z, qua.y, qua.x, qua.w);

        pipe.transform.eulerAngles = Vector3.Lerp(pipe.transform.eulerAngles, new Vector3(0, 0, pipe.transform.eulerAngles.x + c), 3f);
    }

    /*
     * Function that is executed when enemy dies.
     */
    void deathBump() {
        state = 2;
        rig.gravityScale = 3;
        animator.SetBool("Death", true);
        GetComponent<Collider2D>().isTrigger = false;
        rig.velocity = new Vector2(rig.velocity.x, 25);
        rig.constraints = RigidbodyConstraints2D.None;
        StartCoroutine(destroyOverTime());
    }

    public void die() {
        Destroy(gameObject);
    }

    /*
     * Triggered when an enemy dies. Discards of object after 2 seconds.
     */
    IEnumerator destroyOverTime() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    /*
     * When enemy wants to attack enemy, sets animation state information
     */
    Vector3 attackTarget;
    Vector3 pastPos;
    float stateTime = 0;
    Vector2 relVel;
    private GameObject activeProjectile;
    float projectileCounter = 0;

    void attackMode() {
        if (!activeProjectile && !animator.GetBool("Hurt") && !animator.GetBool("Awakening") && !animator.GetBool("Death"))
        {
            attackTarget = player.transform.position;
            activeProjectile = Instantiate(projectile);
            activeProjectile.transform.position = transform.Find("Root").position + activeProjectile.transform.right * 0;
            damageBox = activeProjectile.GetComponent<Collider2D>();
            damageBox.enabled = true;
            
            Vector3 direction = (attackTarget - activeProjectile.transform.position).normalized;
            direction = new Vector3(direction.x, direction.y, direction.z);

            Quaternion qua = Quaternion.LookRotation(direction);
            int c = 0;
            if ((transform.position - attackTarget).x < 0)
            {
                // target is to the right 
                c = 180;
            }
            activeProjectile.transform.rotation = new Quaternion(qua.z, qua.y, qua.x, qua.w);

            activeProjectile.transform.eulerAngles = new Vector3(0, 0, activeProjectile.transform.eulerAngles.x + c);

            // Add force
            activeProjectile.transform.GetComponent<Rigidbody2D>().AddForce(direction * 700);
        }

        projectileCounter += Time.deltaTime;
        if (activeProjectile)
        {
            if (projectileCounter > 1)
            {
                projectileCounter = 0;
                state = 0;
                Destroy(activeProjectile);
            }
            else if (projectileCounter > .4)
            {
                animator.SetBool("Attack", false);
            }
        }
        
        //pastPos = transform.position; 
    }

    private IEnumerator fireProjectile()
    {
        activeProjectile = Instantiate(projectile);
        activeProjectile.transform.position = pipe.transform.GetChild(0).transform.localPosition;

        float counter = 0;
        while (counter < .001f)
        {
            Debug.Log("Moving");
            counter += Time.deltaTime;

            activeProjectile.transform.position = Vector2.MoveTowards(activeProjectile.transform.position, attackTarget, .2f);

            yield return null;
        }

        animator.SetBool("Attack", false);
        Destroy(activeProjectile);
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
            manager.addScore(10);
            manager.addMult();
        }
        if (health <= 0) {
            animator.SetBool("Death", true);
        }
    }

    void OnDestroy() {
        if (GameObject.FindGameObjectWithTag("spawner")) {
            GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>().removeCurrent(gameObject);
        }
    }
}
