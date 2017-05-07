using UnityEngine;

public class Timer : MonoBehaviour {

    public float timeRemaining;
    public float defaultTimeRemaining = 10.0f;
    public bool isStillCountingDown;
    bool paused;

	// Use this for initialization
	void Start () {
        timeRemaining = defaultTimeRemaining;
        isStillCountingDown = true;
	}
	
    public void setTimeRemaining(float aFloat)
    {
        timeRemaining = aFloat;
    }

    public void setDefaultTimeRemaining(float aFloat)
    {
        defaultTimeRemaining = aFloat;
    }        
    
    public float getTimeRemaining ()
    {
        return timeRemaining;
    }

    public bool isTimeRemaining()
    {
        return isStillCountingDown;
    }

    public void Reset()
    {
        Reset(defaultTimeRemaining);
    }

    public void Reset(float newTime)
    {
        timeRemaining = newTime;
        isStillCountingDown = true;
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

    public void stopTimer()
    {
        paused = true;
    }

    public void startTimer() {
        paused = false;
    }

	// Update is called once per frame
	void Update () {
        if(isStillCountingDown && !paused)
        {
            tick();
        }
	}
}
