using UnityEngine;
using System.Collections;

public class FanPosterVisual : MonoBehaviour {

	// Use this for initialization
	void Start () {
		PlayerID rand = (PlayerID)Random.Range(0,ControllerManager.instance.NumPlayers) + 1;
		ProfileData profile = ProfileManager.instance.GetProfile(rand);
		if(profile != null) {
			transform.GetChild(0).GetComponent<TextMesh>().text = "GO\n" + profile.Name + "!!!";
			transform.GetChild(1).GetComponent<TextMesh>().text = "GO\n" + profile.Name + "!!!";
			transform.GetChild(0).GetComponent<TextMesh>().color = profile.SecondaryColor;
			transform.GetChild(2).GetComponent<SpriteRenderer>().color = profile.PrimaryColor;
		}
		Destroy(this);
	}
}
