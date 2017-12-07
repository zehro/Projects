using UnityEngine;
using System.Collections;

public class HubDoorInput : MonoBehaviour
{
    LogicInput input;
    public HubDoor targetDoor;

    bool hasTriggered;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
    }

    void Update()
    {
        if (!hasTriggered && input.getInput().sqrMagnitude > 0)
        {
            hasTriggered = true;
            targetDoor.open();
        }
    }
}