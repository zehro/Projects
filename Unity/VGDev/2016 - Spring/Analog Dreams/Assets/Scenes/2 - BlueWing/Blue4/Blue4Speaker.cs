using UnityEngine;
using System.Collections;

public class Blue4Speaker : MonoBehaviour
{
    LogicInput input;
    AudioSource sound;

    float on = 0;
    float dataPrev = 0;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        sound = transform.Find("Sound").GetComponent<AudioSource>();
    }

    void Update()
    {
        float data = input.getInput().magnitude;

        if (dataPrev == 0 && data > 0)
        {
            on = 1;
            sound.Stop();
            sound.volume = 1;
            sound.pitch = 1;
            sound.Play();
        }

        if (data == 0)
        {
            on = Mathf.Max(on - 0.003f, 0);
            sound.volume = on * on;
            sound.pitch = on * on;
            if (on == 0 && sound.isPlaying)
                sound.Stop();
        }

        dataPrev = data;
    }
}