using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VariableGravity))]
[RequireComponent(typeof(PortalDestroy))]

public class PlayerGrabbable : MonoBehaviour, PlayerInteractable
{
    public Vector3 holdPosition = new Vector3(0, -0.5f, 1.5f);
    public Vector3 holdRotation = new Vector3(-10, 0, 0);
    public bool preserveHoldRotation = false;
    public bool disallowStandGrab = false;

    Quaternion offsetRotation;
    bool isGrabbed = false;
    bool isSolid = true;
    bool holdOverride = true;

    GameController game;
    Rigidbody body;
    VariableGravity gravity;
    PortalDestroy destroy;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        body = GetComponent<Rigidbody>();
        gravity = GetComponent<VariableGravity>();
        destroy = GetComponent<PortalDestroy>();
    }

    void FixedUpdate()
    {
        // Enforce solidity (layer 9 = IgnorePlayer)

        gameObject.layer = (isSolid ? 0 : 9);

        // If this object is being grabbed, slowly rotate it to a nice target rotation

        if (isGrabbed)
        {
            Quaternion targetRot = Quaternion.Euler(holdRotation);
            if (!preserveHoldRotation)
                targetRot *= offsetRotation;
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, 0.2f);
        }

        // If it's not being grabbed anymore and it's still unsolid, this object continually attempts
        // to restore its solidity (only doing so once it's no longer colliding with the player)
        // (1 << 8) is a layermask which only collides with the Player layer

        else if (!isSolid && holdOverride)
        {
            Vector3 halfExtents = GetComponent<Collider>().bounds.extents;
            if (!Physics.CheckBox(transform.position, halfExtents, transform.rotation, (1 << 8)))
                isSolid = true;
        }
    }

    void grab()
    {
        // When an object is grabbed, it becomes unsolid, turns off its gravity,
        // and turns on a ton of drag to make it move more smoothly

        isGrabbed = true;
        isSolid = false;

        gravity.enableGravity(false);
        body.drag = 10;
        body.angularDrag = 100;

        // Set the offset rotation which this object will be held at
        // If this object is set to preserve its hold rotation, the object will always display that rotation,
        // but otherwise you can grab objects in intervals of 90 degrees on all axes (looks nice when grabbing)

        Vector3 rot = transform.localRotation.eulerAngles;
        float x = Mathf.Floor(rot.x / 90 + 0.5f) * 90;
        float y = Mathf.Floor(rot.y / 90 + 0.5f) * 90;
        float z = Mathf.Floor(rot.z / 90 + 0.5f) * 90;
        offsetRotation = Quaternion.Euler(x, y, z);
    }

    void release()
    {
        // When an object is released, it sets its gravity direction back to
        // the players', and sets its drags back to their normal values

        isGrabbed = false;

        gravity.enableGravity(true);
        gravity.setGravityToPlayer();
        body.drag = 0;
        body.angularDrag = 0.05f;
    }

    public void interact(int data)
    {
        if (data == 0)
            grab();
        else
            release();
    }

    public bool isInteractable()
    {
        if (disallowStandGrab)
        {
            Vector3 pos = game.player.transform.InverseTransformPoint(transform.position);
            float bound = transform.localScale.x;
            if (pos.y > -1.6f * bound && pos.y < -0.5f * bound
             && pos.x > -0.8f * bound && pos.x < +0.8f * bound
             && pos.z > -0.8f * bound && pos.z < +0.8f * bound)
                return false;
        }

        return (destroy.canInteract() && holdOverride);
    }

    public void overrideInteractability(bool i)
    {
        holdOverride = i;
    }
}