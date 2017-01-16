package deltabattery.projectiles 
{
	import cobaltric.ContainerGame;
	import flash.display.MovieClip;
	import flash.geom.Point;
	
	/**
	 *	A cluster missile.
	 * 
	 *	After a set amount of time, splits into three small and fast rockets.
	 * 
	 *	@author Alexander Huynh
	 */
	public class Missile_Cluster extends ABST_Missile 
	{
		private var airburst:int;				// time to burst
		private var warning:MovieClip;			// visual warning
		private var rocketParams:Object;
		
		public function Missile_Cluster(_cg:ContainerGame, _mc:MovieClip, _origin:Point, _target:Point, _type:int=0, params:Object=null) 
		{
			if (!params)
			{
				params = new Object();
				params["velocity"] = 2;
				params["partInterval"] = 10;
			}
			else
			{
				if (!params["velocity"])
					params["velocity"] = 2;
				params["partInterval"] = 10;
			}

			rocketParams = new Object();
			rocketParams["velocity"] = 6;
			rocketParams["money"] = 50;
			rocketParams["damage"] = 4;

			super(_cg, _mc, _origin, _target, _type, params);
			airburst = 150 + getRand(0, 90);
			
			money = 400;
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

				// overridden code
				if (--airburst == 0)
				{
					// spawn rockets
					createExplosion = false;
					cg.manMiss.spawnProjectile("rocket", new Point(mc.x, mc.y), new Point(target.x, target.y), type, rocketParams);
					cg.manMiss.spawnProjectile("rocket", new Point(mc.x, mc.y), new Point(target.x + 30, target.y - 30), type, rocketParams);
					cg.manMiss.spawnProjectile("rocket", new Point(mc.x, mc.y), new Point(target.x - 30, target.y + 30), type, rocketParams);
					for (var i:int = 0; i < 3 + getRand(0, 3); i++)
						cg.manPart.spawnParticle(partType, new Point(mc.x, mc.y), 0, dx * .1, dy * .10, .05);
					awardMoney = false;
					destroy();
				}
				// warning
				else if (airburst == 45)
				{
					warning = new SpecialWarning();
					mc.addChild(warning);
				}
				else if (warning)
				{
					warning.gotoAndStop(warning.currentFrame + 1);
					warning.rotation += (airburst < 10 ? 0 : 4);
				}
			}
			
			return readyToDestroy;
		}
	}
}