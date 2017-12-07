using UnityEngine;
using System.Collections;
using Assets.Scripts.UI;
using Assets.Scripts.Managers;

public class UILevelSwitch : MonoBehaviour {

	public string[] levels = {"Loading",
		"MenuTest",
		"Testing",
		"BoosterPackSelection",
		"Credits",
		"Level1",
		"Level2",
		"Level3",
		"Level4",
		"Level5",
		"BoosterPackSelectionMultiplayer",
		"MultiplayerBattle"};

	public void LevelSwitch(string level) {
		LoadingScreen.instance.LoadLevel(level);
	}

	public void LevelSwitchForPackSelection() {
		GameManager.CardSelect = true;
		//Application.LoadLevel(3);
        LoadingScreen.instance.LoadLevel(3);
    }

	public void LevelSwitchForMultiplayerPackSelection() {
		GameManager.CardSelect = true;
		Application.LoadLevel(10);
        LoadingScreen.instance.LoadLevel(10);
    }

	public void ReloadLevel() {
		LevelSwitch(Application.loadedLevelName);
	}

	public void SetLoadingLevel(string _level) {
		LoadingScreen.LevelToLoad = _level;
	}

	public void DeckSelectNextLevel() {
        DeckTransfer[] decks = GameObject.FindObjectsOfType<DeckTransfer>();
        for(int i = 0; i < decks.Length; i++)
        {
            Destroy(decks[i].gameObject);
        }
		SetLoadingLevel(getNextLevel());
		LevelSwitchForPackSelection();
	}

	public void DeckSelectThisLevel() {
        DeckTransfer[] decks = GameObject.FindObjectsOfType<DeckTransfer>();
        for (int i = 0; i < decks.Length; i++)
        {
            Destroy(decks[i].gameObject);
        }
        LevelSwitchForPackSelection();
	}

	public void DirectNextLevel() {
		LevelSwitch(getNextLevel());
	}

	public void BackToMenu() {
		LevelSwitch("MenuTest");
	}

	private string getNextLevel() {
		int current = Application.loadedLevel;
		return levels[++current];
	}
}
