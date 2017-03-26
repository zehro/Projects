using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mine : Attack {
	protected override void Start () {
        transform.GetChild(0).GetComponent<ParticleSystem>().Stop();

        base.Start();

        Renderer renderer = GetComponent<Renderer>();
        Color mineColor;
        if (this.crit)
        {
			Object.Instantiate (this.gameObject, transform.position + transform.forward * 4, transform.rotation);
			Object.Instantiate (this.gameObject, transform.position + transform.right * 3 + transform.forward * 2, transform.rotation);
			Object.Instantiate (this.gameObject, transform.position - transform.right * 3 + transform.forward * 2, transform.rotation);

        }
        else
        {
            mineColor = LevelManager.instance.playerDict[agressor].GetRobeMaterial().color;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //Player won't detonate the mine
		if (other.gameObject.GetComponent<BaseControl>() 
			&& other.gameObject.GetComponent<BaseControl>().playerOwner != agressor)
        {
            base.OnTriggerEnter(other);

            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            Destroy(this.gameObject, .1f);
        }
    }
}
