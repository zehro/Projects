using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class MatchSummaryManager : MonoBehaviour {

	public static Dictionary<PlayerID,int> playerInfo;

	public static KeyValuePair<PlayerID,int>[] orderedPlayers;

	public static PlayerID winner;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}

	void OnLevelWasLoaded(int level) {
		if (SceneManager.GetActiveScene().name.Equals("MatchSummary")) {
			orderedPlayers = playerInfo.OrderByDescending((KeyValuePair<PlayerID, int> arg) => arg.Value).ToArray();

			for(int i = 0; i < orderedPlayers.Length; i++) {
				if(i > 0 && orderedPlayers[i-1].Value == orderedPlayers[i].Value) {
					playerInfo[orderedPlayers[i].Key] = i-1;
				} else {
					if(i == 0) playerInfo[orderedPlayers[i].Key] = 0;
					else playerInfo[orderedPlayers[i].Key] = playerInfo[orderedPlayers[i-1].Key]+1;

				}
			}        


			GameObject p1 = GameObject.Find("Player_Cosmetic_Winner");
			GameObject p2 = GameObject.Find("Player_Cosmetic (1)");
			GameObject p3 = GameObject.Find("Player_Cosmetic (2)");
			GameObject p4 = GameObject.Find("Player_Cosmetic (3)");

			p1.GetComponent<CosmeticPlayer>().id = playerInfo.Keys.ElementAt(0);
			p2.GetComponent<CosmeticPlayer>().id = playerInfo.Keys.ElementAt(1);

			if(playerInfo.Count == 2) {
				p3.SetActive(false);
				p4.SetActive(false);
				p2.transform.position = new Vector3(-0.5f,-3f,-0.5f);
				p1.transform.position = new Vector3(0.5f,-3f,-0.5f);
			}
			else if(playerInfo.Count == 3) {
				p3.GetComponent<CosmeticPlayer>().id = playerInfo.Keys.ElementAt(2);
				p4.SetActive(false);
				p3.transform.position = new Vector3(-1f,-3f,-0.5f);
				p2.transform.position = new Vector3(0f,-3f,-0.5f);
				p1.transform.position = new Vector3(1f,-3f,-0.5f);
			}
			else if(playerInfo.Count == 4) {
				p3.GetComponent<CosmeticPlayer>().id = playerInfo.Keys.ElementAt(2);
				p4.GetComponent<CosmeticPlayer>().id = playerInfo.Keys.ElementAt(3);
			}

		} else {
			Destroy(this.gameObject);
		}
	}
}
