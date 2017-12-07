using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Util;

public class MapSelector : MonoBehaviour {

	public bool arenaSelector;

	public int currentSelectedMap;

	public Text mapTitle;
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(-1000*currentSelectedMap,0f),Time.deltaTime*Mathf.Abs((transform.localPosition.x+(1000*currentSelectedMap)))*2f);
	}

	public void NextMap() {
		currentSelectedMap = (currentSelectedMap + 1) % GetNumStages();
		UpdateStageName();
	}

	public void PrevMap() {
		currentSelectedMap -= 1;
		if (currentSelectedMap < 0) {
			currentSelectedMap = GetNumStages() - 1;
		}
		UpdateStageName();
	}

	/// <summary>
	/// Gets the number of stages that can be selected.
	/// </summary>
	/// <returns>the number of stages that can be selected.</returns>
	private int GetNumStages() {
		return arenaSelector ? ((int)Enums.BattleStages.NumStages) : ((int)Enums.TargetPracticeStages.NumStages);
	}

	/// <summary>
	/// Gets the name of the currently selected stage.
	/// </summary>
	/// <returns>The name of the currently selected stage.</returns>
	private void UpdateStageName() {
		string currentStageName = arenaSelector ? GetBattleStageName() : GetTargetStageName();
		mapTitle.text = currentStageName;
		mapTitle.transform.GetChild(0).GetComponent<Text>().text = currentStageName;
		mapTitle.transform.GetChild(1).GetComponent<Text>().text = currentStageName;
	}

	/// <summary>
	/// Gets the name of the current battle stage.
	/// Used to put spaces where they should be.
	/// </summary>
	/// <returns>The name of the current battle stage.</returns>
	private string GetBattleStageName() {
		Enums.BattleStages stage = (Enums.BattleStages)currentSelectedMap;
		switch (stage) {
		default: return AddSpaces(stage.ToString());
		}
	}

	/// <summary>
	/// Gets the name of the current target stage.
	/// Used to put spaces where they should be.
	/// </summary>
	/// <returns>The name of the current target stage.</returns>
	private string GetTargetStageName() {
		Enums.TargetPracticeStages stage = (Enums.TargetPracticeStages)currentSelectedMap;
		switch (stage) {
		default: return AddSpaces(stage.ToString());
		}
	}

	/// <summary>
	/// Adds spaces to a camel-case name.
	/// </summary>
	/// <returns>The name with spaces added to it.</returns>
	/// <param name="name">The name to add spaces to.</param>
	private string AddSpaces(string name)
	{
		for (int i = name.Length - 1; i > 0; i--)
		{
			int letter = (int)name[i];
			if (letter >= 48 && letter <= 57 || letter >= 65 && letter <= 90)
			{
				// Capital letters or numbers.
				name = name.Insert(i, " ");
			}
		}
		return name;
	}
}
