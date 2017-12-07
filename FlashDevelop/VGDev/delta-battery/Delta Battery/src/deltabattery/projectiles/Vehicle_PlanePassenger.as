package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * An unarmed passenger plane.
	 * Don't accidentally shoot it.
	 * 
	 * Awards money if it escapes; otherwise, deducts money.
	 * 
	 * @author Alexander Huynh
	 */
	public class Vehicle_PlanePassenger extends ABST_Vehicle 
	{
		public function Vehicle_PlanePassenger(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			if (!_target || !_origin)		// dem random crashes though
			{
				if (_mc)
					_mc.visible = false;
				destroy();
			}
			else
			{
			
				_target.y = _origin.y;

				if (!params)
					params = { velocity:(1 + getRand(0, 2)) };
				else
					params["velocity"] = 1 + getRand(0, 2);
				
				super(_cg, _mc, _origin, _target, _type, params);
				
				typeSmart = 1;		// teach DELTA STRIKE to ignore this
				money = -500;
				hp = hpMax = 13;
			}
		}
		
		override public function step():Boolean
		{
			if (!markedForDestroy)
			{
				// calculate and perform movement
				mc.x -= velocity;
				
				// destroy if out of bounds12
				if (mc.x < -450)
				{
					money *= -1;		// earn money instead of losing it
					destroy();
				}
			}
			
			// returns TRUE if this object needs to be removed
			return readyToDestroy;
		}
	}
}