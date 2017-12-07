using UnityEngine;
using System.Collections;

public class LogicSwitch3Panel : MonoBehaviour
{
    LogicOutput s1;
    LogicOutput s2;
    LogicOutput s3;
    LogicOutput output;

    void Awake()
    {
        s1 = transform.Find("LogicSwitch Left/Output").GetComponent<LogicOutput>();
        s2 = transform.Find("LogicSwitch Middle/Output").GetComponent<LogicOutput>();
        s3 = transform.Find("LogicSwitch Right/Output").GetComponent<LogicOutput>();
        output = transform.Find("Output").GetComponent<LogicOutput>();
    }

    void Update()
    {
        output.setOutput(Vector3.Max(Vector3.Max(s1.getOutput(), s2.getOutput()), s3.getOutput()));
    }
}