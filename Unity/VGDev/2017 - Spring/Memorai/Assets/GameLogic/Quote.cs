using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//For showing a cutscene quote, creates a quote, and the time it remains on screen
public class Quote  {
    string mainLine = "";
    float runTime = 4.0f;
    
	public Quote(string line, float time) {
        mainLine = line;
        runTime = time;
    }

    public Quote(string line) {
        mainLine = line;
    }

    public string getLine() {
        return mainLine;
    }

    public float getTime() {
        return runTime;
    }
}
