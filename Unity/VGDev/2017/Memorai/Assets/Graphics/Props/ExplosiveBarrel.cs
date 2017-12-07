using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour {
    CameraFuncs cam;
    Animator animator;
    public int health = 20;

    public GameObject explosion;
    public GameObject fire;

	// Use this for initialization
	void Start () {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (animator != null && animator.GetBool("Hurt")) animator.SetBool("Hurt",false);
	}

    void death() {
        animator.SetBool("Death", true);
    }

    public void hurt() {
        if (health > 0) {
            health -= 10;
            if (health <= 0) {
                death();
            }
        }
        
    }


    public void explode() {
        cam.shake(0.5f, 0.04f);
        Instantiate(explosion, transform.position, Quaternion.identity);
        for (int i = 0; i < 3; i++) {
            GameObject fireObj = Instantiate(fire, transform.position, Quaternion.identity);
            Rigidbody2D fireRig = fireObj.GetComponent<Rigidbody2D>();
            fireRig.velocity = new Vector2(Random.Range(-10, 10), Random.Range(40, 60));
        }
        Destroy(gameObject);
    }
}
