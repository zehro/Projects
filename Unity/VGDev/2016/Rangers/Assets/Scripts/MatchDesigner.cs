using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Util;

public class MatchDesigner : MonoBehaviour {

	public ValueModifierUI kills, stock, time;
	public Text matchType;

//	private int killLimit, stockLimit;
//
//	private float timeLimit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Gets the current game settings set up by the current match designer.
	/// </summary>
	/// <returns>The game settings</returns>
	public GameSettings GetSettings() {
		GameSettings settings = new GameSettings();

		if (kills.gameObject.activeInHierarchy)
			settings.KillLimit = kills.value;
		else
			settings.KillLimit = Mathf.Infinity;
		
		if (stock.gameObject.activeInHierarchy)
			settings.StockLimit = stock.value;
		else
			settings.StockLimit = Mathf.Infinity;

		settings.TimeLimit = time.value;

		settings.Type = (Enums.GameType)System.Enum.Parse(typeof(Enums.GameType),matchType.text);

		return settings;
	}
}
