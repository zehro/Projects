using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Data;
using Assets.Scripts.Tokens;
using Assets.Scripts.Util;
using Assets.Scripts.Arrows;

public class InGamePlayerInfoUI : MonoBehaviour {

	public PlayerID id;

	private GameObject tagText, indicatorsContainer, crownIcon;

	private Text livesText, deathsText, killsText;

	private Image token1, token2, token3;

	private int prevNumTypes;

	// Use this for initialization
	void Start () {
		tagText = transform.FindChild("TagBox").GetChild(0).gameObject;
		indicatorsContainer = transform.FindChild("Player1Indicator").gameObject;

		for (int i = 0; i < indicatorsContainer.transform.childCount; i++) {
			if (i == (int)(id-1))
				indicatorsContainer.transform.GetChild(i).GetComponent<Image>().color = Color.white;
			else
				indicatorsContainer.transform.GetChild(i).GetComponent<Image>().color = Color.black;
		}

		tagText.GetComponent<Text>().text = ProfileManager.instance.GetProfile(id).Name;
		tagText.GetComponent<Text>().color = ProfileManager.instance.GetProfile(id).SecondaryColor;
		tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(id).PrimaryColor;

		token1 = transform.FindChild("TokenDisplay").FindChild("Token 1").GetChild(0).GetComponent<Image>();
		token2 = transform.FindChild("TokenDisplay").FindChild("Token 2").GetChild(0).GetComponent<Image>();
		token3 = transform.FindChild("TokenDisplay").FindChild("Token 3").GetChild(0).GetComponent<Image>();

		crownIcon = transform.FindChild("CrownIcon").gameObject;

		if(GameManager.instance.CurrentGameSettings.Type == Assets.Scripts.Util.Enums.GameType.Deathmatch) {
			transform.FindChild("DeathMatchInfo").gameObject.SetActive(true);
			deathsText = transform.FindChild("DeathMatchInfo").FindChild("DeathValue").GetComponent<Text>();
			killsText = transform.FindChild("DeathMatchInfo").FindChild("KillsValue").GetComponent<Text>();
		} else if(GameManager.instance.CurrentGameSettings.Type == Assets.Scripts.Util.Enums.GameType.Stock) {
			transform.FindChild("StockInfo").gameObject.SetActive(true);
			livesText = transform.FindChild("StockInfo").FindChild("LivesValue").GetComponent<Text>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(crownIcon != null) {
			if(livesText) livesText.text = GameManager.instance.GetPlayer(id).LifeComponent.Lives.ToString();
			if(deathsText) deathsText.text = GameManager.instance.GetPlayer(id).LifeComponent.Deaths.ToString();
			if(killsText) killsText.text = GameManager.instance.GetPlayer(id).LifeComponent.kills.ToString();

			token1.sprite = null;
			token2.sprite = null;
			token3.sprite = null;

			token1.color = new Color(1,1,1,0.5f);
			token2.color = new Color(1,1,1,0.5f);
			token3.color = new Color(1,1,1,0.5f);

			int numTypes = 0;
			int types = GameManager.instance.GetPlayer(id).ArcheryComponent.ArrowTypes;
			for(int i = 0; i < (int)Enums.Arrows.NumTypes; i++)
			{
				// Check to see if the type exists in the current arrow
				if(Bitwise.IsBitOn(types, i) && ((Enums.Arrows)i) != Enums.Arrows.Normal)
				{
					// Add an arrow property and update the delegates
					if(numTypes == 0) {
						token1.sprite = TokenToSprite.instance.dict[(Enums.Arrows)i];
						token1.color = Color.white;
						numTypes++;
					}
					else if(numTypes == 1) {
						token2.sprite = TokenToSprite.instance.dict[(Enums.Arrows)i];
						token2.color = Color.white;
						numTypes++;
					}
					else if(numTypes == 2) {
						token3.sprite = TokenToSprite.instance.dict[(Enums.Arrows)i];
						token3.color = Color.white;
						numTypes++;
					}
				}
			}

			if(prevNumTypes != numTypes) {
				token1.transform.localScale = Vector3.one*2f;
				token2.transform.localScale = Vector3.one*2f;
				token3.transform.localScale = Vector3.one*2f;
				token1.transform.parent.localScale = Vector3.one*2f;
				token2.transform.parent.localScale = Vector3.one*2f;
				token3.transform.parent.localScale = Vector3.one*2f;
			} else {
				token3.transform.localScale = Vector3.MoveTowards(token3.transform.localScale,Vector3.one,Time.deltaTime);
				token3.transform.parent.localScale = Vector3.MoveTowards(token3.transform.parent.localScale,Vector3.one,Time.deltaTime*3f);
				token2.transform.localScale = Vector3.MoveTowards(token2.transform.localScale,Vector3.one,Time.deltaTime*2f);
				token2.transform.parent.localScale = Vector3.MoveTowards(token2.transform.parent.localScale,Vector3.one,Time.deltaTime*3f);
				token1.transform.localScale = Vector3.MoveTowards(token1.transform.localScale,Vector3.one,Time.deltaTime*3f);
				token1.transform.parent.localScale = Vector3.MoveTowards(token1.transform.parent.localScale,Vector3.one,Time.deltaTime*3f);
			}

			prevNumTypes = numTypes;
				
			if(GameManager.instance.CurrentWinner == id) crownIcon.SetActive(true);
			else crownIcon.SetActive(false);
		}
	}
}
