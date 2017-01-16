package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Enemy;
	import managers.StructureManager;

	public class EnemyManager extends Manager
	{
		public var eneVec:Vector.<MovieClip> = new Vector.<MovieClip>();	// list of targets
		public var strMan:StructureManager;
		public var astMan:AsteroidManager;

		public function EnemyManager(_par:MovieClip, _st:StructureManager, _am:AsteroidManager)
		{
			super(_par);
			strMan = _st;
			astMan = _am;
		}
		
		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			if (eneVec.length == 0) return;
			for (i = eneVec.length - 1; i >= 0; i--)
				if (eneVec[i].step(colVec, p))
					eneVec.splice(i, 1);
				else if (ld)
					eneVec[i].leftMouse(p);
		}
		
		public function addEnemy(e:Enemy):void
		{
			par.game.cont.addChild(e);
			eneVec.push(e);
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return eneVec;
		}
		
		override public function destroy():void
		{
			for each (var e:Enemy in eneVec)
			{
				e.destroy();
				if (par.game.cont.contains(e))
					par.game.cont.removeChild(e);
			}
			eneVec = new Vector.<MovieClip>();
		}
	}
}
