package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.EnemyManager;
	import managers.Manager;

	public class Enemy extends Floater
	{		
		protected var manager:EnemyManager;
		
		public var moveSpd:Number = .2;		// acceleration per frame
		public var minSpd:Number = .1;		// if speed is lower than this, speed becomes 0
		public var maxSpd:Number = 3;		// go no faster than this
		public var turnSpd:Number = 4;		// rotation in degrees per frame
		public var friction:Number = .97;
		
		// 0 - beam; 1 - ballistic; 2 - explosion
		public var armor:Vector.<Number> = new Vector.<Number>();
		public var dmg:Number;
		public var rof:int;
		public var cooldown:int;
		public var range:int;

		public var tgt:Floater;		// target of interest
	
		public function Enemy(_em:EnemyManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int = 0)
		{
			super(_cg, _xP, _yP, _dX, _dY, _hp, _ts);
			manager = _em;
			setMapIcon(0xFF0000, "d");
			collideWithSameType = false;
		}
				
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// check for min speed and handle friction
			if (abs(dX) > maxSpd)
				dX = maxSpd * (dX > 0 ? 1 : -1);
			else
				abs(dX) < minSpd ? dX = 0 : dX *= friction;
			if (abs(dY) > maxSpd)
				dY = maxSpd * (dY > 0 ? 1 : -1);
			else
				abs(dY) < minSpd ? dY = 0 : dY *= friction;
			return hp == 0;
		}
		
		protected function forward():void
		{
			dY += Math.sin(degreesToRadians(rotation)) * moveSpd;
			dX += Math.cos(degreesToRadians(rotation)) * moveSpd;
		}
		
		protected function reverse():void
		{
			dY -= Math.sin(degreesToRadians(rotation)) * moveSpd;
			dX -= Math.cos(degreesToRadians(rotation)) * moveSpd;
		}
	}
}
