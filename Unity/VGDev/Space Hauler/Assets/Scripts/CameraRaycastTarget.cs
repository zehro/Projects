using UnityEngine;
using UnityEngine.Events;

public class CameraRaycastTarget : MonoBehaviour {

    public GameObject prompt;
    public UnityEvent events;

    public void Trigger()
    {
        events.Invoke();
        WheelPromptOff();
    }

    // Temporary (permanent) coupling
    void WheelPromptOff() {
        prompt.SetActive(false);
    }

}
