using UnityEngine;
using System.Collections;

public class HubDoor : MonoBehaviour
{
    AudioSource sound;
    Transform left;
    Transform right;

    bool opened;
    float openedTime;

    void Awake()
    {
        sound = GetComponent<AudioSource>();
        left = transform.Find("Left");
        right = transform.Find("Right");
    }

    void Update()
    {
        if (opened)
        {
            float t = Mathf.Clamp01((Time.time - (openedTime + 1)) / 3f);
            left.localRotation = Quaternion.Euler(0, 0, t * t * -90);
            right.localRotation = Quaternion.Euler(0, 0, t * t * 90);
        }
    }

    public void open()
    {
        if (!opened)
        {
            opened = true;
            openedTime = Time.time;
            sound.Play();
        }
    }

    public float getOpen()
    {
        if (!opened)
            return 0;
        return Mathf.Clamp01((Time.time - (openedTime + 1)) / 3f);
    }
}