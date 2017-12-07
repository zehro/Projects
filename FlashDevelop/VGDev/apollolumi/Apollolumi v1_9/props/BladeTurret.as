// shoves things that are nearby away

package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.StructureManager;
	import managers.Manager;

	public class BladeTurret extends Structure
	{	
		private var dmg:int = 25;
	
		public function BladeTurret(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Energy Blade Turret";
			mass = 7;
			useLargestHitbox = true;
			range = 0;
			
			widthSP = heightSP = 85;		// override automatic setting
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (hp <= 0) return true;
			
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;

			rotation -= 13;
			return hp <= 0;
		}
		
		override public function collide(f:Floater):void
		{
			if (f.ID != "structure")
			{
				cg.changeHP(-dmg, f);
				cg.changeHP(-2, this);
				cg.sndMan.playSound("impact1");
			}
			else
			{
				x -= dX * 3;
				y -= dY * 3;
			}
			//trace("hit (EBT): " + f);
			handleCollision(this, f);
		}
	}
}
