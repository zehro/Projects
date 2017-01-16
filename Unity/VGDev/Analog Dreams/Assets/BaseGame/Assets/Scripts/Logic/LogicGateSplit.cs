using UnityEngine;
using System.Collections;

public class LogicGateSplit : MonoBehaviour
{
    LogicInput input;
    LogicOutput output1;
    LogicOutput output2;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        output1 = transform.Find("Output1").GetComponent<LogicOutput>();
        output2 = transform.Find("Output2").GetComponent<LogicOutput>();
    }

    void Update()
    {
        output1.setOutput(input.getInput());
        output2.setOutput(input.getInput());
    }
}