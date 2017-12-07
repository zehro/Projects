using UnityEngine;
using System.Collections;

public class LogicCable : MonoBehaviour
{
    public LogicOutput start;
    public LogicInput end;
    public float unitsPerSecond = 8;
    public bool fade = false;
    public bool ignoreDistance = false;

    Vector3 data;

    Material mat;
    AudioSource sound;

    void Awake()
    {
        mat = transform.Find("Cable").GetComponent<MeshRenderer>().material;
        sound = transform.Find("SparkAudio").GetComponent<AudioSource>();
    }

	void Update()
    {
        if (start != null)
        {
            Vector3 input = start.getOutput();
            Vector3 prev = data;

            float step;

            if (!ignoreDistance)
            {
                step = Time.smoothDeltaTime * Time.timeScale * unitsPerSecond;
                step /= (start.transform.position - end.transform.position).magnitude;
            }

            else
            {
                step = Time.deltaTime / unitsPerSecond;
            }

            data = Vector3.Scale(data + new Vector3(step, step, step), input);
            data = Vector3.Max(Vector3.Min(data, Vector3.one), Vector3.zero);
            mat.SetVector("_Data", data);
            mat.SetFloat("_Fade", (fade ? 1 : 0));
            mat.SetFloat("_Flash", 0);

            if (data.sqrMagnitude > prev.sqrMagnitude)
            {
                sound.transform.position = Vector3.Lerp(start.transform.position, end.transform.position, Mathf.Max(data.x, data.y, data.z));
                sound.volume = 1;
            }

            else
                sound.volume = 0;
        }

        if (end != null)
        {
            Vector3 output = new Vector3();
            output.x = Mathf.Floor(data.x);
            output.y = Mathf.Floor(data.y);
            output.z = Mathf.Floor(data.z);
            end.setInput(output);
        }
    }

    public Vector3 getData()
    {
        return data;
    }
}