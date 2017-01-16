using UnityEngine;
using System.Collections;

public class TV : MonoBehaviour, PlayerInteractable
{
    public string nextScene;
    public Texture[] textures;
    
    GameController game;
    BoxCollider tapeChecker;
    Material mat;

    bool hasTape;
    float insertTime;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        tapeChecker = transform.Find("TVTapeChecker").GetComponent<BoxCollider>();
        mat = transform.Find("TVScreen").GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (!hasTape)
        {
            // Update the texture

            mat.SetTexture("_EmissionMap", textures[Mathf.FloorToInt(Time.time % 2)]);

            // Test to see if the tape is inside

            Vector3 localPos = tapeChecker.transform.InverseTransformPoint(game.vhsTape.transform.position);
            localPos -= tapeChecker.center;
            localPos.x = Mathf.Abs(localPos.x);
            localPos.y = Mathf.Abs(localPos.y);
            localPos.z = Mathf.Abs(localPos.z);

            if (isInteractable() && (Vector3.Min(localPos, tapeChecker.size * 0.5f) == localPos))
            {
                game.win(this);
                game.vhsTape.insert(this);
                hasTape = true;
                insertTime = Time.time;
            }
        }

        else
        {
            if (Time.time > insertTime + 1)
                mat.SetTexture("_EmissionMap", textures[2]);
        }
    }

    public void interact(int data)
    {
    }

    public bool isInteractable()
    {
        Vector3 pos = transform.InverseTransformPoint(game.playerCamera.transform.position);

        return (game.player.isGrounded()
            && ((Mathf.Abs(pos.x) < 0.6f) && (pos.y < 2f) && (pos.z > 1.9f) && (pos.z < 2.5f))
            && game.playerCursor.isGrabbing(game.vhsTape.GetComponent<PlayerGrabbable>()));
    }
}