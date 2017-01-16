using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{
	public AudioClip levelStart;
	public AudioClip levelEnd;
	public AudioClip checkpointSound;
	public AudioClip checkpointBoostSound;

	GameController controller;
	AudioClip ambience;
	AudioClip music;

	AudioSource ambienceSource;
	AudioSource musicSource;

	AudioSource[] checkpointAudio;

	void Awake()
	{
		// Get the ambience and music from the game controller

		controller = transform.parent.gameObject.GetComponent<GameController>();
		ambience = controller.ambience;
		music = controller.music;

		// Play the ambience

		ambienceSource = transform.Find("Ambience").GetComponent<AudioSource>();
		ambienceSource.clip = ambience;
		ambienceSource.Play();

		// Set up the level start music

		musicSource = transform.Find("Music").GetComponent<AudioSource>();
		musicSource.clip = levelStart;

		// Get the checkpoint audio sources

		checkpointAudio = transform.Find("CheckpointAudio").GetComponents<AudioSource>();
	}

	void Start()
	{
		// Play the level start music

		musicSource.PlayDelayed(1.5F);
	}

	public void playLevelMusic()
	{
		musicSource.clip = music;
		musicSource.loop = true;
		musicSource.Play();
	}

	public void playEndMusic()
	{
		musicSource.Stop();
		musicSource.clip = levelEnd;
		musicSource.loop = false;
		musicSource.Play();
	}

	public void playCheckpointSound(int index, bool boost)
	{
		AudioClip clip = checkpointSound;
		if (boost)
			clip = checkpointBoostSound;

		checkpointAudio[index].PlayOneShot(clip);
	}
}