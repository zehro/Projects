package props
{
	import flash.display.MovieClip;
	import managers.StructureManager;
	import managers.Manager;
	import flash.geom.Point;

	public class PointDefenseLaser extends Structure
	{
		private var cool:int;
		private var rof:int = 10;
		private var blast:int;
		private var BLAST:int = 3;		// how many frames laser is visible
		private var dmg:int = 15;
		
		private var tgt:Floater;		// what to shoot at
	
		public function PointDefenseLaser(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Point Defense Laser";
			mass = 1.5;
			
			range = 375;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (hp <= 0) return true;
			
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			if (cool > 0)
				cool--;
			// acquire new target
			else if (!tgt)
			{
				blast = 1;
				gfx.graphics.clear();
				var minDist:Number = range;
				var f:Floater = null;
				for each (var m:Manager in colVec)				// -- change to "for" if optimization needed
					for each (var mc:MovieClip in m.getVec())
					{
						if (!(mc is Floater) || mc == this) continue;
						f = mc as Floater;
						if (f.ID == "structure" || f.ID == "asteroid" || f.ID == "projectile") continue;
						if (f.isTractorTgt) continue;
						var dist:Number = getDist(this, f);
						if (dist < minDist)
						{
							// check if it's even moving
							if (f.dX == 0 && f.dY == 0) continue;
							// check if it's moving towards you
							if (((f.x > x && f.dX < 0) || (f.x < x && f.dX > 0)) ||
								((f.y > y && f.dY < 0) || (f.y < y && f.dY > 0)))			
							{
								minDist = dist;
								tgt = f;
							}
						}
					}
			}
			if (tgt)
			{
				if (tgt.hp <= 0)			// if target is destroyed
				{
					gfx.graphics.clear();
					tgt = null;
					return hp == 0;
				}
				if (getDist(this, tgt) > range)	// check if target moved out of range
				{
					tgt = null;
					return hp == 0;
				}
				if (blast > 0)					// if not on cooldown and laser is firing
				{
					if (--blast == 0)			// if laser is finished firing
					{
						cool = rof;
						gfx.graphics.clear();
					}
					else						// if laser is still firing
						drawLine(1, 0xFF0000, .7, tgt);
				}
				else if (cool == 0)				// if laser is ready to fire
				{
					tgt.changeHP(-dmg);
					blast = BLAST;
					drawLine(1, 0xFF0000, .7, tgt);
					cg.sndMan.playSound("laser1");
					playAtk();
				}
			}

			rotation += 2;
			gfx.rotation -= 2;
			return hp == 0;
		}
		
		override public function resetTurret():void
		{
			drawLine(1, 0x000000, 0, this);
			tgt = null;
		}
	
		override public function collide(f:Floater):void
		{
			if (f.ID != "structure")
			{
				cg.changeHP(-5, f);		// -- temporary	
				cg.changeHP(-5, this);		// -- temporary	
			}
			trace("hit (PDL): " + f);
			handleCollision(this, f);
		}
		
		/*override public function destroy():void
		{
			hp = 0; visible = false;
			if (par.contains(this))
				par.removeChild(this);
		}*/
	}
}
