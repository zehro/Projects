using UnityEngine;
using System.Collections;

public class LogicInput : MonoBehaviour
{
    Vector3 input;

    public void setInput(Vector3 newIn)
    {
        input = newIn;
    }

    public Vector3 getInput()
    {
        return input;
    }
}