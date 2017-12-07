using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInteract : MonoBehaviour {

    GameObject player;
    Rigidbody2D rig;
    public int health = 12;
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
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }
        //rig = gameObject.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageBox.enabled = false;
    }

    /**
     * Use this to update the enemy's state information
     */
    void Update()
    {

        if (rig.velocity.x < -0.01)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        if (rig.velocity.x > 0.01)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        if (health <= 0)
        {
            if (dead == false)
            {
                deathBump();
                dead = true;
            }
        }

        if (state != 1 && state != 2 && Random.Range(0, 200) == 1)
        {
            attackTarget = player.transform.position;
            state = 1;
            animator.SetBool("Attack", true);
        }

        if (state == 1)
        {
            animator.SetBool("GetHit", true);
        }

        if (state == 2 && animator.GetBool("Hurt"))
        {
            animator.SetBool("Hurt", false);
        }
    }

    /*
     * Function that is executed when enemy dies.
     */
    void deathBump()
    {
        state = 2;
        rig.gravityScale = 3;
        animator.SetBool("Death", true);
        GetComponent<Collider2D>().isTrigger = false;
        rig.velocity = new Vector2(rig.velocity.x, 25);
        rig.constraints = RigidbodyConstraints2D.None;
        StartCoroutine(destroyOverTime());
    }

    /*
     * Triggered when an enemy dies. Discards of object after 2 seconds.
     */
    IEnumerator destroyOverTime()
    {
        yield return new WaitForSeconds(2);
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

    void getHit() {

    }

    private IEnumerator fireProjectile()
    {
        yield return null;
    }

    public void endAttackMode()
    {
        stateTime = 0;
        animator.SetBool("Attack", false);
        state = 0;
    }

    //Procedure for getting hurt
    public void getHurt()
    {
        if (!animator.GetBool("Death"))
        {
            GameManager manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
            health -= 10;
            endAttackMode();
            manager.addScore(10);
            manager.addMult();
        }
    }

    void OnDestroy()
    {
        if (GameObject.FindGameObjectWithTag("spawner"))
        {
            GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>().removeCurrent(gameObject);
        }
    }
}
