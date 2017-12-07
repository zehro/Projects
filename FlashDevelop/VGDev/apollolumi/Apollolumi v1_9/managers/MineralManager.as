package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Mineral;

	public class MineralManager extends Manager
	{
		public var minVec:Vector.<MovieClip> = new Vector.<MovieClip>();

		public function MineralManager(_par:MovieClip)
		{
			super(_par);
		}
		
		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			if (minVec.length == 0) return;
			for (i = minVec.length - 1; i >= 0; i--)
				if (minVec[i].step(colVec, p))
					minVec.splice(i, 1);
				else if (rd)
					minVec[i].rightMouse();
		}
		
		public function spawnMineral(xLoc:Number, yLoc:Number, type:int, impurities:Number = 0):void
		{
			var min:Mineral = new Mineral(this, par, xLoc + getRand(-30, 30), yLoc + getRand(-30, 30), getRand(-3, 3), getRand(-3, 3), 100, type, impurities);
			par.game.cont.addChild(min);
			minVec.push(min);
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return minVec;
		}
		
		override public function destroy():void
		{
			for each (var m:Mineral in minVec)
			{
				m.destroy();
				if (par.game.cont.contains(m))
					par.game.cont.removeChild(m);
			}
			minVec = new Vector.<MovieClip>();
		}
	}
}
