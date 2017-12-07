using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSender : MonoBehaviour
{
    public CollisionReceiver receiver;

    void OnCollisionEnter(Collision col)
    {
        receiver.OnReceivedCollisionEnter(col);
    }

    void OnTriggerEnter(Collider other)
    {
        receiver.OnReceivedTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        receiver.OnReceivedTriggerStay(other);
    }
}