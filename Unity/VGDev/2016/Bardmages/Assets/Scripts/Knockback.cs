using UnityEngine;
using System.Collections;

public class Knockback : Attack
{
    public float knockSpeed;
    public float knockValue;

    private Rigidbody rigidbodyTune;
    private BaseControl enemy;

    protected override void Start() {
        base.Start();
        rigidbodyTune = GetComponent<Rigidbody>();
        
    }

    public void Update() {
        rigidbodyTune.velocity = transform.forward * knockSpeed;
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);

        if (other.gameObject.GetComponent<BaseControl>()
            && other.gameObject.GetComponent<BaseControl>().player != agressor) {

            //GetComponent<MeshRenderer>().enabled = false;
            //GetComponent<TrailRenderer>().enabled = false;
            //GetComponent<BoxCollider>().enabled = false;

            try {
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            } catch (UnityException e) {
                Destroy(this.gameObject);
            }
            
            

            enemy = other.gameObject.GetComponent<BaseControl>();
            Vector3 forward = transform.forward;
            enemy.Knockback(new Vector2(forward.x, forward.z) * knockValue);

            Destroy(transform.GetChild(1).gameObject);
            transform.GetChild(0).GetComponent<ParticleSystem>().transform.parent = null;

            Destroy(this.gameObject);
        }
    }

	void FixedUpdate () {
		if (crit) {
			
			bool sphereCast = true;
			RaycastHit hitInf = new RaycastHit ();
			if (Physics.Raycast (new Ray (transform.position, transform.forward), out hitInf)) {
				if (hitInf.collider.transform.root.GetComponent<BaseControl> ()) {
					sphereCast = false;
				}
			}
			if (sphereCast) {
				Collider[] cols = Physics.OverlapSphere (transform.position + transform.forward * 10f, 10f);
				foreach (Collider c in cols) {
					if (c.transform.root.GetComponent<BaseControl> () && this.agressor != c.transform.root.GetComponent<BaseControl> ().playerOwner) {
						GetComponent<Rigidbody> ().AddForce (((c.transform.root.position - transform.position) - transform.forward * 2f).normalized * 1000f, ForceMode.Acceleration);
					}
				}
			}
			if (GetComponent<Rigidbody> ().velocity != Vector3.zero) {
				transform.forward = GetComponent<Rigidbody> ().velocity;
			}
		}
	}
}