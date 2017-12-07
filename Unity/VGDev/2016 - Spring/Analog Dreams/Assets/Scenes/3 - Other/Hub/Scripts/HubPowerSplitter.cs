using UnityEngine;
using System.Collections;

public class HubPowerSplitter : MonoBehaviour
{
    LogicInput input;
    LogicOutput output1;
    LogicOutput output2;
    LogicOutput output3;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        output1 = transform.Find("Output1").GetComponent<LogicOutput>();
        output2 = transform.Find("Output2").GetComponent<LogicOutput>();
        output3 = transform.Find("Output3").GetComponent<LogicOutput>();
    }

    void Update()
    {
        Vector3 data = input.getInput();
        output1.setOutput(data);
        output2.setOutput(data);
        output3.setOutput(data);
    }
}