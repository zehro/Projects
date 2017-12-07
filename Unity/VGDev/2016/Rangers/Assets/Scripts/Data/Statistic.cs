using UnityEngine;
using System.Collections.Generic;

public class Statistic {

	public int arrowsShot, arrowsHit, suicides;

	private float accuracy;

	public Dictionary<PlayerID, int> killedPlayer, killedByPlayer;

	public Statistic() {
		killedPlayer = new Dictionary<PlayerID, int>();
		killedByPlayer = new Dictionary<PlayerID, int>();
	}

	public float Accuracy {
		get {
			return (float)arrowsHit/Mathf.Max(1,arrowsShot);
		}
	}

	public string InfoText {
		get {
			string forReturn = "Accuracy- " + Accuracy.ToString("F1") + "\n";
			forReturn += "Suicides- " + suicides + "\n";
			forReturn += "Killed-\n";
			foreach (KeyValuePair<PlayerID,int> kvp in killedPlayer) {
				forReturn += " " + ProfileManager.instance.GetProfile(kvp.Key).Name + ": " + kvp.Value + "\n";
			}
			forReturn += "\nKilled By-\n";
			foreach (KeyValuePair<PlayerID,int> kvp in killedByPlayer) {
				forReturn += " " + ProfileManager.instance.GetProfile(kvp.Key).Name + ": " + kvp.Value + "\n";
			}
			return forReturn;
		}
	}

}
