using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UINumbering : MonoBehaviour {
    public Sprite[] sprites = new Sprite[8];
	// Use this for initialization
	void Start () {
        Spawner spawner = GameObject.FindGameObjectWithTag("spawner").GetComponent<Spawner>();
        if (spawner.spawner.Length == 0) Destroy(gameObject);
        Image img = GetComponent<Image>();
        img.sprite = sprites[spawner.spawner.Length - 1];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
