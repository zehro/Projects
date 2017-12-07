// collects nearby minerals

package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.StructureManager;
	import managers.Manager;

	public class AutoCollector extends Structure
	{	
		public function AutoCollector(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Auto-Mineral Collector";
			mass = 1.25;
			
			range = 400;
			tractorSpeed = 4;
			friction = .97;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			handleTractor();
			
			rotation -= .5;
			gfx.rotation += .5;	// orientate tractor layer
			return hp == 0;
		}
		
		override public function collide(f:Floater):void
		{
			if (f.ID != "structure")
			{
				cg.changeHP(-5, f);
				cg.changeHP(-5, this);
			}
			trace("hit (AC): " + f);
			handleCollision(this, f);
		}
	}
}
