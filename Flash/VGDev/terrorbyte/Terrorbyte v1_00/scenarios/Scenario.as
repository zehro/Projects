package scenarios
{
	import flash.geom.Point;
	import flash.display.MovieClip;

	public class Scenario
	{
		protected var cg:ContainerGame;
		protected var turret:MovieClip;
		
		public function Scenario()
		{
		}
		
		public function setCG(_cg:ContainerGame)
		{
			cg = _cg;
		}
		
		public function loadScenario():void
		{
			// -- override this function
		}
		
		protected function setBonusTime(n:Number):void
		{
			cg.gameMan.bonusTime = n;
		}
		
		protected function makeWaypoints(arr:Array):Vector.<Point>
		{
			var wp:Vector.<Point> = new Vector.<Point>();
			for (var i:uint = 0; i < arr.length; i += 2)
				wp.push(new Point(arr[i], arr[i+1]));
			return wp;
		}
	}
}