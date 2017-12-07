using UnityEngine;
using System.Collections;

public class LogicSwitch : MonoBehaviour, PlayerInteractable
{
    public LogicColor color;
    public float timerLength = 5;
    public AudioClip[] timerSounds;
    public bool startOn;
    public bool stayOn;
    Vector3 data;

    bool onOff = false;
    float position = 0;
    
    float inputTime = 0;
    float flipTime = 0.5f;
    float resetTime = 0.6f;

    Transform handle;
    Material handleMat;
    LogicOutput output;
    AudioSource[] sound;
    AudioSource[] timerSound;
    Transform timerNeedle;
    Material timerFace;
    float timerPosition;

    void Awake()
    {
        handle = transform.Find("Handle");
        handleMat = handle.GetComponent<MeshRenderer>().material;
        output = transform.Find("Output").GetComponent<LogicOutput>();
        sound = handle.GetComponents<AudioSource>();
        timerSound = transform.Find("Timer/TimerAudio").GetComponents<AudioSource>();
        timerNeedle = transform.Find("Timer/TimerNeedle");
        timerFace = transform.Find("Timer/TimerFace").GetComponent<MeshRenderer>().material;
        if (!timerNeedle.gameObject.activeInHierarchy)
            timerNeedle = null;

        // If this switch is supposed to start on, make it flip immediately

        if (startOn)
        {
            flipTime = 0.01f;
            interact(0);
        }
    }

    void Start()
    {
        data = LogicColors.colorToVector(color);
        handleMat.color = new Color(data.x, data.y, data.z);
    }

    void Update()
    {
        // Move handle

        float step = Time.smoothDeltaTime * Time.timeScale;
        position = Mathf.Clamp01(position + step / flipTime * (onOff ? 1 : -1));
        float t = (onOff ? Mathf.Pow(position, 3) : 1 - Mathf.Pow(1 - position, 3));
        handle.localRotation = Quaternion.Euler(t * -180, 0, 0);

        // Fix the flip time if this switch started immediately

        if (startOn && position == 1)
            flipTime = 0.5f;

        // Run timer if it exists

        if (timerNeedle != null)
        {
            Vector3 v;

            if (onOff)
            {
                float r = (timerLength - (Time.time - inputTime - flipTime)) / timerLength;
                timerPosition = Mathf.Min(r, t);
                timerNeedle.localRotation = Quaternion.Euler(0, timerPosition * 180, 0);
                v = Vector3.one * timerPosition;

                if (r <= 0.2 && timerSound[0].clip != timerSounds[1])
                {
                    float time = timerSound[0].time;
                    timerSound[0].clip = timerSounds[1];
                    timerSound[0].Play();
                    timerSound[0].time = time % timerSounds[1].length;
                }
                if (r <= 0)
                {
                    interact(0);
                    timerSound[0].Stop();
                    timerSound[1].Play();
                }
            }

            else
            {
                timerNeedle.localRotation = Quaternion.Euler(0, timerPosition * t * 180, 0);
                v = Vector3.one * timerPosition * t;
            }
            
            v = Vector3.Scale(v, LogicColors.colorToVector(color));
            timerFace.SetVector("_Data", v);
        }

        output.setOutput(data * Mathf.Floor(position));
    }

    public void interact(int data)
    {
        onOff = !onOff;
        inputTime = Time.time;
        sound[(onOff ? 0 : 1)].Play();

        if (timerNeedle != null)
        {
            if (onOff)
            {
                timerSound[0].clip = timerSounds[0];
                timerSound[0].PlayDelayed(flipTime);
                timerSound[0].time = 0;
            }
            else
            {
                timerSound[0].Stop();
            }
        }
    }

    public bool isInteractable()
    {
        if (stayOn && onOff)
            return false;
        return (Time.time > inputTime + resetTime);
    }

    public bool isOn()
    {
        return onOff;
    }
}