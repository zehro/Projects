using UnityEngine;
using System.Collections;

public class Respawn: MonoBehaviour {
    public float limit = 15;
    Vector3 pos;

    void Start () {
        pos = transform.position;
    }

    // Update is called once per frame
    void Update () {
        //checks if player is below a certain height;
        //if so, resets their position, velocity, and rotation.
        if(transform.position.y < -limit) {
            transform.position = pos;
            GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
            transform.rotation = new Quaternion(0,0,0,0);
        }
    }
}
