using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour {

    string str = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(str))
        {
            LevelManager.instance.survived();
        }
    }
}
