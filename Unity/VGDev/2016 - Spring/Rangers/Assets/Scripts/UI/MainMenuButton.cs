using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : Selectable {

	private bool isSelected = false;

	private float startingTimer = 0f;

	private Transform parentText;

	// Use this for initialization
	protected override void Start () {
        base.Start();
		parentText.GetChild(0).GetComponent<Text>().CrossFadeAlpha(0.25f,0.5f, false);
		parentText.GetChild(1).GetComponent<Text>().CrossFadeAlpha(0.25f,0.5f, false);
	}

	protected override void OnEnable() {
		base.OnEnable();
		parentText = transform.GetChild(1);
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -10f);
		parentText.localPosition = new Vector3(parentText.localPosition.x, parentText.localPosition.y, -20f);
		parentText.GetChild(0).localPosition = new Vector3(parentText.GetChild(0).localPosition.x, parentText.GetChild(0).localPosition.y, 30f);
		parentText.GetChild(1).localPosition = new Vector3(parentText.GetChild(1).localPosition.x, parentText.GetChild(1).localPosition.y, 60f);
		startingTimer = 1f;
//		GetComponent<Selectable>().interactable = false;
	}
	
	// Update is called once per frame
	void Update () {
//		GetComponent<Selectable>().interactable = true;
		if(startingTimer > 0) {
			startingTimer -= Time.deltaTime;
			if(Random.value < startingTimer) {
//				GetComponent<Image>().color = new Color(1, 1, 1, Random.value+0.5f);
				parentText.GetComponent<Text>().color = new Color(1, 1, 1, Random.value+0.5f);
				parentText.GetChild(0).GetComponent<Text>().color = new Color(Random.value*2f + 0.5f, Random.value*2f + 0.5f, Random.value*2f + 0.5f, Random.value/2f);
				parentText.GetChild(1).GetComponent<Text>().color = new Color(Random.value*2f + 0.5f, Random.value*2f + 0.5f, Random.value*2f + 0.5f, Random.value/4f);
			} else {
//				GetComponent<Image>().color = Color.white;
				parentText.GetComponent<Text>().color = Color.white;
				parentText.GetChild(0).GetComponent<Text>().color = new Color(1,1,1,0.5f);
				parentText.GetChild(1).GetComponent<Text>().color = new Color(1,1,1,0.25f);
			}
		}
		if(isSelected) {
//			float colorVal = Mathf.Max(0.5f,Mathf.Min(1f,Mathf.Abs(Mathf.Sin(Time.time*5f))));
			float colorVal = Mathf.Abs(parentText.localPosition.z/10f);
			GetComponent<Image>().color = new Color(colorVal,colorVal,colorVal,1);
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,
				new Vector3(transform.localPosition.x, transform.localPosition.y, -5f), Time.deltaTime*20f);
			parentText.localPosition = Vector3.MoveTowards(parentText.localPosition,
				new Vector3(parentText.localPosition.x, parentText.localPosition.y, -10f), Time.deltaTime*20f);
			parentText.GetChild(0).localPosition = Vector3.MoveTowards(parentText.GetChild(0).localPosition,
				new Vector3(parentText.GetChild(0).localPosition.x, parentText.GetChild(0).localPosition.y, 15f), Time.deltaTime*40f);
			parentText.GetChild(1).localPosition = Vector3.MoveTowards(parentText.GetChild(1).localPosition,
				new Vector3(parentText.GetChild(1).localPosition.x, parentText.GetChild(1).localPosition.y, 30f), Time.deltaTime*40f);
		} else {
			GetComponent<Image>().color = Color.gray;
			transform.localPosition = Vector3.MoveTowards(transform.localPosition,
				new Vector3(transform.localPosition.x, transform.localPosition.y, 5f), Time.deltaTime*20f);
			parentText.localPosition = Vector3.MoveTowards(parentText.localPosition,
				new Vector3(parentText.localPosition.x, parentText.localPosition.y, 0f), Time.deltaTime*20f);
			parentText.GetChild(0).localPosition = Vector3.MoveTowards(parentText.GetChild(0).localPosition,
				new Vector3(parentText.GetChild(0).localPosition.x, parentText.GetChild(0).localPosition.y, 10f), Time.deltaTime*40f);
			parentText.GetChild(1).localPosition = Vector3.MoveTowards(parentText.GetChild(1).localPosition,
				new Vector3(parentText.GetChild(1).localPosition.x, parentText.GetChild(1).localPosition.y, 20f), Time.deltaTime*40f);
			
		}
	}

	public override void OnSelect (UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnSelect (eventData);
		isSelected = true;
		parentText.GetChild(0).GetComponent<Text>().CrossFadeAlpha(1f,0.5f, false);
		parentText.GetChild(1).GetComponent<Text>().CrossFadeAlpha(1f,0.5f, false);
	}

	public override void OnDeselect (UnityEngine.EventSystems.BaseEventData eventData)
	{
		base.OnDeselect (eventData);
		isSelected = false;
		parentText.GetChild(0).GetComponent<Text>().CrossFadeAlpha(0.25f,0.5f, false);
		parentText.GetChild(1).GetComponent<Text>().CrossFadeAlpha(0.25f,0.5f, false);
	}
}
