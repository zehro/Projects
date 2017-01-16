package 
{
	public class ContainerOutro extends Container
	{
		public var winState:String;
		
		public function ContainerOutro(ws:String)
		{
			super();
			winState = ws;
			if (winState == "quit")
				completed = true;
			else
				gotoAndPlay(winState);
		}
		
		override public function step():Boolean
		{
			return completed;
		}
	}
}
