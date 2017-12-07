using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tunnel : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.CompareTag("Player"))
        {
            hit.GetComponent<TruckController>().reverse(collision.contacts[0].normal);
            hit.GetComponent<CargoHealth>().loseCargo();
        }
    }
}
