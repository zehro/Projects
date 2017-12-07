using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BadgeAnimator : MonoBehaviour {

	public Sprite badge1_1,badge1_2,badge1_3,badge2_1,badge2_2,badge2_3,badge3_1,badge3_2,badge3_3;
	private PlayerID id;

	public RectTransform mask;
	public Text tagText, infoText;
	public AutoKeyUI close, up, down;

	private bool flipped;
	private float startTimer;
	private Vector3 initialScale = Vector3.one;

	private bool infoOpen = true;

	// Use this for initialization
	void Start () {
		if(MatchSummaryManager.playerInfo != null) {
			id = transform.root.GetComponent<CosmeticPlayer>().id;
			if(MatchSummaryManager.playerInfo.ContainsKey(id)) {
				switch(MatchSummaryManager.playerInfo[id]) {
					case 0:
						transform.GetChild(0).GetComponent<Image>().sprite = badge1_3;
						transform.GetChild(1).GetComponent<Image>().sprite = badge1_2;
						transform.GetChild(2).GetComponent<Image>().sprite = badge1_1;
						break;
					case 1:
						transform.GetChild(0).GetComponent<Image>().sprite = badge2_3;
						transform.GetChild(1).GetComponent<Image>().sprite = badge2_2;
						transform.GetChild(2).GetComponent<Image>().sprite = badge2_1;
						transform.localScale = Vector3.one*0.75f;
						break;
					default:
						transform.GetChild(0).GetComponent<Image>().sprite = badge3_3;
						transform.GetChild(1).GetComponent<Image>().sprite = badge3_2;
						transform.GetChild(2).GetComponent<Image>().sprite = badge3_1;
						transform.localScale = Vector3.one*0.5f;
						break;
				}
				close.id = id;
				up.id = id;
				down.id = id;
				infoText.text = StatisticManager.instance.statistics[id].InfoText;
				tagText.text = ProfileManager.instance.GetProfile(id).Name;
			}
		}
		transform.GetChild(0).localPosition = new Vector3(0,0,-3f);
		transform.GetChild(1).localPosition = new Vector3(0,0,-3f);
		transform.GetChild(2).localPosition = new Vector3(0,0,-3f);
		initialScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(startTimer > 2f) {
			if(mask && mask.sizeDelta.y < 2f && infoOpen) {
				mask.sizeDelta = Vector2.MoveTowards(mask.sizeDelta, new Vector2(1.3f,2f),(mask.sizeDelta.y+0.1f)*Time.deltaTime*3f);
			} else if (mask && mask.sizeDelta.y > 0f && !infoOpen) {
				mask.sizeDelta = Vector2.MoveTowards(mask.sizeDelta, new Vector2(1f,0f),(mask.sizeDelta.y+0.1f)*Time.deltaTime*3f);
			}
				
			transform.localScale = Vector3.Lerp(initialScale, initialScale/2f, mask.sizeDelta.y/2f);
			transform.localPosition = Vector3.Lerp(-Vector3.up, Vector3.up*0.7f, mask.sizeDelta.y/2f);

			if(id != PlayerID.None) {
				if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.X, id)) infoOpen = !infoOpen;
				if(infoText.rectTransform.anchoredPosition.y < -1.5f) {
					if(ControllerManager.instance.GetTrigger(ControllerInputWrapper.Triggers.RightTrigger, id) > 0) infoText.rectTransform.anchoredPosition += Vector2.up*Time.deltaTime;
					down.gameObject.SetActive(true);
				} else {
					down.gameObject.SetActive(false);
				}
				if(infoText.rectTransform.anchoredPosition.y > -2.5f) {
					if(ControllerManager.instance.GetTrigger(ControllerInputWrapper.Triggers.LeftTrigger, id) > 0) infoText.rectTransform.anchoredPosition -= Vector2.up*Time.deltaTime;
					up.gameObject.SetActive(true);
				} else {
					up.gameObject.SetActive(false);
				}
			}
		} else {
			startTimer += Time.deltaTime;
			transform.GetChild(0).localPosition = Vector3.MoveTowards(transform.GetChild(0).localPosition, new Vector3(0,0,0),Time.deltaTime*4f);
			transform.GetChild(1).localPosition = Vector3.MoveTowards(transform.GetChild(1).localPosition, new Vector3(0,0,-0.1f),Time.deltaTime*3f);
			transform.GetChild(2).localPosition = Vector3.MoveTowards(transform.GetChild(2).localPosition, new Vector3(0,0,-0.2f),Time.deltaTime*2f);
		}

	}
}
