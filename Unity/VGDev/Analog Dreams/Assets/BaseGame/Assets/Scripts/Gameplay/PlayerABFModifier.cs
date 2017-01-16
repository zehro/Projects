using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class PlayerABFModifier : MonoBehaviour
{
    public float antiBumpForceOverride;
    Player player;
    BoxCollider box;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        box = GetComponent<BoxCollider>();
    }

    void LateUpdate()
    {
        Vector3 move = transform.InverseTransformDirection(player.playerRot * player.playerMove);
        if (box.bounds.Contains(player.transform.position) && move.x < 0)
            player.setAntiBumpForce(antiBumpForceOverride);
    }
}