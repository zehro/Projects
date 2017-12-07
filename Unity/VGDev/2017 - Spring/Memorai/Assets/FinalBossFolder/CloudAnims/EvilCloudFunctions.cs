using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilCloudFunctions : MonoBehaviour {
    public GameObject[] topLayerClouds;
    public GameObject[] bottomLayerClouds;
    public GameObject lightning;
    CameraFuncs cam;
    Animator animator;

    bool sparking = false;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFuncs>();
	}
	
	//REMOVE ON FINAL BUILD! ONLY USED FOR DEBUG!
	void Update () {
        if (Input.GetKeyDown(KeyCode.K) && !sparking) { StartCoroutine(sparkSimul()); }
	}

    public void startSparks() {
        if (!sparking) { StartCoroutine(spark()); }
    }

    public void startSimulSparks() {
        if (!sparking) { StartCoroutine(sparkSimul()); }
    }

    //Causes spark of electricty to come from the sky.
    IEnumerator spark() {
        sparking = true;
        int choices = Random.Range(1, 7);
        int r = 1;
        for (int i = 1; i <= 4; i = i << 1) {
            if ((choices & i) != 0) {
                animator.SetLayerWeight(r, 1);
                animator.SetTrigger("Cloud" + r.ToString() + "Shock");

                yield return new WaitForSeconds(1f);
                for (int y = 0; y < 5; y++) {
                    Instantiate(lightning, new Vector3(bottomLayerClouds[r - 1].transform.position.x - 5 + (3 * y), bottomLayerClouds[r - 1].transform.position.y), Quaternion.identity);
                    yield return new WaitForSeconds(0.1f);
                    StartCoroutine(cam.shakeScreen(0.3f));
                    
                }
                //yield return new WaitForSeconds(0.5f);
                animator.SetLayerWeight(r, 0);
            }
            r++;
        }
        sparking = false;
        //Step 2: Shock em' boy!
    }

    //Sparks everything at once;
    IEnumerator sparkSimul() {
        sparking = true;
        int choices = Random.Range(1, 8);
        int r = 1;
        ArrayList rx = new ArrayList();
        for (int i = 1; i < 4; i++) {
            if ((choices & (1 << i - 1)) != 0) {
                rx.Add(r + 0);
                animator.SetLayerWeight(r, 1);
                animator.SetTrigger("Cloud" + r.ToString() + "Shock");                
            }
            r++;
        }
        yield return new WaitForSeconds(1f);
        for (int y = 0; y < 5; y++) {
            foreach (int cloud in rx) {
                Instantiate(lightning, new Vector3(bottomLayerClouds[cloud - 1].transform.position.x - 5 + (3 * y), bottomLayerClouds[cloud - 1].transform.position.y), Quaternion.identity);
            }          
        }
        StartCoroutine(cam.shakeScreen(1f));
        yield return new WaitForSeconds(0.2f);
        animator.SetLayerWeight(1, 0);
        animator.SetLayerWeight(2, 0);
        animator.SetLayerWeight(3, 0);
        sparking = false;
        //Step 2: Shock em' boy!
    }
}
