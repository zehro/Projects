using UnityEngine;
using System.Collections;

public class PortalDoor : MonoBehaviour
{
    public AudioClip doorOn;
    public AudioClip doorOff;
    public AudioClip doorHum;
    public AudioClip doorChangeColor;

    Vector3 color;
    PortalDoor linkedDoor;
    float flash;
    float hum;

    LogicInput input;
    PortalDoorSurface surface;
    Material lightMaterial;
    Material shineMaterial;
    MeshRenderer lightRenderer;
    Collider lightCollider;
    AudioSource[] sound;

    void Awake()
    {
        input = transform.Find("Input").GetComponent<LogicInput>();
        surface = transform.Find("Archway/Surface").GetComponent<PortalDoorSurface>();
        lightMaterial = transform.Find("Archway/Light").GetComponent<MeshRenderer>().material;
        shineMaterial = transform.Find("Archway/Shine").GetComponent<MeshRenderer>().material;
        lightRenderer = transform.Find("Archway/Light").GetComponent<MeshRenderer>();
        lightCollider = transform.Find("Archway/Light").GetComponent<MeshCollider>();
        sound = transform.Find("Audio").GetComponents<AudioSource>();
    }

    void Update()
    {
        // If a door is ever given a new color, it plays a sound for being turned on/off,
        // and then all the portals in the level need to update their linkings

        Vector3 newColor = input.getInput();
        if (color != newColor)
        {
            sound[0].PlayOneShot(doorChangeColor, 0.75f);
            if (newColor != Vector3.zero && color == Vector3.zero)
                sound[0].PlayOneShot(doorOn);
            if (newColor == Vector3.zero && color != Vector3.zero)
                sound[0].PlayOneShot(doorOff);

            flash = 1;
            color = newColor;

            PortalDoor[] doors = FindObjectsOfType<PortalDoor>();
            foreach (PortalDoor p in doors)
                p.relink(doors);

            if (linkedDoor != null)
            {
                linkedDoor.sound[0].PlayOneShot(doorChangeColor, 0.75f);
                linkedDoor.flash = 1;
            }
        }

        // Turn the surface and collision on and off

        bool on = (linkedDoor != null) && (color != Vector3.zero);
        if (!on)
            surface.turnOffCam();
        surface.gameObject.SetActive(on);
        lightCollider.enabled = !on;

        // Set the color of the light material

        Color c = new Color(color.x, color.y, color.z) * 2;

        float t = Mathf.Pow(flash, 10);
        c = Color.Lerp(c, Color.white * 2, t);
        c.a = t;
        surface.color = c;

        flash = Mathf.Max(0, flash - 0.005f * Time.timeScale);

        lightMaterial.SetColor("_EmissionColor", c);
        shineMaterial.SetColor("_TintColor", c * 0.5f);
        DynamicGI.SetEmissive(lightRenderer, c);

        // Sound

        hum = Mathf.Clamp01(hum + (color != Vector3.zero ? 0.05f : -0.05f));
        sound[1].pitch = hum;
        sound[1].volume = hum * 0.5f;
    }

    public void relink(PortalDoor[] doors)
    {
        linkedDoor = null;

        // If this door isn't receiving any power now, there's no need to hook it up

        if (color == Vector3.zero)
            return;

        // Go through every door. If this door finds another with its same color, it links to it,
        // but if it finds another one of the same color later, it unhooks itself again.

        foreach (PortalDoor p in doors)
        {
            if (p != this && p.color == color)
            {
                if (linkedDoor == null)
                    linkedDoor = p;
                else
                {
                    linkedDoor = null;
                    return;
                }
            }
        }

        // If this door is now linked, give the surface its new data

        if (linkedDoor != null)
        {
            surface.setSurfaceType(GetInstanceID() < linkedDoor.GetInstanceID());
            surface.setLinkedSurface(linkedDoor.surface);
        }
    }

    public bool isConnected()
    {
        return (linkedDoor != null) && (color != Vector3.zero);
    }
}