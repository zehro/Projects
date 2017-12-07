using UnityEngine;
using DG.Tweening;

/// <summary>
/// Wrapper for the DOShake method from DOTween applied on an object
/// </summary>
public class ShakeObject : MonoBehaviour {
    public float length;
    public float shakeAmount;
    public int vibration;
    public float randomness;

    public void Shake()
    {
        Shake(new Vector3(shakeAmount, shakeAmount, shakeAmount));
    }

    public void Shake(Vector3 direction)
    {
        transform.DOShakePosition(length, direction * shakeAmount, vibration, randomness);
        transform.DOShakeRotation(length, direction * shakeAmount * 10, vibration, randomness);
    }
}
