using UnityEngine;
using System.Collections;

public class Blue3Controller : MonoBehaviour
{
    GameController game;
    LogicGravityBox[] gravityBoxes;
    Rigidbody[] floatyBoxes;
    LogicCable[] floatyCables;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        gravityBoxes = transform.Find("GravityBoxes").GetComponentsInChildren<LogicGravityBox>();
        floatyBoxes = transform.Find("FloatyBoxes").GetComponentsInChildren<Rigidbody>();
        floatyCables = transform.Find("FloatyCables").GetComponentsInChildren<LogicCable>();
    }

    void Start()
    {
        foreach (Rigidbody r in floatyBoxes)
        {
            r.rotation = Random.rotation;
            r.angularVelocity = Random.insideUnitSphere * 0.4f;
            r.velocity = Random.insideUnitSphere * 0.2f;
        }
    }

    void Update()
    {
        // Run the cable logic

        float s = (Time.timeSinceLevelLoad > 1 ? 8 : 10000);
        foreach (LogicCable c in floatyCables)
            c.unitsPerSecond = s;

        // Depending on where the player is, activate the lights on the gravity boxes (optimization)

        int p = 0;
        if (game.playerCamera.transform.position.x > -30)
            p++;
        if (game.playerCamera.transform.position.x > 30)
            p++;
        
        gravityBoxes[0].setLight(p == 0);
        gravityBoxes[1].setLight(p == 0);
        gravityBoxes[2].setLight(p == 1);
        gravityBoxes[3].setLight(p == 1);
        gravityBoxes[4].setLight(p == 2);
        gravityBoxes[5].setLight(p == 2);
    }
}