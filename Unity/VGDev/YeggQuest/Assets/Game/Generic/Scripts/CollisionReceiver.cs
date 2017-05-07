using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionReceiver : MonoBehaviour
{
    public virtual void OnReceivedCollisionEnter(Collision col)
    {

    }

    public virtual void OnReceivedTriggerEnter(Collider other)
    {

    }

    public virtual void OnReceivedTriggerStay(Collider other)
    {

    }
}