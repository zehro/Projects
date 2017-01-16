using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;
using Assets.Scripts.Data;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour 
{

	private Image healthBar, followBar, strengthBar;
	private Controller playerRef;

	/// <summary> The panel displaying the player's health. </summary>
	private Transform healthPanel;
	/// <summary> The panel displaying the player's arrow strength. </summary>
	private Transform strengthPanel;

	// Use this for initialization
	void Start () 
	{
		playerRef = transform.root.GetComponent<Controller>();
		healthPanel = transform.FindChild("HealthPanel");
		strengthPanel = transform.FindChild("StrengthPanel");
		healthBar = healthPanel.FindChild("Overlay").GetComponent<Image>();
		followBar = healthPanel.FindChild("Follow").GetComponent<Image>();
		strengthBar = strengthPanel.FindChild("Overlay").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float health = playerRef.LifeComponent.HealthPercentage;
//		float strength = playerRef.ArcheryComponent.StrengthPercentage;

		healthBar.fillAmount = health;
		followBar.fillAmount = Mathf.MoveTowards(followBar.fillAmount, healthBar.fillAmount, Time.deltaTime/2f);
//		strengthBar.rectTransform.localScale = new Vector3(-strength, strength, strength);

		if(playerRef.ArcheryComponent.UpperBodyFacingRight && !Mathf.Approximately(transform.localEulerAngles.y,270f))
		{
			GetComponent<RectTransform>().localRotation = Quaternion.RotateTowards(GetComponent<RectTransform>().localRotation,
				Quaternion.Euler(new Vector3(0f,270f,0f)), Time.deltaTime*720f);
		} 
		else if(!playerRef.ArcheryComponent.UpperBodyFacingRight && !Mathf.Approximately(transform.localEulerAngles.y,90f)) 
		{
			GetComponent<RectTransform>().localRotation = Quaternion.RotateTowards(GetComponent<RectTransform>().localRotation,
				Quaternion.Euler(new Vector3(0f,90f,0f)), Time.deltaTime*720f);
		}
		bool alive = playerRef.LifeComponent.Health > 0;
		healthPanel.gameObject.SetActive(alive);
		strengthPanel.gameObject.SetActive(alive);
	}
}
