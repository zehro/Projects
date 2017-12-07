using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;

public class TokenToSprite : MonoBehaviour {

	public static TokenToSprite instance;
	public Enums.Arrows[] types;
	public Sprite[] images;

	public Dictionary<Enums.Arrows,Sprite> dict;

	void Start() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(this);
		}

		dict = new Dictionary<Enums.Arrows, Sprite>();
		for(int i = 0; i < types.Length; i++) {
			dict.Add(types[i],images[i]);
		}
	}

}
