using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerGrabbable))]
public class PortalDestroy : MonoBehaviour
{
    Vector3 initialPos;
    Quaternion initialRot;
    PortalDoor[] portals;
    GameController game;

    Rigidbody body;
    VariableGravity gravity;

    float danger = 0;
    float minDangerDist = 3;
    float maxDangerDist = 6;
    float reset = 0;
    float resetLength = 10;

    void Awake()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        portals = FindObjectsOfType<PortalDoor>();
        game = FindObjectOfType<GameController>();
        body = GetComponent<Rigidbody>();
        gravity = GetComponent<VariableGravity>();

        if (portals.Length == 0)
            enabled = false;
    }

    void Update()
    {
        // Find nearest portal

        float minDist = Mathf.Infinity;
        PortalDoor minPortal = null;

        foreach (PortalDoor p in portals)
        {
            if (p.isConnected())
            {
                float d = Vector3.SqrMagnitude(transform.position - p.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    minPortal = p;
                }
            }
        }

        // Find danger based on nearest portal

        if (minPortal != null)
        {
            Vector3 localPos = minPortal.transform.InverseTransformPoint(transform.position);
            danger = Mathf.Clamp(1 - (localPos.magnitude - minDangerDist) / (maxDangerDist - minDangerDist), 0, 0.99f);
            danger = Mathf.Pow(danger, 3);
            if ((Mathf.Clamp(localPos.z, 0, 1) == localPos.z) && (Mathf.Clamp(localPos.y, 0, 4) == localPos.y) && (Mathf.Abs(localPos.x) < 1))
                danger = 1;
        }

        else
            danger = 0;

        // If danger is too great, we reset this object

        if (danger >= 1)
        {
            danger = 0;
            reset = resetLength;
            game.playerFX.fxFlare(2000, 0.75f);
            game.player.cameraShake(0);
            game.playDestroyNoise();
        }

        // Reset the object (this holds it in place for a short period of time to ensure it resists
        // the player cursor's spring force)

        if (reset > 0)
        {
            reset--;
            transform.position = initialPos;
            transform.rotation = initialRot;
            body.isKinematic = true;
            gravity.resetGravityToInitial();

            if (reset == 0)
                body.isKinematic = false;
        }
    }

    public bool canInteract()
    {
        return (reset == 0);
    }

    public float getDanger()
    {
        return danger;
    }
}