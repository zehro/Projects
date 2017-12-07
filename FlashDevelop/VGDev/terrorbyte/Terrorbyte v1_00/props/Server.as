package props
{
	import flash.display.MovieClip;
	import managers.Manager;
	import managers.NodeManager;
	
	public class Server extends Node
	{
		public function Server(man:Manager, _nodeMan:NodeManager, _xP:int, _yP:int, id:String, _type:String, tb:int, resist:int, tier:int, perma:Boolean = false)
		{
			super(man, _nodeMan, _xP, _yP, id, tb, resist, tier);

			timeBox = hackTimer;
			infoBox = infobox;
			tbQueue = tbqueue;
			genericBox = genericbox;
			
			tfTb = tf_tb;
			tfResist = tf_resist;
			tfTier = tf_tier;
			
			//trace("Server about to call setup() ... " + id + " " + tb + " : " + tfTb.text);
			
			ring.visible = conquerable = perma;
			
			setup();
			//trace("\t\tI was placed with " + tb + " Terrorbytes.");
		}
		
		public function setAsHome():void
		{
			base.gotoAndStop("home");
			
			updateState(0, false, "hacking");
			setOffline(true);
			filters = [glowB];
			tfResist.visible = tfTier.visible = false;
		}
		
		/*override public function step():Boolean
		{
			if (!isRunning) return false;

			return childStep();
		}*/
	}
}
