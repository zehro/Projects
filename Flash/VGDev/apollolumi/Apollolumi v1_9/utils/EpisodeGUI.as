package utils
{
	import flash.display.MovieClip;
	
	public class EpisodeGUI extends MovieClip
	{		
		public var entry:Object;
		
		public function EpisodeGUI(_entry:Object, id:String)
		{
			trace("\tNew episode recieved: " + _entry);
			if (_entry == null)
				newEntry(id);
			else
				entry = _entry;
			trace("\tFormed a: " + entry);
		}

		public function newEntry(id:String):void
		{
			entry = new Object();
			switch (id)
			{
				case "S-T":
					entry["s1"] = 10000;	entry["n1"] = "System";
					entry["s2"] = 8000;		entry["n2"] = "System";
					entry["s3"] = 6000;		entry["n3"] = "System";
					entry["s4"] = 4000;		entry["n4"] = "System";
					entry["s5"] = 2000;		entry["n5"] = "System";
				break;
				case "S-E":
					entry["s1"] = 5000;		entry["n1"] = "System";
					entry["s2"] = 4000;		entry["n2"] = "System";
					entry["s3"] = 3000;		entry["n3"] = "System";
					entry["s4"] = 2000;		entry["n4"] = "System";
					entry["s5"] = 1000;		entry["n5"] = "System";
				break;
				case "S-H":
					entry["s1"] = 20000;	entry["n1"] = "System";
					entry["s2"] = 16000;	entry["n2"] = "System";
					entry["s3"] = 12000;	entry["n3"] = "System";
					entry["s4"] = 8000;		entry["n4"] = "System";
					entry["s5"] = 4000;		entry["n5"] = "System";
				break;
				default:
					entry["s1"] = 0;	entry["n1"] = "System";
					entry["s2"] = 0;	entry["n2"] = "System";
					entry["s3"] = 0;	entry["n3"] = "System";
					entry["s4"] = 0;	entry["n4"] = "System";
					entry["s5"] = 0;	entry["n5"] = "System";
			}
		}
		
		public function feedData(c:MovieClip):void
		{
			//trace("recieved a " + c);
			c.board.tScore.text = entry["s1"].toString();	c.board.tName.text = entry["n1"];
			c.board.tScore2.text = entry["s2"].toString();	c.board.tName2.text = entry["n2"];
			c.board.tScore3.text = entry["s3"].toString();	c.board.tName3.text = entry["n3"];
			c.board.tScore4.text = entry["s4"].toString();	c.board.tName4.text = entry["n4"];
			c.board.tScore5.text = entry["s5"].toString();	c.board.tName5.text = entry["n5"];
		}
	}
}
