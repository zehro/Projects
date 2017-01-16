using UnityEngine;
using System.Collections;

public class LogicGateOR : MonoBehaviour
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
        Vector3 data = input1.getInput() + input2.getInput();
        data = Vector3.Min(data, Vector3.one);
        output.setOutput(data);
    }
}