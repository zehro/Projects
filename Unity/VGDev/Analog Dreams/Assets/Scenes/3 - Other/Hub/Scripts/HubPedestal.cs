using UnityEngine;
using System.Collections;

public class HubPedestal : MonoBehaviour
{
    LogicInput input1;
    LogicInput input2;
    LogicInput input3;
    Transform pedestal;
    AudioSource sound;

    bool preWarm;
    float preWarmTime = 1;
    Vector3 initialPosition;
    float initialData;
    bool rising;
    float risingTime;
    float risingLength = 6;

    void Awake()
    {
        input1 = transform.Find("Input1").GetComponent<LogicInput>();
        input2 = transform.Find("Input2").GetComponent<LogicInput>();
        input3 = transform.Find("Input3").GetComponent<LogicInput>();
        pedestal = transform.Find("PedestalTop");
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!preWarm && Time.timeSinceLevelLoad > preWarmTime)
        {
            preWarm = true;
            initialPosition = pedestal.localPosition;
            initialData = getData();
        }

        if (preWarm && !rising && initialData != getData())
        {
            rising = true;
            risingTime = Time.time;
            sound.Play();
        }

        if (preWarm && rising)
        {
            float t = Mathf.Clamp01((Time.time - risingTime) / risingLength);
            t = Mathf.SmoothStep(0, 1, t);
            pedestal.localPosition = initialPosition + new Vector3(0, 0, t * 2);
        }
    }

    float getData()
    {
        return input1.getInput().sqrMagnitude
             + input2.getInput().sqrMagnitude
             + input3.getInput().sqrMagnitude;
    }
}