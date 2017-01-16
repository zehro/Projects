using UnityEngine;
using System.Collections;

public class LogicGateThrough : MonoBehaviour
{
    public bool invert;
    LogicInput input;
    LogicOutput output;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        output = transform.Find("Output").GetComponent<LogicOutput>();
    }

    void Update()
    {
        Vector3 data = input.getInput();
        output.setOutput(invert ? (Vector3.one - data) : data);
    }
}