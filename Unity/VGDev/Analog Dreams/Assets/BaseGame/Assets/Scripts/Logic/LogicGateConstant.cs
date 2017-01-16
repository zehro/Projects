using UnityEngine;
using System.Collections;

public class LogicGateConstant : MonoBehaviour
{
    public LogicColor color;
    LogicOutput output;
    MeshRenderer lightSign;

    void Awake()
    {
        output = transform.Find("Output").GetComponent<LogicOutput>();
        lightSign = transform.Find("Light").GetComponent<MeshRenderer>();
    }

    void Start()
    {
        updateEmission();
    }

    void Update()
    {
        output.setOutput(LogicColors.colorToVector(color));
    }

    public void updateEmission()
    {
        Vector3 data = LogicColors.colorToVector(color);
        Color c = new Color(data.x, data.y, data.z);
        lightSign.material.SetColor("_EmissionColor", c);
        DynamicGI.SetEmissive(lightSign, c);
    }
}