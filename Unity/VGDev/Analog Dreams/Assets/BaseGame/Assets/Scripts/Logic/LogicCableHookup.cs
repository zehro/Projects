using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LogicCable))]

public class LogicCableHookup : MonoBehaviour
{
    #if UNITY_EDITOR

    public Mesh customWireModel;
    public bool useLocalRotation;

    void Update()
    {
        LogicOutput start = GetComponent<LogicCable>().start;
        LogicInput end = GetComponent<LogicCable>().end;
        Transform wireModel = transform.Find("Cable");
        Transform inputPlug = transform.Find("CableInputPlug");
        Transform outputPlug = transform.Find("CableOutputPlug");

        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        if (start != null)
        {
            transform.position = start.transform.position;
            inputPlug.position = start.transform.position;
            inputPlug.rotation = start.transform.rotation;
        }

        else
        {
            transform.position = Vector3.zero;
            inputPlug.localPosition = Vector3.zero;
            inputPlug.rotation = Quaternion.Euler(270, 0, 0);
        }

        if (end != null)
        {
            outputPlug.position = end.transform.position;
            outputPlug.rotation = end.transform.rotation;
        }

        else
        {
            outputPlug.localPosition = Vector3.zero;
            outputPlug.rotation = Quaternion.Euler(270, 180, 0);
        }

        if (customWireModel == null && start != null && end != null)
        {
            wireModel.localPosition = Vector3.zero;
            wireModel.rotation = Quaternion.LookRotation(end.transform.position - start.transform.position, Vector3.up);
            wireModel.localScale = new Vector3(1, 1, (end.transform.position - start.transform.position).magnitude);
        }

        else
        {
            wireModel.localPosition = Vector3.zero;
            if (!useLocalRotation)
                wireModel.rotation = Quaternion.Euler(270, 0, 0);
            else
                wireModel.rotation = transform.parent.rotation;
            wireModel.localScale = Vector3.one;
        }

        if (customWireModel != null)
        {
            wireModel.GetComponent<MeshFilter>().mesh = customWireModel;
        }
    }

    #endif
}