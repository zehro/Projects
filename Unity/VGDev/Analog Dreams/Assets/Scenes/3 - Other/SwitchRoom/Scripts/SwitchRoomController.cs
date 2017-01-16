using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

public class SwitchRoomController : MonoBehaviour
{
    public Texture[] tvTextures;

    GameController game;
    MeshRenderer powerTube;
    Light powerTubeLight;
    ParticleSystem powerTubeParticles;
    LogicInput powerTubeInput;
    LogicSwitch powerTubeSwitch;
    LogicCable powerTubeSwitchCable;
    LogicCable[] powerTubeCables;
    LogicOutput[] powerTubeCableOutputs;
    AudioSource[] powerTubeSounds;
    AudioSource[] powerTubeWireSounds;

    Transform[] turbines = new Transform[3];
    float[] turbineSpeed = { 0, 0, 0 };
    float[] turbineAccel = { 0.0015f, 0.00075f, 0.000375f };
    float[] turbineMaxSpeed = { 3f, 2.25f, 1.5f };

    Light[] upperDeckLights;
    Light switchLight;
    Light switchAreaLight;

    HubDoor entranceDoor;
    LogicGateConstant entranceDoorPower;
    GameObject sequenceBreakBlock;

    GUITexture introFade;

    int index;
    Color indexColor;
    string indexString;

    bool startedActivated = false;
    bool hasActivated = false;
    bool hasSwitched = false;
    bool hasZapped = false;
    float activeTime = 0;
    float warmupTime = 4;
    float zappedTime = 0;

    float hum = 0;

    void Awake()
    {
        game = FindObjectOfType<GameController>();
        powerTube = transform.Find("Generator/PowerTube").GetComponent<MeshRenderer>();
        powerTubeLight = transform.Find("Generator/PowerTubeLight").GetComponent<Light>();
        powerTubeParticles = transform.Find("Generator/PowerTubeParticles").GetComponent<ParticleSystem>();
        powerTubeInput = transform.Find("Generator/PowerTubeInput/Input").GetComponent<LogicInput>();
        powerTubeSwitch = transform.Find("Generator/PowerTubeSwitch/LogicSwitch").GetComponent<LogicSwitch>();
        powerTubeSwitchCable = transform.Find("Generator/PowerTubeSwitchCable").GetComponent<LogicCable>();
        powerTubeCables = transform.Find("Generator/PowerTubeCables").GetComponentsInChildren<LogicCable>();
        powerTubeCableOutputs = transform.Find("Generator/PowerTubeCableOutputs").GetComponentsInChildren<LogicOutput>();
        powerTubeSounds = transform.Find("Generator/PowerTubeSounds").GetComponentsInChildren<AudioSource>();
        powerTubeWireSounds = transform.Find("Generator/PowerTubeWireSounds").GetComponentsInChildren<AudioSource>();

        turbines[0] = transform.Find("Generator/Turbine1");
        turbines[1] = transform.Find("Generator/Turbine2");
        turbines[2] = transform.Find("Generator/Turbine3");

        upperDeckLights = transform.Find("Lighting/UpperDeck").GetComponentsInChildren<Light>();
        switchLight = transform.Find("Lighting/Switch/SwitchLight").GetComponent<Light>();
        switchAreaLight = transform.Find("Lighting/Switch/SwitchAreaLight").GetComponent<Light>();

        entranceDoor = transform.Find("EntranceHallway/EntranceDoor").GetComponent<HubDoor>();
        entranceDoorPower = transform.Find("EntranceHallway/EntranceDoorPower").GetComponent<LogicGateConstant>();
        sequenceBreakBlock = transform.Find("SequenceBreakBlock").gameObject;

        introFade = transform.Find("IntroFade").GetComponent<GUITexture>();

        // Set index

        index = GameController.SWITCH_INDEX;
        indexColor = LogicColors.indexToColor(index);
        indexString = LogicColors.indexToString(index);

        // If we've turned on this switch before, activate everything immediately

        if (GameController.GAME_PROGRESS[index] > 0)
        {
            startedActivated = true;
            powerTubeSwitchCable.unitsPerSecond = 10000;

            // Visual effects

            Vector3 data = new Vector3(indexColor.r, indexColor.g, indexColor.b);
            powerTube.material.SetVector("_Data", data);

            powerTubeLight.transform.position = Vector3.up * 15;
            powerTubeLight.intensity = 8;
            powerTubeLight.bounceIntensity = 0.5f;
            powerTubeLight.color = Color.Lerp(indexColor, Color.white, 0.125f);

            powerTubeParticles.transform.localScale = new Vector3(1.2f, 1.2f, 1);
            powerTubeParticles.startColor = Color.Lerp(indexColor, Color.white, 0.125f);

            // Cables

            data = new Vector3(indexColor.r, indexColor.g, indexColor.b);
            foreach (LogicOutput o in powerTubeCableOutputs)
                o.setOutput(data);

            // Lighting

            switchLight.gameObject.SetActive(false);
            switchAreaLight.gameObject.SetActive(false);

            // Audio

            powerTubeSounds[1].volume = 0.75f;
            powerTubeSounds[1].pitch = 0.75f;
            powerTubeSounds[2].volume = 0.75f;
            powerTubeSounds[2].pitch = 1.5f;

            // Open the gate

            sequenceBreakBlock.SetActive(false);
        }

        // TV setup

        game.tvs[0].textures[0] = tvTextures[index * 2];
        game.tvs[0].textures[1] = tvTextures[index * 2 + 1];

        // Switch setup

        powerTubeSwitch.color = LogicColors.indexToLogicColor(index);

        // Cable setup

        foreach (LogicCable c in powerTubeCables)
        {
            c.unitsPerSecond = Random.Range(0.25f, 1f) + (startedActivated ? 10000 : 0);
            c.fade = true;
        }

        // Lighting setup
        
        foreach (Light l in upperDeckLights)
        {
            l.color = indexColor;
            l.intensity = 0;
        }

        switchAreaLight.color = indexColor;
        entranceDoorPower.color = LogicColors.vectorToColor(new Vector3(indexColor.r, indexColor.g, indexColor.b));
    }

    void Start()
    {
        if (startedActivated)
            powerTubeSwitch.interact(0);
        entranceDoorPower.updateEmission();
    }

    void Update()
    {
        // If we activated everything immediately, we need to update very little

        for (int l = 0; l < 3; l++)
            upperDeckLights[l].intensity = entranceDoor.getOpen() * (l == 0 ? 2 : 5);

        if (Time.timeSinceLevelLoad < 5)
        {
            float t = Mathf.InverseLerp(0.1f, 3f, Time.timeSinceLevelLoad);
            t = t * t * t * (t * (t * 6 - 15) + 10);
            introFade.color = new Color(0, 0, 0, (1 - t) * 0.5f);
            AudioListener.volume = t * t;
            game.playerCursor.cursorScale = t;
        }

        if (startedActivated)
        {
            powerTubeParticles.Emit(15);

            for (int i = 0; i < 3; i++)
                turbines[i].Rotate(0, 0, turbineMaxSpeed[i] * (i == 1 ? -1 : 1));

            return;
        }

        // When the user flips the switch, update the game progress (this is robust so
        // that progression works arbitrarily in the editor) and then save the game immediately

        if (!hasSwitched && powerTubeSwitch.isOn())
        {
            hasSwitched = true;

            GameController.LEVEL_PROGRESS[0] = GameController.LEVEL_COUNT[0];
            GameController.GAME_PROGRESS[0] = 1;

            if (indexString == "Green" || indexString == "Blue")
            {
                GameController.LEVEL_PROGRESS[1] = GameController.LEVEL_COUNT[1];
                GameController.GAME_PROGRESS[0] = 2;
                GameController.GAME_PROGRESS[1] = 1;
            }

            if (indexString == "Blue")
            {
                GameController.LEVEL_PROGRESS[2] = GameController.LEVEL_COUNT[2];
                GameController.GAME_PROGRESS[1] = 2;
                GameController.GAME_PROGRESS[2] = 1;
            }

            game.saveGame("SwitchRoom" + indexString);

            // Also, open the gate so they can actually walk to the TV now

            sequenceBreakBlock.SetActive(false);
        }

        // If they've flipped the switch, wait for the signal to begin the animation

        if (hasSwitched && !hasActivated)
        {
            float i = powerTubeSwitchCable.getData().magnitude;
            i = i * i * i * (i * (i * 6 - 15) + 10);
            switchLight.intensity = 8 * (1 - i);
            switchAreaLight.intensity = 3 * (1 - i);

            Vector3 input = powerTubeInput.getInput();

            if (input != Vector3.zero)
            {
                hasActivated = true;
                activeTime = Time.time;
                game.player.cameraShake(0.05f);
                powerTubeSounds[0].Play();

                switchLight.gameObject.SetActive(false);
                switchAreaLight.gameObject.SetActive(false);

                hum = 0.25f;
            }
        }

        // Do the animation

        if (hasActivated)
        {
            float t = (Time.time - activeTime) / warmupTime;
            t = Mathf.Pow(Mathf.Clamp01(t), 4);
            float s = 0;
            if (t == 1)
                s = 1 / (1 + 2.5f * (Time.time - (activeTime + warmupTime)));

            // Visual effects

            Vector3 data = new Vector3(indexColor.r, indexColor.g, indexColor.b) * t;
            powerTube.material.SetVector("_Data", data);
            powerTube.material.SetFloat("_Flash", s);

            powerTubeLight.transform.position = Vector3.up * Mathf.Lerp(5, 15, t);
            powerTubeLight.intensity = Mathf.Clamp01(t * 10) * 8;
            powerTubeLight.bounceIntensity = t * 0.5f + s * 5;
            powerTubeLight.color = Color.Lerp(indexColor, Color.white, 0.125f + s * 0.5f);

            powerTubeParticles.transform.localScale = new Vector3(1.2f, 1.2f, t);
            powerTubeParticles.startColor = Color.Lerp(indexColor, Color.white, 0.125f + s * 0.875f);
            if (t > 0)
                powerTubeParticles.Emit(15);

            // Cables

            if (!hasZapped && t == 1)
            {
                hasZapped = true;
                zappedTime = Time.time;
                game.player.cameraShake(0.4f);

                data = new Vector3(indexColor.r, indexColor.g, indexColor.b);
                foreach (LogicOutput o in powerTubeCableOutputs)
                    o.setOutput(data);
            }

            // Turbines

            if (hasZapped && game.gameState() == GameState.Play)
            {
                float tz = (Time.time - zappedTime) / 2;

                for (int i = 0; i < 3; i++)
                {
                    if (turbineSpeed[i] < turbineMaxSpeed[i] && tz > i)
                        turbineSpeed[i] += turbineAccel[i];
                    turbines[i].Rotate(0, 0, turbineSpeed[i] * (i == 1 ? -1 : 1));
                }
            }

            // Power tube audio

            if (hasActivated)
                hum = Mathf.Min(hum + 0.001f, 1);
            float h = Mathf.SmoothStep(0, 1, hum * hum) * 0.75f;
            powerTubeSounds[1].volume = h;
            powerTubeSounds[1].pitch = h;
            powerTubeSounds[2].volume = h;
            powerTubeSounds[2].pitch = h * 2;

            // Wire audio

            if (hasZapped)
            {
                foreach (AudioSource a in powerTubeWireSounds)
                {
                    if (!a.isPlaying)
                        a.Play();
                }

                float sc = (Time.time - zappedTime) * 1.5f;
                powerTubeWireSounds[0].transform.parent.localScale = new Vector3(1, sc, 1);
            }
        }
    }
}