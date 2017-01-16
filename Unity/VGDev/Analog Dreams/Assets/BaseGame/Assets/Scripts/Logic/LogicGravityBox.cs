using UnityEngine;
using System.Collections;

public class LogicGravityBox : MonoBehaviour
{
    [System.Serializable]
    public class Force
    {
        public LogicColor color;
        public GravityDirection gravity;
        public float pitch;
    }

    [SerializeField]
    public Force[] forces;
    public bool affectAll;
    public VariableGravity[] affected;

    GameController game;

    LogicInput input;
    ParticleSystem particles;
    MeshRenderer gBox;
    MeshRenderer gBoxGlow;
    Light gBoxLight;
    ParticleSystem gBoxParticles;
    AudioSource[] sound;

    Vector3 data;
    Vector3 prevData;
    Vector3 color;
    Vector3 gravity;
    float flash;
    float hum;
    float humPitch;
    float humPitchTarg;
    float humPitchDrag = 16;
    
    void Awake()
    {
        if (affectAll)
            affected = FindObjectsOfType<VariableGravity>();

        game = FindObjectOfType<GameController>();

        input = transform.Find("Input").GetComponent<LogicInput>();
        particles = transform.Find("Particles").GetComponent<ParticleSystem>();
        gBox = transform.Find("GBox").GetComponent<MeshRenderer>();
        gBoxGlow = transform.Find("GBox/GBoxGlow").GetComponent<MeshRenderer>();
        gBoxLight = transform.Find("GBox/GBoxLight").GetComponent<Light>();
        gBoxParticles = transform.Find("GBox/GBoxParticles").GetComponent<ParticleSystem>();
        sound = transform.Find("Audio").GetComponents<AudioSource>();
    }

    void Update()
    {
        prevData = data;
        data = input.getInput();

        // If the input to this box has changed, figure out its new color and gravity

        if (prevData != data)
        {
            color = Vector3.zero;
            gravity = Vector3.zero;
            
            LogicColor newC = LogicColors.vectorToColor(data);

            foreach (Force f in forces)
            {
                if (f.color == newC && f.color != LogicColor.Black)
                {
                    color = LogicColors.colorToVector(f.color);
                    gravity = gravityToVector(f.gravity);
                    humPitchTarg = f.pitch;
                    break;
                }
            }

            // Visual and audio effects

            flash = 1;
            if (color != Vector3.zero)
                sound[1].Play();
            else if (prevData != Vector3.zero && color == Vector3.zero)
                sound[2].Play();

            // Tell every object this gravity box affects to recalculate its gravity

            foreach (VariableGravity g in affected)
                g.recalculateBoxGravity();

            // Rotate the gbox to reflect which direction it's facing now

            if (color != Vector3.zero)
                gBox.transform.rotation = Quaternion.FromToRotation(Vector3.forward, gravity);
        }

        // Update the visual effects
        
        Color c = new Color(color.x, color.y, color.z) * 2;
        float t = Mathf.Pow(flash, 10);
        c = Color.Lerp(c, Color.white * 2, t);
        Color cPart = Color.Lerp(c, Color.white * (isActive() ? 1 : 0), 0.25f);

        flash = Mathf.Max(0, flash - 0.005f * Time.timeScale);

        gBox.material.SetColor("_EmissionColor", c);
        DynamicGI.SetEmissive(gBox, c);
        gBoxLight.color = c;
        gBoxLight.intensity = 3 + t * 3;
        gBoxLight.range = 7 + t * 3;
        gBoxParticles.startColor = cPart;

        c.a = Mathf.Max(t, 0.15f + Mathf.Sin(Time.time * 10) * 0.025f);
        gBoxGlow.material.SetColor("_TintColor", c);
        gBoxParticles.Emit((int)(c.a * 15));

        // Update the audio effects

        float targ = (isActive() ? 1 : 0);
        if (hum != targ)
            hum = Mathf.Clamp01(hum + Mathf.Sign(targ - hum) * 0.0075f * Time.deltaTime * 60);
        humPitch += (humPitchTarg - humPitch) / humPitchDrag * (Time.deltaTime * 60);

        if (targ == 1)
        {
            sound[0].pitch = (1 - Mathf.Pow(1 - hum, 6)) * Mathf.Pow(2, humPitch / 12f);
            sound[0].volume = 1 - Mathf.Pow(1 - hum, 9);
        }

        else
        {
            sound[0].pitch = Mathf.Pow(hum, 3) * Mathf.Pow(2, humPitch / 12f);
            sound[0].volume = hum;
        }

        // If this box is on, run particles on every object it's affecting

        if (isActive())
        {
            particles.GetComponent<Renderer>().enabled = true;
            particles.startColor = cPart;
            
            foreach (VariableGravity g in affected)
            {
                // Set position, rotation, and scale of the emitter to the shape

                particles.transform.position = g.transform.position;
                particles.transform.rotation = g.transform.rotation;
                Vector3 scale = g.GetComponent<MeshFilter>().mesh.bounds.size;
                scale.Scale(g.transform.localScale);
                particles.transform.localScale = scale;

                particles.Emit(2);
            }
        }

        if (particles.particleCount == 0)
        {
            particles.GetComponent<Renderer>().enabled = false;
        }

        // Keep the particle system on the camera so it is never culled

        particles.transform.position = game.playerCamera.transform.position;
        particles.transform.localScale = Vector3.one;
    }

    public bool isActive()
    {
        return (gravity != Vector3.zero);
    }

    public bool affects(VariableGravity test)
    {
        if (!isActive())
            return false;
        else
        {
            foreach (VariableGravity g in affected)
                if (g == test)
                    return true;
            return false;
        }
    }

    public Vector3 getGravity()
    {
        return gravity;
    }

    Vector3 gravityToVector(GravityDirection d)
    {
        switch (d)
        {
            case GravityDirection.Right:
                return Vector3.right;
            case GravityDirection.Left:
                return Vector3.left;
            case GravityDirection.Up:
                return Vector3.up;
            case GravityDirection.Down:
                return Vector3.down;
            case GravityDirection.Forward:
                return Vector3.forward;
            case GravityDirection.Back:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }

    public void setLight(bool on)
    {
        gBoxLight.enabled = on;
    }
}

public enum GravityDirection
{
    Right, Left, Up, Down, Forward, Back
}