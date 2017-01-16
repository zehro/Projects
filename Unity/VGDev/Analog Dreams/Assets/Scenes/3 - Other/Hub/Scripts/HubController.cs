using UnityEngine;
using System.Collections;

public class HubController : MonoBehaviour
{
    public AudioClip[] stingers;

    GameController game;
    MeshRenderer skylight;
    AudioSource skylightAudio;
    LogicOutput[] powerSourceOutputs;
    Transform pedestal;
    
    Light balconyLight;
    Light[] statueLights;
    Light[] spotlights;
    Light[] finalLights;
    HubDoor finalDoor;
    ParticleSystem finalParticles;
    MeshCollider finalDoorGate;
    Light entranceLight;
    HubDoor entranceDoor;
    LogicGateConstant entranceDoorPower;

    AudioSource music;
    AudioSource musicIntro;
    AudioSource[] upperAudio;
    AudioSource[] lowerAudio;

    Transform fx1;
    Transform fx2;

    GUITexture introFade;

    int active;
    Color activeColor;
    bool hasActivated;
    bool hasPowered;
    float activeZPos = -24;
    float activeTime = -1;
    float activeWarmupTime = 2;
    float activeCooldownTime = 12;
    bool musicPlayed = false;

    Color skylightColor = new Color(0.55f, 0.75f, 1);

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        skylight = transform.Find("Lighting/Skylight").GetComponent<MeshRenderer>();
        skylightAudio = transform.Find("Audio/SkylightAudio").GetComponent<AudioSource>();
        powerSourceOutputs = transform.Find("Logic/PowerSource").GetComponentsInChildren<LogicOutput>();
        pedestal = transform.Find("Logic/Pedestal/PedestalTop");

        balconyLight = transform.Find("Lighting/BalconyLight").GetComponent<Light>();
        statueLights = pedestal.Find("Statue").GetComponentsInChildren<Light>();
        spotlights = transform.Find("Lighting/Spotlights").GetComponentsInChildren<Light>();
        finalLights = pedestal.Find("FinalLights").GetComponentsInChildren<Light>();
        finalDoor = pedestal.Find("FinalDoor").GetComponent<HubDoor>();
        finalParticles = pedestal.Find("Particles").GetComponent<ParticleSystem>();
        finalDoorGate = transform.Find("StaticGeometry/CollisionFinalDoor").GetComponent<MeshCollider>();
        entranceLight = transform.Find("Lighting/EntranceLight").GetComponent<Light>();
        entranceDoor = transform.Find("Logic/EntranceDoor").GetComponent<HubDoor>();
        entranceDoorPower = transform.Find("Logic/EntranceDoorPower").GetComponent<LogicGateConstant>();

        music = transform.Find("Audio/Music").GetComponent<AudioSource>();
        musicIntro = transform.Find("Audio/MusicIntro").GetComponent<AudioSource>();
        upperAudio = transform.Find("Audio/UpperAudio").GetComponentsInChildren<AudioSource>();
        lowerAudio = transform.Find("Audio/LowerAudio").GetComponentsInChildren<AudioSource>();

        fx1 = transform.Find("VFX/Reflection Probe");
        fx2 = transform.Find("StaticGeometry/BasementStars");

        introFade = transform.Find("VFX/IntroFade").GetComponent<GUITexture>();
    }

    void Start()
    {
        // Find out which color we should be activating in the animation
        // If we've beaten a wing but have seen its animation before, we go one more

        active = -1;
        while (active < 2 && GameController.LEVEL_PROGRESS[active + 1]
                          >= GameController.LEVEL_COUNT[active + 1])
            active++;
        if (active > -1 && GameController.GAME_PROGRESS[active] == 2)
            active++;

        Vector3 a = indexToVector(active);
        activeColor = new Color(a.x, a.y, a.z);

        // Turn on the components of the power source accordingly

        for (int i = 0; i < active; i++)
            powerSourceOutputs[i].setOutput(indexToVector(i));

        // We need to force-start the stuff the player has already activated

        LogicCable[] cables = GetComponentsInChildren<LogicCable>();
        foreach (LogicCable c in cables)
        {
            if (active > 0 && c.name.Contains("Red"))
                c.unitsPerSecond = 10000f;
            else if (active > 1 && c.name.Contains("Green"))
                c.unitsPerSecond = 10000f;
            else if (active > 2 && c.name.Contains("Blue"))
                c.unitsPerSecond = 10000f;
        }

        // Set proper starting position of pedestal

        pedestal.localPosition = new Vector3(0, 0, Mathf.Max(0, active) * 2 - 6);

        // If this is the very first time we've ever been to the hub, play the intro music

        if (active == -1)
        {
            musicIntro.ignoreListenerVolume = true;
            musicIntro.Play();
        }
    }

    void Update()
    {
        // Handle the intro fade. The timing is different (to allow the string intro to play)
        // if this is the first time we've ever been to the hub

        float iDelay = 13.8f;

        if (Time.timeSinceLevelLoad < 20)
        {
            float o = (active == -1 ? iDelay : 0);
            float t = Mathf.InverseLerp(0.1f + o * 0.8f, 3f + o, Time.timeSinceLevelLoad);
            t = t * t * t * (t * (t * 6 - 15) + 10);
            introFade.color = new Color(0, 0, 0, (1 - t) * 0.5f);
            AudioListener.volume = t * t;
            game.playerCursor.cursorScale = t;

            if (Time.timeSinceLevelLoad > o)
            {
                entranceDoorPower.color = LogicColor.White;
                entranceDoorPower.updateEmission();
            }
        }

        // Handle the entrance animation

        entranceLight.intensity = entranceDoor.getOpen() * 5;

        // Handle the power activation animation

        if (!hasActivated && active != -1 && active != 3 && GameController.GAME_PROGRESS[active] == 1 && game.player.transform.position.z > activeZPos)
        {
            hasActivated = true;
            activeTime = Time.time;

            GameController.GAME_PROGRESS[active] = 2;
            game.saveGame("Hub");

            skylightAudio.Play();
        }

        if (hasActivated && !hasPowered && Time.time > (activeTime + activeWarmupTime))
        {
            hasPowered = true;
            powerSourceOutputs[active].setOutput(indexToVector(active));
            
            game.player.cameraShake(0.5f);
        }

        // Dynamic sky lighting effect

        Color color = skylightColor * 2;

        if (hasActivated && !hasPowered)
        {
            float t = Mathf.Clamp01((Time.time - activeTime) / activeWarmupTime);
            t = Mathf.Pow(t, 6);
            color = Color.Lerp(skylightColor, activeColor, t * 0.25f);
            color *= (2 + t * 6);
        }

        if (hasActivated && hasPowered)
        {
            float t = Mathf.Clamp01((Time.time - (activeTime + activeWarmupTime)) / activeCooldownTime);
            t = Mathf.Pow(1 - t, 3);
            color = Color.Lerp(skylightColor, activeColor, t);
            color *= (2 + t * 16);

            if (active == 2)
            {
                float o = finalDoor.getOpen();
                if (o > 0)
                {
                    finalDoorGate.enabled = false;
                    if (!finalParticles.isPlaying)
                        finalParticles.Play();
                }

                skylightColor = Color.black;
                balconyLight.intensity = t * 1.5f;
                entranceLight.intensity = t * 5f;
                foreach (Light l in statueLights)
                    l.enabled = false;
                foreach (Light l in spotlights)
                    l.range = 2.5f + t * 2.5f;
                foreach (Light l in finalLights)
                    l.enabled = true;
                finalLights[0].intensity = o * 6;
                finalLights[1].intensity = o;
                finalLights[2].intensity = o * 8;
            }

            float interval;
            interval = ((active == 2) ? 24f : 15);
            if (!musicPlayed && (Time.time - activeTime) > interval)
            {
                musicPlayed = true;
                music.clip = stingers[active];
                music.Play();
            }
        }

        // Lots of special things happen when you turn on the Blue Wing switch, so when you come back afterwards,
        // there's more stuff than usual to take care of / activate

        if (active == 3)
        {
            finalDoorGate.enabled = false;
            if (!finalParticles.isPlaying)
                finalParticles.Play();

            color = Color.black;
            balconyLight.intensity = 0;
            entranceLight.intensity = 0;
            foreach (Light l in statueLights)
                l.enabled = false;
            foreach (Light l in spotlights)
                l.range = 2.5f;
            foreach (Light l in finalLights)
                l.enabled = true;
            finalLights[0].intensity = 6;
            finalLights[1].intensity = 1;
            finalLights[2].intensity = 8;
        }

        skylight.material.SetColor("_EmissionColor", color);
        DynamicGI.SetEmissive(skylight, color);

        // Turn on the different effects depending on where the player is

        bool fx = (game.playerCamera.transform.position.y > 0.5f);
        fx1.gameObject.SetActive(fx);
        fx2.gameObject.SetActive(!fx);

        float v = Mathf.InverseLerp(-5, 1.9f, game.playerCamera.transform.position.y);
        v = Mathf.SmoothStep(0, 1, v);
        foreach (AudioSource a in upperAudio)
            a.volume = v;
        foreach (AudioSource a in lowerAudio)
            a.volume = 1 - v;
    }

    Vector3 indexToVector(int i)
    {
        return new Vector3((i == 0 ? 1 : 0), (i == 1 ? 1 : 0), (i == 2 ? 1 : 0));
    }
}