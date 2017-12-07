using UnityEngine;

public class Timer : MonoBehaviour {

    public float timeRemaining = 10.0f;
    private bool isStillCountingDown = false;
    bool paused;

	// Use this for initialization
	void Start () {
        if (!isStillCountingDown)
        {
            isStillCountingDown = true;
            Invoke("tick", 1f);
        }
	}
	
    public void setTimeRemaining(float aFloat)
    {
        timeRemaining = aFloat;
    }
    
    public float getTimeRemaining ()
    {
        return timeRemaining;
    }

    public bool isTimeRemaining()
    {
        return timeRemaining < 0;
    }

    private void tick()
    {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0)
        {
            isStillCountingDown = false;
            CancelInvoke("tick");
        }
    }

    private void stopTimer()
    {
        paused = !paused;
    }

	// Update is called once per frame
	void Update () {
        if(isStillCountingDown && !paused)
        {
            tick();
        }
	}
}
