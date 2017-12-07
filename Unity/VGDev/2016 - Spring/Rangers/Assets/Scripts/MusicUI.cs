using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MusicUI : MonoBehaviour {

	private Image playIcon;
	private Text songTitle;
	private float h;

	// Use this for initialization
	void Start () {
		playIcon = transform.FindChild("PlayIcon").GetComponent<Image>();
		songTitle = transform.FindChild("SongTitle").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		h += Time.deltaTime/5f;
		if(h > 1) h = 0;
		Color temp = Color.HSVToRGB(h,1,1);
		playIcon.color = new Color(temp.r,temp.g,temp.b, 0.5f);
		songTitle.text = "\"" + MusicManager.instance.currentTrackTitle + "\"";
	}
}
