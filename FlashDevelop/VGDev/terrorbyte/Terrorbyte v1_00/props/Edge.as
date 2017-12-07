package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;

	public class Edge extends MovieClip
	{
		private var node1:Node;
		private var node2:Node;
		private var connFX:MovieClip;
		
		public var connections:int;
		public var nodeTgt:int = 0;
		private var counter:int = -1;
		
		public function Edge(n1:Node, n2:Node, cfx:MovieClip)
		{
			node1 = n1;
			node2 = n2;
			
			connFX = cfx;
			
			/*connFX = new UIConnecting();
			
			connFX.x = (n1x() + n2x()) * .5;
			connFX.y = (n1y() + n2y()) * .5;
			trace(x + " " + y);
			trace(connFX.x + " " + connFX.y);
			addChild(connFX);*/
			connFX.visible = false;
			connFX.stop();
		}
		
		public function updateTime(tick:int):void
		{
			if (counter == -1 || nodeTgt == 0)
				return;
			counter -= tick;
			
			if (counter <= 0)
			{
				counter = -1;
				/*if (nodeTgt == 1)
					node1.recieveTb();
				else
					node2.recieveTb();*/
				nodeTgt = 0;
				connFX.visible = false;
				connFX.stop();
			}
		}
		
		public function sendTb(n:Node, time:int):void
		{
			n.beingHacked = true;
			if (node1 == n)
				nodeTgt = 1;
			else
				nodeTgt = 2;
			counter = time;
			connFX.visible = true;
			connFX.gotoAndPlay(1);
		}
		
		public function getNode1():Node
		{
			return node1;
		}
		
		public function getNode2():Node
		{
			return node2;
		}
		
		public function n1():String
		{
			return node1.id;
		}
		
		public function n2():String
		{
			return node2.id;
		}
		
		public function n1x():int
		{
			return node1.x;
		}
		
		public function n1y():int
		{
			return node1.y;
		}
		
		public function n2x():int
		{
			return node2.x;
		}
		
		public function n2y():int
		{
			return node2.y;
		}
	}
}
