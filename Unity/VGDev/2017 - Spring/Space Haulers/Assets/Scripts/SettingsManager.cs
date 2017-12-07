using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

struct Settings
{
    public float volume;
    public float fov;
    public bool fullscreen;
    public QualitySettings quality;

    public Settings(float volume, float fov, bool fullscreen, QualitySettings quality)
    {
        this.volume = volume;
        this.fov = fov;
        this.fullscreen = fullscreen;
        this.quality = quality;
    }
}

public class SettingsManager : MonoBehaviour {

    public Slider fovSlider;
    public new Camera camera;
    public Button fullscreenButton;
    Settings settings;

	void Start () {
        LoadSettings();
	}
	
	void Update () {
		
	}

    //LoadSettings can be recoded later to actually use the struct
    public void LoadSettings()
    {
        if (camera != null) {
            camera.fieldOfView = PlayerPrefs.HasKey("FOV") ? PlayerPrefs.GetFloat("FOV") : 75f;
        }
        if (fovSlider != null) {
            fovSlider.value = PlayerPrefs.HasKey("FOV") ? PlayerPrefs.GetFloat("FOV") : 75f;
        }
        int screen = PlayerPrefs.HasKey("FullScreen") ? PlayerPrefs.GetInt("FullScreen") : 0;
        int quality = PlayerPrefs.HasKey("QualityLevel") ? PlayerPrefs.GetInt("QualityLevel") : 5;
        Screen.fullScreen = (screen == 0) ? false : true;
        QualitySettings.SetQualityLevel(quality);
    }

    public void SaveSettings()
    {
 
    }

    public void SetFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0);
    }

    void SetScreenResolution(int i)
    {
        Screen.SetResolution(Screen.resolutions[i].width, Screen.resolutions[i].height, settings.fullscreen);
    }

    public void SetQuality(int i)
    {
        QualitySettings.SetQualityLevel(i);
        PlayerPrefs.SetInt("QualityLevel", i);
    }

    public void setFOV()
    {
        PlayerPrefs.SetFloat("FOV", fovSlider.value);
    }
}
