// shoves things that are nearby away

package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.StructureManager;
	import managers.Manager;

	public class ForceDefense extends Structure
	{
		private var cool:int;
		private var rof:int = 60;
		//private var range:int = 250;
	
		public function ForceDefense(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Force Defense Turret";
			mass = 1.5;
			range = 250;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (hp <= 0) return true;
			
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			// if on cooldown, update cooldown timer
			if (cool > 0)
				cool--;
			// if cooldown is finished...
			else
			{
				// shove targets that are in range away from you
				var tgt:Boolean; var f:Floater = null;
				// look at all game objects
				for each (var m:Manager in colVec)				// -- change to "for" if optimization needed
					for each (var mc:MovieClip in m.getVec())
					{
						if (!(mc is Floater) || mc == this) continue;
						f = mc as Floater;
						if (f.ID == "structure" || f.ID == "projectile") continue;		// don't shove other structures or projectiles
						if (f.isTractorTgt) continue;			// don't shove anything the player is tractoring
						var dist:Number = getDist(this, f);		// get range to target
						if (dist < range)						// if target is in range
						{
							// check if it's even moving
							if (f.dX == 0 && f.dY == 0) continue;
							// check if it's moving towards you
							if (((f.x > x && f.dX < 0) || (f.x < x && f.dX > 0)) ||
								((f.y > y && f.dY < 0) || (f.y < y && f.dY > 0)))			
							{
								tgt = true;			// flag to play the flash FX later
								f.dX *= -.7;
								f.dY *= -.7;
								if (abs(f.dX) < .1) f.dX = 0;
								if (abs(f.dY) < .1) f.dY = 0;
							}
						}
					}
				if (tgt)		// play the flash FX and go on cooldown
				{
					cg.sndMan.playSound("pdl");
					cool = rof;
					fx.gotoAndPlay(2);
					playAtk();
				}
			}

			rotation -= .5;
			return hp <= 0;
		}
		
		override public function collide(f:Floater):void
		{
			if (f.ID != "structure")
			{
				cg.changeHP(-5, f);		// -- temporary	
				cg.changeHP(-5, this);		// -- temporary	
			}
			//trace("hit (FD)");
			handleCollision(this, f);
		}
	}
}
