using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelyManBehaviour : MonoBehaviour {
    Animator animator;
    Rigidbody2D rig;
    GameObject player;

    public int health = 30;
    public GameObject skeletonParts;
    public Transform headPosition;

    [Range(0, 1000)]
    public int attackProb = 400;

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
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>());
    }
	
	void Update () {
        //Gets out of the awakening state
        if (animator.GetBool("Awakening")) {
            state = -1;
        } else if (state == -1 && !animator.GetBool("Awakening")) {
            state = 0;
        }

        if (rig.velocity.x < -0.05) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * 1, transform.localScale.y, transform.localScale.z);
        }
        if (rig.velocity.x > 0.05) {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }

        //Set animator stuff
        animator.SetBool("Falling", Mathf.Abs(rig.velocity.y) > 0.05);
        animator.SetBool("Running", Mathf.Abs(rig.velocity.x) > 0.05);

        if (state == 0) {
            idle();
        } else if (state == 1) {
            idling = false;
            attackMode();
        }

        if (Random.Range(attackProb, 0) == 1) {
            state = 1;
        }

	}

    //When he's not in attack mode
    bool idling = true;
    void idle() {
        //float distX = transform.position.x - player.transform.position.x;
        //if (idling == false) {
        //    int[] sign = { 1, -1 };
        //    rig.velocity = new Vector2(Random.Range(20, 50) * sign[Random.Range(0,1)], rig.velocity.y);
        //    idling = true;
        //}
    }

    float timeout = 0;
    void attackMode() {
        float distX = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(distX) > 2) {
            rig.velocity = new Vector2(Mathf.Sign(distX) * 20, rig.velocity.y);
        } else {
            rig.velocity = new Vector2(0, rig.velocity.x);
            state = 2;
            StartCoroutine(attack());
            return;
        }
        if (timeout > 12 && distX > 5) {
            state = 0;
            idling = false;
            timeout = 0;
        }
        timeout += Time.deltaTime;
    }

    IEnumerator attack() {
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        state = 0;
    }
    void death() {
        try {
            Spawner spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
            CameraFuncs camfuncs = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
            if (spawner != null && camfuncs != null && spawner.getCurArray().Count == 1 && (object)spawner.getCurArray()[spawner.getCurArray().Count - 1] == gameObject) {
                camfuncs.startSlowMo();
            }
        } catch { }

        state = 3;
        animator.SetBool("Death", true);
        GameObject parts = Instantiate(skeletonParts, transform.position, Quaternion.identity);
        parts.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
        StartCoroutine(waitDestroy());
    }

    IEnumerator waitDestroy() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }

    public void hurt() {
        health -= 10;
        state = 0;
        if (health <= 0) {
            death();
        }
    }

    void OnDestroy() {
        if (GameObject.FindGameObjectWithTag("spawner")) {
            GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>().removeCurrent(gameObject);
        }
    }

    public void die() {
        Destroy(gameObject);
    }
}
