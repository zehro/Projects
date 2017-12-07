using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spiritBallProjectile : MonoBehaviour {
    Rigidbody2D rig;
    bool spinning = true;
    public GameObject spikes;
    public int rot = 0;
	// Use this for initialization
	void Start () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foreach (Collider2D col in GetComponents<Collider2D>()) {
            Physics2D.IgnoreCollision(GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(), col);
        }

        rig = GetComponent<Rigidbody2D>();
        rot = Random.Range(0, 2);
        if (rot == 0) {
            StartCoroutine(shootBolts());
        } else {
            StartCoroutine(shootBoltsRotational());
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (spinning) {
            transform.Rotate(0, 0, 10);
        }
    }

    IEnumerator shootBolts() {
        yield return new WaitForSeconds(4f);
        rig.velocity = Vector3.zero;
        spinning = false;
        yield return new WaitForSeconds(1f);
        Transform player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        Vector3 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        float shootingDir = -Mathf.Sign(player.localScale.x);
        for (int i = 0; i < 10; i++) {
            //transform.Rotate(0, 0, 15);
            transform.rotation = Quaternion.AngleAxis(angle + (shootingDir * i*10), Vector3.forward);
            GameObject spike = Instantiate(spikes, transform.position, transform.rotation);
            Physics2D.IgnoreCollision(spike.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>());
            float spikeRot = transform.rotation.eulerAngles.z;
            //spike.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(spikeRot), Mathf.Cos(90 - spikeRot)) * 50;
            spike.GetComponent<Rigidbody2D>().velocity = transform.right * 50;
            yield return new WaitForSeconds(0.1f);
        }
        spinning = true;
        GetComponent<Animator>().SetTrigger("FadeAway");
    }

    IEnumerator shootBoltsRotational() {
        yield return new WaitForSeconds(4f);
        rig.velocity = Vector3.zero;
        spinning = false;
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 10; i++) {
            transform.Rotate(0, 0, 360/10);
            GameObject spike = Instantiate(spikes, transform.position, transform.rotation);
            float spikeRot = transform.rotation.eulerAngles.z;
            //spike.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(spikeRot), Mathf.Cos(90 - spikeRot)) * 50;
            spike.GetComponent<Rigidbody2D>().velocity = transform.right * 50;
            yield return new WaitForSeconds(0.1f);
        }
        spinning = true;
        GetComponent<Animator>().SetTrigger("FadeAway");
    }

    public void destroySelf() {
        Destroy(gameObject);
    }
}
