using UnityEngine;
using System.Collections;

/// <summary>
/// Perlin shake.
/// </summary>
/// <description>
/// Camera script grabbed from this blog: http://unitytipsandtricks.blogspot.com/2013/05/camera-shake.html 
/// with minor changes
/// </description>
public class PerlinShake : MonoBehaviour {
	
	public float duration = 0.3f;
	public float speed = 1.0f;
	public float magnitude = 0.07f;
	
	public bool test = false;
	
	// -------------------------------------------------------------------------
	public void PlayShake() {
		
		StopAllCoroutines();
		StartCoroutine("Shake");
	}
	
	// -------------------------------------------------------------------------
	void Update() {
		if (test) {
			test = false;
			PlayShake();
		}
	}
	
	// -------------------------------------------------------------------------
	IEnumerator Shake() {
		
		float elapsed = 0.0f;
		float randomStart = Random.Range(-1000.0f, 1000.0f);
		
		while (elapsed < duration) {
			
			elapsed += Time.deltaTime;			
			
			float percentComplete = elapsed / duration;			
			
			// We want to reduce the shake from full power to 0 starting half way through
			float damper = 1.0f - Mathf.Clamp(2.0f * percentComplete - 1.0f, 0.0f, 1.0f);
			
			// Calculate the noise parameter starting randomly and going as fast as speed allows
			float alpha = randomStart + speed * percentComplete;
			
			// map noise to [-1, 1]
			float x = Util.Noise.GetNoise(alpha, 0.0f, 0.0f) * 2.0f - 1.0f;
			float y = Util.Noise.GetNoise(0.0f, alpha, 0.0f) * 2.0f - 1.0f;
			
			x *= magnitude * damper;
			y *= magnitude * damper;

			Vector3 targetPos = Camera.main.GetComponent<CameraFollow>().TargetPos;
			
			Camera.main.transform.position = new Vector3(x + Camera.main.transform.position.x, y + Camera.main.transform.position.y, Camera.main.transform.position.z);
				
			yield return null;
		}
		
		Camera.main.transform.position = Camera.main.GetComponent<CameraFollow>().TargetPos;
	}
}
