using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class VariableGravity : MonoBehaviour
{
    public Vector3 gravity = Vector3.down;
    Vector3 initialGravity;
    bool useGravity = true;
    Vector3 boxGravity;
    bool useBoxGravity;

    Player player;
    LogicGravityBox[] boxes;
    Rigidbody body;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        boxes = FindObjectsOfType<LogicGravityBox>();
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        initialGravity = gravity;
    }

    void FixedUpdate()
    {
        if (useBoxGravity)
            gravity = boxGravity;
        if (useGravity)
            body.AddForce(gravity * Physics.gravity.magnitude, ForceMode.Acceleration);
    }

    public void enableGravity(bool on)
    {
        useGravity = on;
    }

    public void setGravityToPlayer()
    {
        gravity = player.transform.rotation * Vector3.down;
    }

    public void recalculateBoxGravity()
    {
        boxGravity = Vector3.zero;
        useBoxGravity = false;

        foreach (LogicGravityBox b in boxes)
        {
            if (b.affects(this))
            {
                boxGravity += b.getGravity();
                useBoxGravity = true;
            }
        }

        boxGravity.Normalize();
    }

    public void resetGravityToInitial()
    {
        gravity = initialGravity;
    }
}