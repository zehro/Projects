using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 * Used as a general purpose class for hurting enemies; If you don't want any specific behaviour
 */
public class EnemyDefeatGeneralPurpose : MonoBehaviour {
    public bool knockback = true;           //Specify if you want knockback on enemy
    public bool verticalKnockback = false;
    public bool isVunerable = true;

    public GameObject impactEffect;         //Sparks or stars to show damage occured
    public UnityEvent hurtEvent;            //Any functions you want called in when enemy is hurt

    Animator animator;
    Rigidbody2D rig;
    CameraFuncs cam;
    GameManager manager;
    public GameObject memOrbs;
    public bool produceOrbs = false;
    // Use this for initialization
    void Start () {
        rig = gameObject.GetComponent<Rigidbody2D>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void OnTriggerEnter2D(Collider2D other) {
        float dir = Mathf.Sign(other.transform.root.localScale.x);
        Vector2 impactPoint = other.bounds.ClosestPoint(transform.position);

        SwordBehaviour sword = other.gameObject.GetComponent<SwordBehaviour>();
        
        if (animator) {
            isVunerable = !animator.GetBool("Hurt");
        }

        if (other.gameObject.tag == "sword" && sword.activated && isVunerable) {
            int vert = 0;
            if (verticalKnockback) { vert = 10;}
            if (produceOrbs && Random.Range(1,3) == 1) Instantiate(memOrbs, transform.position, transform.rotation);
            if (impactEffect) Instantiate(impactEffect, impactPoint, Quaternion.identity);
            if (knockback && rig != null) rig.velocity = new Vector2(-dir * 20, rig.velocity.y + vert);
            if (verticalKnockback) rig.AddTorque(90 * Mathf.Sign(rig.velocity.x));
            //Vector2 impactDir = new Vector2(transform.position.x, transform.position.y) - impactPoint;
            //if (knockback && rig != null) rig.velocity = impactDir.normalized * 20;


            if (animator) animator.SetBool("Hurt", true);

            if (animator && !animator.GetBool("Death")) {
                if (manager) {
                    //manager.addScore(10);
                    //manager.addMult();
                }
            }
            if (cam && !cam.getShaking()) StartCoroutine(cam.shakeScreen());
            if (hurtEvent != null) hurtEvent.Invoke();
        }
    }

    public void playAudioClip(AudioClip clip) {
        GameObject a = new GameObject(clip.name);
        a.AddComponent<AudioSource>();
        AudioSource source = a.GetComponent<AudioSource>();
        source.loop = false;
        source.priority = 20;
        source.clip = clip;
        a.AddComponent<DestroyOnTime>();
        a.GetComponent<DestroyOnTime>().waitTime = clip.length + 0.01f;
        source.Play();

    }

    void OnDestroy() {
        try {
            if (GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>() != null) {
                Spawner spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
                spawner.removeCurrent(gameObject);
            }
        } catch {}
    }
}
