using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]

public class CollisionNoise : MonoBehaviour
{
    AudioSource source;
    Rigidbody body;
    public AudioClip[] hardImpacts;
    public AudioClip[] medImpacts;
    public AudioClip[] softImpacts;

    int playerTime = 0;
    float hardCutoff = 20f;
    float medCutoff  = 2f;
    float softCutoff = 0.3f;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (playerTime > 0)
            playerTime--;
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Player")
            playerTime = 5;
        if (playerTime == 0)
        {
            float v = col.impulse.magnitude * body.velocity.magnitude;
            source.pitch = Random.Range(0.9f, 1.1f);

            if (v > hardCutoff)
                source.PlayOneShot(hardImpacts[Random.Range(0, hardImpacts.Length)], Mathf.Min(v * 0.25f, 1.0f));
            else if (v > medCutoff)
                source.PlayOneShot(medImpacts[Random.Range(0, medImpacts.Length)], Mathf.Min(v * 0.5f, 1.0f));
            else if (v > softCutoff)
                source.PlayOneShot(softImpacts[Random.Range(0, softImpacts.Length)], Mathf.Min(v * 0.5f, 1.0f));
        }
    }
}