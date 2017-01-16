using UnityEngine;
using System.Collections;

public class LogicOutput : MonoBehaviour
{
    Vector3 output;

    public void setOutput(Vector3 newOut)
    {
        output = newOut;
    }

    public Vector3 getOutput()
    {
        return output;
    }
}