package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * A laser-firing satellite.
	 * 
	 * Flies in a curving pattern and fires 'lasers' (bullets) every now and then.
	 * Hard to hit, but fragile.
	 * 
	 * @author Alexander Huynh
	 */
	public class Vehicle_Satellite extends ABST_Vehicle 
	{
		private var timer:Number = 0;
		
		private var dirX:int;
		private var dirY:int;
		private const ACCELY:Number = .15;
		private const ACCELX:Number = .05;

		private const SPEED_LIMIT:Number = 2;
		private const LIMIT_DN:int = -100;
		private const LIMIT_UP:int = -200;
		private const LIMIT_RI:int = 250;
		private const LIMIT_LE:int = -250;
		
		public function Vehicle_Satellite(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{			
			super(_cg, _mc, _origin, _target, _type, params);
			dx = 2;
			dy = 1;

			dirX = 1;
			dirY = 1;
			
			hp = hpMax = 3;
			money = 750;
		}
		
		override public function step():Boolean
		{
			if (!markedForDestroy)
			{
				// calculate and perform movement
				if (dy > 0 && mc.y > LIMIT_DN)
					dirY = -1;
				else if (dy < 0 && mc.y < LIMIT_UP)
					dirY = 1;
				dy += ACCELY * dirY;
				if (dy > SPEED_LIMIT)
					dy = SPEED_LIMIT;
				else if (dy < -SPEED_LIMIT)
					dy = -SPEED_LIMIT;
					
				if (dx > 0 && mc.x > LIMIT_RI)
					dirX = -1;
				else if (dx < 0 && mc.x < LIMIT_LE)
					dirX = 1;
				dx += ACCELX * dirX;
				if (dx > SPEED_LIMIT)
					dx = SPEED_LIMIT;
				else if (dx < -SPEED_LIMIT)
					dx = -SPEED_LIMIT;
					
				mc.x += dx;
				mc.y += dy;

				if (mc.x > -150 && timer == 150)
				{
					timer = 0;
					cg.manBull.spawnProjectile("chain", new Point(mc.x, mc.y), new Point(cg.game.city.x, cg.game.city.y));
				}
			}
			
			timer++;
			
			// returns TRUE if this object needs to be removed
			return readyToDestroy;
		}
	}
}