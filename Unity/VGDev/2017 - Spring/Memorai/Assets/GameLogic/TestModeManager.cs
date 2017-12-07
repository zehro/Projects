using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestModeManager : MonoBehaviour {
    public Text text;
    public Button leftButton;
    public Button rightButton;
    int index = 0;

    public GameObject[] enemies;
	void Start() {
        text.text = enemies[index].name;
    }
    public void change(int i) {
        if (index + i < enemies.Length && index + i >= 0) {
            index += i;
            text.text = enemies[index].name;
        }
    }

    public void create() {
        Instantiate(enemies[index], new Vector2(0, 5), Quaternion.identity);
    }

}
