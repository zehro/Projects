package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * Bomber thingamajigger
	 * 
	 * @author Jesse Chen
	 */
	public class Vehicle_Bomber extends ABST_Vehicle 
	{
		private var timer:Number = 0;
		
		public function Vehicle_Bomber(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int = 0, params:Object = null)
		{
			_target.y = _origin.y;
			
			if (!params)
				params = { velocity:(2 + Math.random()) };
			else
				params["velocity"] = 2 + Math.random();
			
			super(_cg, _mc, _origin, _target, _type, params);
			
			money = 750;
		}
		
		override public function step():Boolean
		{
			if (!markedForDestroy)
			{
				// calculate and perform movement
				mc.x += velocity;

				if (mc.x < 380 && timer % 90 == 0)
				{
					var params:Object = { velocity: velocity };
					cg.manMiss.spawnProjectile("bomb", new Point(mc.x, mc.y + 10), new Point(cg.game.city.x, cg.game.city.y), 0, params);
				}
				
				if ((Math.abs(mc.x) > 800 || mc.y > 370))
					destroy();
			}
			
			timer++;
			
			// returns TRUE if this object needs to be removed
			return readyToDestroy;
		}
	}

}