using UnityEngine;

public class MineCollision : AsteroidCollision {

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(str))
        {
            other.GetComponent<CargoHealth>().loseCargo();
            other.GetComponent<ShakeObject>().Shake((other.transform.position - transform.position).normalized);
            GetComponent<AudioSource>().pitch += Random.Range(-pitchRange, pitchRange);
            GetComponent<AudioSource>().Play();
            Destroy(transform.parent.gameObject, .1f);
        }
    }
}
