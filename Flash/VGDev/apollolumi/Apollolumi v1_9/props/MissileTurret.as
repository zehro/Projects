package props
{
	import flash.display.MovieClip;
	import managers.StructureManager;
	import managers.Manager;
	import flash.geom.Point;

	public class MissileTurret extends Structure
	{
		private var cool:int;
		private var rof:int = 150;
		private var dmg:int = 65;
		
		private var tgt:Floater;		// what to shoot at
		private var tgtDelay:int = 0;	// don't check for targets every frame
	
		public function MissileTurret(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Missile Turret";
			mass = 2;
			
			range = 900;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			if (hp <= 0) return true;
			
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			if (!tgt)
				top.rotation -= .5;
			
			if (cool > 0)
				cool--;
			// acquire new target
			else if (!tgt)
			{
				var maxDist:Number = 0;
				var f:Floater = null;
				for each (var m:Manager in colVec)				// -- change to "for" if optimization needed
					for each (var mc:MovieClip in m.getVec())
					{
						if (!(mc is Floater) || mc == this) continue;
						f = mc as Floater;
						if (f.ID == "structure" || f.ID == "asteroid" || f.ID == "projectile") continue;
						var dist:Number = getDist(this, f);
						if (dist <= range && dist > maxDist)
						{
							maxDist = dist;
							tgt = f;
						}
					}
			}
			if (tgt)
			{
				if (tgt.hp <= 0)			// if target is destroyed
					tgt = null;
				else if (getDist(this, tgt) > range)	// check if target moved out of range
					tgt = null;
				else if (cool == 0)				// if missile is ready to fire
				{
					cg.sndMan.playSound("launch3");
					top.rotation = getAngle(x, y, tgt.x, tgt.y);
					playAtk();
					cool = rof;
					cg.addProjectile(x, y, 3, top.rotation, "missile", 230, dmg, true, tgt);
				}
			}

			base.rotation += 2;
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
			trace("hit (MT): " + f);
			handleCollision(this, f);
		}
	}
}
