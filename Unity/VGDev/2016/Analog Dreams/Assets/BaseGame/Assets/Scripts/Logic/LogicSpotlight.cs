using UnityEngine;
using System.Collections;

public class LogicSpotlight : MonoBehaviour
{
    public Transform target;
    public float targetDrag = 0.05f;
    public AudioClip turnOn;
    public AudioClip turnOff;

    LogicInput input;
    Vector3 prev;
    Light spotlight;
    public float spotlightStrength = 4;
    float spotlightStrengthDrag = 0.1f;
    MeshRenderer spotlightLens;
    AudioSource[] sound;

    Transform target1;
    Transform target2;
    Transform pivot1;
    Transform pivot2;
    AudioSource motor1;
    AudioSource motor2;
    float motorVolumeResponse = 0.06f;
    float motorPitchResponse = 0.015f;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        spotlight = transform.Find("Pivot1/Pivot2/Spotlight").GetComponent<Light>();
        spotlightLens = transform.Find("Pivot1/Pivot2/SpotlightLens").GetComponent<MeshRenderer>();
        sound = transform.Find("Sound").GetComponents<AudioSource>();
        target1 = transform.Find("Target1");
        target2 = transform.Find("Target1/Target2");
        pivot1 = transform.Find("Pivot1");
        pivot2 = transform.Find("Pivot1/Pivot2");
        motor1 = transform.Find("Pivot1").GetComponent<AudioSource>();
        motor2 = transform.Find("Pivot1/Pivot2").GetComponent<AudioSource>();
    }

    void Update()
    {
        // Logic and color

        Vector3 data = input.getInput();
        spotlight.color = new Color(data.x, data.y, data.z);
        float b = (data != Vector3.zero ? spotlightStrength : 0);
        spotlight.intensity += (b - spotlight.intensity) * spotlightStrengthDrag;
        spotlightLens.material.SetColor("_EmissionColor", spotlight.color * spotlight.intensity);
        DynamicGI.SetEmissive(spotlightLens, spotlight.color * spotlight.intensity);

        // Sound

        if (prev != data)
        {
            if (prev == Vector3.zero)
                sound[0].PlayOneShot(turnOn);
            if (data == Vector3.zero)
                sound[0].PlayOneShot(turnOff);
            prev = data;
        }
        sound[1].pitch = spotlight.intensity / spotlightStrength;
        sound[1].volume = sound[1].pitch * 0.5f;

        // Find out where the spotlight should be pointing
        // Points at target if on, and resting position if off

        if (data != Vector3.zero)
        {
            Vector3 coords = transform.InverseTransformPoint(target.position);
            coords.z = 0;
            coords = transform.TransformPoint(coords);
            Vector3 up = transform.rotation * Vector3.forward;
            target1.LookAt(coords, up);
            target1.localRotation *= Quaternion.Euler(270, 0, 0);
            target2.LookAt(target);
            target2.localRotation *= Quaternion.Euler(180, 0, 0);
        }
        else
        {
            target1.localRotation = Quaternion.identity;
            target2.localRotation = Quaternion.identity;
        }

        // Adjust the current heading and motor sounds based on the above

        pivot1.localRotation = Quaternion.Slerp(pivot1.localRotation, target1.localRotation, targetDrag);
        pivot2.localRotation = Quaternion.Slerp(pivot2.localRotation, target2.localRotation, targetDrag);

        float t = Quaternion.Angle(pivot1.localRotation, target1.localRotation);
        float s = Quaternion.Angle(pivot2.localRotation, target2.localRotation);
        motor1.volume = t * motorVolumeResponse * sound[1].volume;
        motor1.pitch = t * motorPitchResponse;
        motor2.volume = s * motorVolumeResponse * sound[1].volume;
        motor2.pitch = s * 8 * motorPitchResponse;
    }
}