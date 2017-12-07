using UnityEngine;
using UnityEngine.UI;

public class UIVideo : MonoBehaviour {

    MovieTexture movie;
    void Start () {
		movie = this.GetComponent<RawImage>().texture as MovieTexture;
		movie.loop = true;
		movie.Play();
	}

	void Update() {
		if (!movie.isPlaying) {
			movie.loop = true;
			movie.Play();
		}
	}
}
