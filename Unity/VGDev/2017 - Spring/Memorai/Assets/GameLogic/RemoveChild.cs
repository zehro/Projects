using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveChild : MonoBehaviour {
    
	void Start () {
        StartCoroutine(remove());
	}

    IEnumerator remove() {
        yield return new WaitForSeconds(0.01f);
        transform.parent = null;
    }
}
