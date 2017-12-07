using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemOrbBehaviour : MonoBehaviour {
    Animator animator;
    GameManager manager;
	// Use this for initialization
	void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach (Collider2D col in GetComponents<Collider2D>()) {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        animator = gameObject.GetComponent<Animator>();
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 3000);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player") collect();
    }

    public void collect() {
        manager.addScore(10);
        animator.SetTrigger("Captured");
    }
    public void destroyObj() {
        Destroy(gameObject);
    }
}
