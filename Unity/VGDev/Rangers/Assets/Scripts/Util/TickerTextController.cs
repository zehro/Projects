using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TickerTextController : MonoBehaviour 
{

	private static string newsTicker = "|News| World News: The government has banned Twitter due to a miscommunication" +
		" on Twitter that implied that Twitter CEO Alex Huynh is a Neo-Communist. # |Sports| Rangers: The Fire Ferrets" +
		" recently took on the much better funded Wet Weasels in the most unexpectedly boring match of the Doubles season; the" +
		" game ended in a disappointing tie. # |Sports| Football: Yet again, there is no football this year. No one cares about" +
		" football at all anymore. # |News| Entertainment: Will Smith died yesterday, after living to the age of 230 by drinking" +
		" nothing but Pepsi, which is notably one of his corporate sponsors. # |Entertainment| Music: Kelly Clarkson has returned" +
		" from the dead, upsetting many long-time Kelly Clarkson fans. # |News| World News: Russia 2 has declared war on Earth 3," +
		" effectively ending the 10-year cabbage embargo. Martians are very concerned about their cabbages, as we all know that" +
		" cabbages are an important aspect of the Martian economy." +
		" # |News| Agriculture: There have been reports of protests outside of the Russia 2 embassy at Slushyville, the provisional" +
		" world capital after the recent nuclear war, by newly sentient cabbages demanding the Russian2s to bring back the cabbage" +
		" embargo. # |News| Kids These Days: Local child Jimmy Phlegembateater gives an update on the latest trends kids have been" +
		" adopting, including taking selfies with their dogs, wearing clown noses to school, and showering in squirrel blood as part" +
		" of a widely popular ritual on Instagram to summon Cthulhu, our savior and destroyer who is currently slumbering in his house" +
		" at R'lyeh and waiting, dreaming...      ph'nglui     mglw'nafh     Cthulhu     R'lyeh     wgah'nagl      fhtagn";


	private static string[] separatedNews;
	private Text tickerText1, tickerText2;

	private string nextString;

	//private float maxMovingTimer = 0.5f;
	//private float movingTimer;

	// Use this for initialization
	void Start () 
	{
		//movingTimer = maxMovingTimer;
		tickerText1 = transform.GetChild(0).GetComponent<Text>();
		tickerText2 = transform.GetChild(1).GetComponent<Text>();
		separatedNews = newsTicker.Split('#');
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (nextString == null) 
		{
			nextString = separatedNews[(int)(Random.value*separatedNews.Length)];
		} 
		else 
		{
			tickerText1.transform.Translate(Vector3.left*Time.deltaTime*100f);
			tickerText2.transform.Translate(Vector3.left*Time.deltaTime*100f);
			if(tickerText1.rectTransform.anchoredPosition.x < -tickerText1.rectTransform.sizeDelta.x) 
			{
				tickerText1.text = nextString;
				tickerText1.rectTransform.sizeDelta = new Vector2(tickerText1.text.Length*13, tickerText1.rectTransform.sizeDelta.y);
				nextString = null;
				tickerText1.rectTransform.anchoredPosition = new Vector2(tickerText2.rectTransform.anchoredPosition.x + tickerText2.rectTransform.sizeDelta.x, tickerText1.rectTransform.anchoredPosition.y);
			}
			if (tickerText2.rectTransform.anchoredPosition.x < -tickerText2.rectTransform.sizeDelta.x) 
			{
				tickerText2.text = nextString;
				tickerText2.rectTransform.sizeDelta = new Vector2(tickerText2.text.Length*13, tickerText2.rectTransform.sizeDelta.y);
				nextString = null;
				tickerText2.rectTransform.anchoredPosition = new Vector2(tickerText1.rectTransform.anchoredPosition.x + tickerText1.rectTransform.sizeDelta.x, tickerText2.rectTransform.anchoredPosition.y);
			}
		}
	}
}
