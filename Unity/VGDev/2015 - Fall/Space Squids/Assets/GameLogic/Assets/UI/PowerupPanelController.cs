using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerupPanelController : MonoBehaviour
{
	public GameObject squidShip;
	public Sprite powerup1;
	public Sprite powerup2;
	public Sprite powerup3;
	SquidController squid;
	RectTransform panelTransform;
	Image panelImage;

	int lastPhase = 0;
	float scale = 0;
	float scaleVel = 0;
	float scaleVelForce = 0.075F;
	float scaleVelDrag = 0.925F;

	float amount = 0;
	float amountDrag = 3;

	void Awake()
	{
		squid = squidShip.GetComponent<SquidController>();
		panelTransform = GetComponent<RectTransform>();
		panelImage = transform.GetChild(0).GetComponent<Image>();
	}
	
	void Update()
	{
		float scaleTarg = (Mathf.Abs(1.5F-squid.getPowerupPhase()) < 1 ? 1 : 0);
		scaleVel += (scaleTarg-scale)*scaleVelForce;
		scaleVel *= scaleVelDrag;
		scale += scaleVel;
		if (scale < 0)
			scale = 0;

		if (lastPhase != squid.getPowerupPhase() && lastPhase == 1)
			scale = 1.5F;
		lastPhase = squid.getPowerupPhase();
		if (lastPhase == 2 && (Time.time % 1) < 0.05)
			scale *= 1.05F;
		panelTransform.localScale = new Vector3(scale, scale, 1);

		int image = squid.getPowerup();
		if (squid.getPowerupPhase() == 1)
			image = Mathf.FloorToInt(Time.time*20) % 3;
		if (image == 0)
			panelImage.sprite = powerup1;
		if (image == 1)
			panelImage.sprite = powerup2;
		if (image == 2)
			panelImage.sprite = powerup3;
		amount += (squid.getPowerupAmount()-amount)/amountDrag;
		panelImage.fillAmount = amount;
	}
}