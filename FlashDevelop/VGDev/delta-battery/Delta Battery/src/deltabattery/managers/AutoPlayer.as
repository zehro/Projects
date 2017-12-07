package deltabattery.managers 
{
	import cobaltric.ContainerGame;
	import deltabattery.projectiles.ABST_Missile;
	import flash.display.MovieClip;
	import flash.events.MouseEvent;
	
	/**	An AI program that controls the turret automatically
	 * 
	 *@author Alexander Huynh
	 */
	public class AutoPlayer extends ABST_Manager 
	{
		private var manMiss:ManagerMissile;
		private var numOnTgt:Object = new Object();
	
		private var turret:MovieClip;
		
		private var cooldown:int;

		public function AutoPlayer(_cg:ContainerGame, _manMiss:ManagerMissile) 
		{
			super(_cg);
			manMiss = _manMiss;
			
			turret = cg.game.mc_turret;
		}

		override public function step():void
		{	
			if (cooldown > 0)
			{
				cooldown--;
				return;
			}
			
			var letgo:Boolean = true;
			
			var missile:ABST_Missile;
			for (var i:int = manMiss.objArr.length - 1; i >= 0; i--)
			{
				missile = manMiss.objArr[i];
				if (!numOnTgt[missile.origin])
				{
					//trace("Found a new missile");
					numOnTgt[missile.origin] = 0;
				}
				if (missile.readyToDestroy)
				{
					//trace("Removed a missile");
					numOnTgt[missile.origin] = null;
					continue;
				}
				
				var distToBase:Number = getDistance(turret.x, turret.y, missile.mc.x, missile.mc.y);
					
				if (distToBase > 700) continue;
				
				var mVelo:Number = missile.velocity;
				var mRot:Number = missile.rot;
				
				var scaleFactor:Number = 70 * (distToBase / 850);
				
				var tgtX:Number = missile.mc.x + mVelo * scaleFactor * Math.cos(mRot);
				var tgtY:Number = missile.mc.y + mVelo * scaleFactor * Math.sin(mRot);
				
				cg.overrideMouse(tgtX, tgtY);
				
				if (distToBase < 110 && !missile.readyToDestroy)
				{
					letgo = false;
					cg.dispatchEvent(new MouseEvent(MouseEvent.RIGHT_MOUSE_DOWN, true, true, tgtX, tgtY));
				}
					
				//trace("TRACKING: " + Math.round(missile.mc.x) + "," + Math.round(missile.mc.y) + "\tFired: " + numOnTgt[missile.origin]);
				if (cooldown == 0 && numOnTgt[missile.origin] == 0)
				{
					trace("*FIRE* Target: " + Math.round(tgtX) + "," + Math.round(tgtY) + "\tSF: " + scaleFactor);

					cg.dispatchEvent(new MouseEvent(MouseEvent.MOUSE_DOWN, true, true, tgtX, tgtY));
					
					numOnTgt[missile.origin]++;
					cooldown = 15;
				}
			}
			
			if (letgo)
				cg.dispatchEvent(new MouseEvent(MouseEvent.RIGHT_MOUSE_UP, true, true, tgtX, tgtY));
		}
	}
}