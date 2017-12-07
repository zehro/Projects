using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	public static MusicManager instance;

	public AudioClip[] tracks;
	private AudioSource player;
	private int currentTrack = 0;

	[HideInInspector]
	public string currentTrackTitle;

	// Use this for initialization
	void Start () {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(this.gameObject);
		}
		player = GetComponent<AudioSource>();
		player.clip = tracks[Random.Range(0,tracks.Length)];
		currentTrackTitle = player.clip.name;
	}
	
	// Update is called once per frame
	void Update () {
		if(!player.isPlaying || ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.RightStickClick)) {
			player.clip = tracks[(++currentTrack)%tracks.Length];
			currentTrackTitle = player.clip.name;
			player.Play();
		}
	}
}
