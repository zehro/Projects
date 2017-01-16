package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 * Long-range Air-to-Surface Missile.
	 * 
	 * Travels horizontally until over its target, then dives straight down.
	 * 
	 * @author Alexander Huynh
	 */
	public class Missile_LASM extends ABST_Missile 
	{
		private var fall:Boolean = false;		// TRUE if heading down, FALSE if travelling horizontally
		private var originalY:Number;
		
		public function Missile_LASM(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			originalY = _target.y;
			_target.y = _origin.y;
			_target.x = 250 + getRand(50);		// TODO y u no work with passed-in target?
			
			super(_cg, _mc, _origin, _target, _type, params);
			
			money = 125;
			damage = 7 + getRand(0, 2);
		}
		
		override public function step():Boolean
		{
			if (!markedForDestroy)
			{
				// calculate and perform movement
				var dx:Number = velocity * Math.cos(rot);
				var dy:Number = velocity * Math.sin(rot);
				
				mc.x += dx;
				mc.y += dy;
				
				updateParticle(dx, dy);
				checkTarget();
				
				if (fall && mc.rotation < 90)
				{
					mc.rotation += 10;
					rot = degreesToRadians(mc.rotation);
				}

				dist = getDistance(mc.x, mc.y, target.x, target.y);

				if ((Math.abs(mc.x) > 800 || dist < 5 || dist > prevDist || mc.y > 370))
				{
					if (mc.y > 370)
					{
						destroy();
						return readyToDestroy;
					}
					if (!fall)
					{
						fall = true;
						velocity += 2;
						target.y = originalY;
					}
				}
				else
					prevDist = dist;
			}
			
			// returns TRUE if this object needs to be removed
			return readyToDestroy;
		}
	}
}