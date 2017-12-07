using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerGrabbable))]

public class VHSTape : MonoBehaviour
{
    GameController game;
    PlayerGrabbable grabbable;
    Rigidbody body;

    int insertState = 0;
    float insertTime;
    public Vector3 insertPos;
    Quaternion insertRot;
    Vector3 targetPos;
    Quaternion targetRot;
    float insertPullbackTime = 0.5f;
    float insertPushinTime = 0.5f;

    TV insertTV;

    void Awake()
    {
        game = GetComponentInParent<GameController>();
        grabbable = GetComponent<PlayerGrabbable>();
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (insertState == 0)
        {
            if (game.playerCursor.isLookingAt(game.tvs))
            {
                grabbable.holdPosition = new Vector3(0, -0.25f, 0.7f);
                grabbable.holdRotation = new Vector3(90, 0, 0);
            }
            else
            {
                grabbable.holdPosition = new Vector3(0, -0.35f, 0.45f);
                grabbable.holdRotation = new Vector3(20, 0, 0);
            }
        }

        else if (insertState == 1)
        {
            float t = Mathf.Clamp01((Time.time - insertTime) / (insertPullbackTime + insertPushinTime));
            t = t * t * t * (t * (t * 6 - 15) + 10);

            float s = Mathf.Clamp01((Time.time - (insertTime + insertPullbackTime)) / (insertPushinTime));
            s = s * s;

            targetPos = insertTV.transform.TransformPoint(new Vector3(0, (1 - s) * 0.6f, 1.81f));

            transform.position = Vector3.Lerp(insertPos, targetPos, t);
            transform.rotation = Quaternion.Slerp(insertRot, targetRot, t);
        }
    }

    public void insert(TV tv)
    {
        grabbable.overrideInteractability(false);
        body.isKinematic = true;
        body.useGravity = false;
        transform.parent = null;

        insertState = 1;
        insertTime = Time.time;
        insertPos = transform.position;
        insertRot = transform.rotation;
        insertTV = tv;

        targetPos = tv.transform.TransformPoint(new Vector3(0, 0.6f, 1.81f));
        targetRot = tv.transform.rotation * Quaternion.Euler(180, 0, 0);
    }
}