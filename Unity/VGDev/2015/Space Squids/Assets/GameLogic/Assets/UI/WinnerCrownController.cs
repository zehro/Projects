using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinnerCrownController : MonoBehaviour
{
	Image crownImage;
	Image crownGlow;
	RectTransform crownImageTransform;
	RectTransform crownGlowTransform;
	AudioSource crownSound;
	int winner = -2;
	float scale = 1;

	float xRot = 0;
	float yRot = 0;
	float xRotVel = 0;
	float yRotVel = 0;
	float rotVelForce = 0.05F;
	float rotVelDrag = 0.95F;

	void Awake()
	{
		crownImage = GetComponent<Image>();
		crownGlow = transform.parent.Find("WinnerCrownGlow").gameObject.GetComponent<Image>();
		crownImageTransform = GetComponent<RectTransform>();
		crownGlowTransform = transform.parent.Find("WinnerCrownGlow").gameObject.GetComponent<RectTransform>();
		crownSound = GetComponent<AudioSource>();
	}

	void Start()
	{
		crownGlow.enabled = true;
	}

	public void changeWinner(int index)
	{
		// If the winner is new, do the effect

		if (index != winner)
		{
			winner = index;

			if (winner != -1)
			{
				crownSound.Play();
				scale = 2;
				xRot = Random.Range(-45,45);
				yRot = Random.Range(-45,45);
			}

			// Fade the crown to its new color

			float fadeTime = 0.1F;
			if (index == -1)
				crownImage.CrossFadeColor(new Color(0.5F, 0.5F, 0.5F, 1F), fadeTime, false, true);
			else if (index == 0)
				crownImage.CrossFadeColor(Color.red, fadeTime, false, true);
			else
				crownImage.CrossFadeColor(Color.blue, fadeTime, false, true);
		}
	}

	void FixedUpdate()
	{
		// Update rotation

		xRotVel += -xRot*rotVelForce;
		yRotVel += -yRot*rotVelForce;
		xRotVel *= rotVelDrag;
		yRotVel *= rotVelDrag;
		xRot += xRotVel;
		yRot += yRotVel;

		// Update the effects

		float alpha;
		scale += (1-scale)*0.1F;
		alpha = 1-Mathf.Pow(2-scale,32);
		crownImageTransform.sizeDelta = new Vector2((alpha+1)*128F, (alpha+1)*128F);
		crownImageTransform.rotation = Quaternion.Euler(xRot, yRot, (scale-1)*720F+xRot*0.2F);
		crownGlowTransform.sizeDelta = new Vector2(scale*384F, scale*384F);

		alpha = 1-Mathf.Pow(2-scale,16);
		alpha += Mathf.Sin(Time.time*30F)*alpha*0.1F;
		alpha = Mathf.Clamp01(alpha);
		crownGlow.color = new Color(1F, 1F, 1F, alpha);
	}
}