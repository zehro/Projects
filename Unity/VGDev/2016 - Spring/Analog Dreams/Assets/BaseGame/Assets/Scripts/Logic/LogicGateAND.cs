using UnityEngine;
using System.Collections;

public class LogicGateAND : MonoBehaviour
{
    LogicInput input1;
    LogicInput input2;
    LogicOutput output;

    void Awake()
    {
        input1 = transform.Find("Input1").GetComponent<LogicInput>();
        input2 = transform.Find("Input2").GetComponent<LogicInput>();
        output = transform.Find("Output").GetComponent<LogicOutput>();
    }

    void Update()
    {
        output.setOutput(Vector3.Min(input1.getInput(), input2.getInput()));
    }
}