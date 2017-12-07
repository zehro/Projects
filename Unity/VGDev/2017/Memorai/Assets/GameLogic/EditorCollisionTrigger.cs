using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Attach this to objects that you want to do things when collided with
 * You could do this with code, but its saves workflow time for me by just
 * attaching UnityEvents to objects
 */
public class EditorCollisionTrigger : MonoBehaviour {
    public UnityEvent triggerEvent;
    public bool destroyOnTrigger = false;
    public bool triggerOnce = false;
    bool triggered = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (!triggerOnce || (triggerOnce && !triggered)) {
            triggerEvent.Invoke();
            if (destroyOnTrigger) Destroy(gameObject);
        }
    }
}
