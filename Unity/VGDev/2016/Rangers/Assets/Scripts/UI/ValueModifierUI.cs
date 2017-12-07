using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Util;

public class ValueModifierUI : MonoBehaviour {

	private enum ValueModifierUIType {Numerical, Time, MatchType};

	[SerializeField]
	private ValueModifierUIType type;

	public float value;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void IncrementValue() {
		switch(type) {
			case ValueModifierUIType.Numerical:
				value++;
				break;
			case ValueModifierUIType.Time:
				value += 10;
				break;
			case ValueModifierUIType.MatchType:
				value++;
				value = ((int)value) % (int)Enums.GameType.NumTypes;
				break;
		}
		UpdateText();
	}

	public void DecrementValue() {
		switch(type) {
		case ValueModifierUIType.Numerical:
			value--;
			break;
		case ValueModifierUIType.Time:
			value -= 10;
			break;
		case ValueModifierUIType.MatchType:
			value--;
			if (value < 0) {
				value = (int)Enums.GameType.NumTypes - 1;
			}
			break;
		}
		UpdateText();
	}

	private void UpdateText() {
		switch(type) {
		case ValueModifierUIType.Numerical:
			if(value <= 0) {
				value = 0;
				transform.GetChild(1).GetComponent<Text>().text = "∞";
				transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "∞";
				transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "∞";
			} else {
				transform.GetChild(1).GetComponent<Text>().text = value + "";
				transform.GetChild(1).GetChild(0).GetComponent<Text>().text = value + "";
				transform.GetChild(1).GetChild(1).GetComponent<Text>().text = value + "";
			}
			break;
		case ValueModifierUIType.Time:
			if(value <= 0) {
				value = 0;
				transform.GetChild(1).GetComponent<Text>().text = "∞";
				transform.GetChild(1).GetChild(0).GetComponent<Text>().text = "∞";
				transform.GetChild(1).GetChild(1).GetComponent<Text>().text = "∞";
			} else {
				int tempVal = ((int)(value)%60);
				transform.GetChild(1).GetComponent<Text>().text = ((int)(value/60)) + ":" + (tempVal == 0 ? "00" : tempVal+"");
				transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ((int)(value/60)) + ":" + (tempVal == 0 ? "00" : tempVal+"");
				transform.GetChild(1).GetChild(1).GetComponent<Text>().text = ((int)(value/60)) + ":" + (tempVal == 0 ? "00" : tempVal+"");
			}
			break;
		case ValueModifierUIType.MatchType:
			transform.GetChild(1).GetComponent<Text>().text = ((Enums.GameType)((int)value)).ToString();
			transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ((Enums.GameType)((int)value)).ToString();
			transform.GetChild(1).GetChild(1).GetComponent<Text>().text = ((Enums.GameType)((int)value)).ToString();
			break;
		}
	}
}
