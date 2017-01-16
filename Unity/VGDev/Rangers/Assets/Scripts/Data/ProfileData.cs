using System;
using UnityEngine;
using Assets.Scripts.Util;

/// <summary>
/// Class for saving player profile data
/// </summary>
[Serializable]
public class ProfileData
{
	private string tag;
	private Color primary, secondary;

	private bool isAI;

    public ProfileData()
    {
        tag = "";
		primary = UnityEngine.Random.ColorHSV(0,1f,0.75f,0.75f,0.75f,0.75f);
		secondary = UnityEngine.Random.ColorHSV(0,1f,0.75f,0.75f,0.75f,0.75f);
    }
    public ProfileData(string name)
    {
        tag = name;
		primary = UnityEngine.Random.ColorHSV(0,1f,0.75f,0.75f,0.75f,0.75f);
		secondary = UnityEngine.Random.ColorHSV(0,1f,0.75f,0.75f,0.75f,0.75f);
    }

	public Color PrimaryColor {
		get {
			return primary;
		}
		set {
			primary = value;
		}
	}

	public Color SecondaryColor {
		get {
			return secondary;
		}
		set {
			secondary = value;
		}
	}

    public string Name
    {
        get { return tag; }
        set { tag = value; }
    }

	public void SetAI() {
		isAI = true;
		primary = Color.black;
		secondary = Color.red;
	}


}
