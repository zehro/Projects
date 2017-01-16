using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    public Texture tex;
    public Shader shader;

    public float fxDesync;
    float fxDesyncFlare = 0;
    float fxDesyncFlareFalloff = 0.9f;
    float fxDesyncFinale = 0;

    GameController game;
    Material mat;

    void Awake()
    {
        game = GetComponentInParent<GameController>();
        mat = new Material(shader);
        mat.hideFlags = HideFlags.DontSave;
    }

    void Update()
    {
        fxDesyncFlare *= fxDesyncFlareFalloff;

        // Manage effects

        float t;
        float s;

        if (game == null)
        {
            s = 0;
            if (Random.Range(0, 480) < 1)
                s += Random.Range(0, 1000);
            if (Random.Range(0, 240) < 1)
                s += Random.Range(0, 10);
            if (Random.Range(0, 3) < 1)
                s += Random.Range(0, 0.01f);
            fxDesync = s + fxDesyncFlare;
            return;
        }

        switch (game.gameState())
        {
            case GameState.EnteringLevel:
                fxDesync = Random.Range(50, 30000);
                break;

            case GameState.Play:
                s = 1 / (game.getStateTime() * 25 + 0.1f);
                if (s < 0.001f)
                    s = 0;
                fxDesync = s + fxDesyncFlare;
                break;

            case GameState.Stopping:
                t = game.getStateTime() / game.getStoppingTime();
                s = Random.Range(1.1f, 1.2f) + Mathf.Pow(t, 20) * 100;
                if (t < 0.01f)
                    s += 2;
                fxDesync = s;
                break;

            case GameState.Pause:
                t = 1 / (game.getStateTime() * 10 + 0.1f);
                s = Mathf.Pow(t, 50);
                if (Random.Range(0, 240) < 1)
                    s += Random.Range(0, 1000);
                if (Random.Range(0, 120) < 1)
                    s += Random.Range(0, 10);
                if (Random.Range(0, 3) < 1)
                    s += Random.Range(0, 0.01f);
                fxDesync = s + fxDesyncFlare;
                break;

            case GameState.Starting:
                fxDesync = Random.Range(50, 300);
                break;

            case GameState.ExitingLevel:
                float f = Mathf.Pow(fxDesyncFinale, 3f);
                float lo = 200 - f * 200;
                float hi = 200 + f * 200;
                fxDesync = Random.Range(f * lo, f * hi);
                if (Random.Range(0, 180 - fxDesyncFinale * 170) < 1)
                    fxDesync += Random.Range(0, fxDesyncFinale * 5000);
                if (f > 0.4f)
                    fxDesync += Random.Range(0, Mathf.InverseLerp(0.4f, 1, f) * Random.Range(300, 500000000));
                break;
        }
    }

    public void fxFlare(float flare, float falloff)
    {
        if (flare > fxDesyncFlare)
            fxDesyncFlare = flare;
        fxDesyncFlareFalloff = falloff;
    }

    public void fxFinale(float finale)
    {
        fxDesyncFinale = finale;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
        mat.SetTexture("_GlitchTex", tex);
        mat.SetFloat("_FXTime", Time.unscaledTime);
        mat.SetFloat("_FXDesync", fxDesync);
        Graphics.Blit(source, destination, mat);
    }
}