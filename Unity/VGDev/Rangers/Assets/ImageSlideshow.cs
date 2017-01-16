using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSlideshow : MonoBehaviour {

	private int selectedImage;

	public void NextImage() {
		selectedImage++;
		if(selectedImage >= transform.childCount) selectedImage = 0;

		for(int i = 0; i < transform.childCount; i++) {
			if(i != selectedImage) {
				transform.GetChild(i).GetComponent<Image>().enabled = false;
			} else { 
				transform.GetChild(i).GetComponent<Image>().enabled = true;
			}
		}
	}

	public void PrevImage() {
		selectedImage--;
		if(selectedImage < 0) selectedImage = transform.childCount - 1;

		for(int i = 0; i < transform.childCount; i++) {
			if(i != selectedImage) {
				transform.GetChild(i).GetComponent<Image>().enabled = false;
			} else { 
				transform.GetChild(i).GetComponent<Image>().enabled = true;
			}
		}
	}
}
