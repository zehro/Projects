using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class PrismEffect : ImageEffectBase {

	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		Graphics.Blit (source, destination, material);
	}
}
