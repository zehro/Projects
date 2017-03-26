using UnityEngine;
using System.Collections;

/// <summary>
/// Shoots a laser forward.
/// </summary>
public class LazerTuneSpawn : Attack {

    /// <summary>
    /// Initializes the attack's fields based on critical hit status.
    /// </summary>
	protected override void Start () {
        destroyAfterTime = crit ? 1f : 0.1f;
        damage = crit ? .7f : 0.2f;
        base.Start();
	}

	void Update() {
		if (crit) {
			transform.Rotate (new Vector3 (0f, 5f * Time.deltaTime, 0f));
		}
	}
}
