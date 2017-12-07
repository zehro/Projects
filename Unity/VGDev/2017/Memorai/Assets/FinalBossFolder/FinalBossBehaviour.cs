using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FinalBossBehaviour : MonoBehaviour {
    CameraFuncs cam;
    Animator animator;
    public EvilCloudFunctions clouds;
    public int health = 100;
    public int attackProbability = 1000;
    public float restTime = 10.0f;

    public GameObject lightning;
    public GameObject[] possibleSpawn;

    public UnityEvent deathEvent;

	void Start () {
        animator = gameObject.GetComponent<Animator>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
	}


    
	void Update () {
        openingCameraShake();    
        if (health < 20) {
            attackProbability = 10;
        }
        if (!animator.GetBool("Vunerable") && !animator.GetBool("Awakening") && !animator.GetBool("Death") && Random.Range(0, attackProbability) == 0) {
            if (health > 30) {
                spawnEnemies();
                clouds.startSparks();
            } else {
                spawnEnemies();
                clouds.startSimulSparks();
            }
            StartCoroutine(cooldown());
        }
	}


    /*
     * Used for the opening camera shake as the boss appears.
     */
    bool awakening = true;
    float timeCounter = 0;
    void openingCameraShake() {
        if (!animator.GetBool("Awakening") && awakening) {
            cam.endShake();
            timeCounter = 0;
            awakening = false;
        } else if (animator.GetBool("Awakening")) {
            if (timeCounter >= 0.01f) {
                cam.shakeOnce();
                timeCounter = 0;
            }
            timeCounter += Time.deltaTime;
        }
    }

    public void hurt() {
        animator.SetBool("Vunerable", false);
        StopAllCoroutines();
        health -= 10;
        if (health <= 0) {
            death();
        }
    }

    void death() {
        MageBehaviour[] cur = FindObjectsOfType<MageBehaviour>();
        foreach (MageBehaviour mage in cur) {
            mage.death();
        }
        animator.SetBool("Death", true);
        cam.shakeOnce();
        deathEvent.Invoke();
    }

    public void triggerSlowMo() {
        if (Time.timeScale < 1) {
            Time.timeScale = 1;
        } else {
            Time.timeScale = 0.5f;
        }
    }
    public void returnDeathScale() {
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    void spawnEnemies() {
        MageBehaviour[] mages = FindObjectsOfType<MageBehaviour>();
        if (mages.Length < 3) {
            if (Random.Range(0, 2) == 1) {
                Instantiate(possibleSpawn[Random.Range(0, possibleSpawn.Length - 1)], transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator cooldown() {
        if (animator.GetBool("Vunerable") == false) {
            yield return new WaitForSeconds(5);
            animator.SetBool("Vunerable", true);
            yield return new WaitForSeconds(restTime);
            if (animator.GetBool("Vunerable")) {
                animator.SetBool("Vunerable", false);
            }
        }
    }



}
