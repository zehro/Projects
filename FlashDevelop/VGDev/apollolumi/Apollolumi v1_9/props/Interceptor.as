package props
{
	import flash.display.MovieClip;
	import managers.StructureManager;
	import managers.ProjectileManager;
	import managers.Manager;
	import props.Projectile;
	import flash.geom.Point;

	public class Interceptor extends Structure
	{
		private var cool:int;
		private var rof:int = 18;
		private var blast:int;
		private var BLAST:int = 4;		// how many frames laser is visible
		
		private var tgt:Floater;		// what to shoot at
	
		public function Interceptor(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int)
		{
			super(_sm, _cg, _xP, _yP, _dX, _dY, _hp, _ts);
			ID = "structure";
			TITLE = "Interceptor";
			mass = 2;
			
			range = 700;
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
				{
					//if (!(m is ProjectileManager))
						//continue;
					for each (var mc:MovieClip in m.getVec())
					{
						if (!(mc is Floater) || mc == this) continue;
						f = mc as Floater;
						if (f.ID != "projectile") continue;
						var proj:Projectile = f as Projectile;
						if (proj.isFriendly) continue;
						var dist:Number = getDist(this, proj);
						if (dist < minDist)
						{
							minDist = dist;
							tgt = proj;
						}
					}
				}
			}
			if (tgt)
			{
				//trace("TGT! " + tgt);
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
						drawLine(1, 0x2DFF00, .7, tgt);
				}
				else if (cool == 0)				// if laser is ready to fire
				{
					tgt.destroy();
					blast = BLAST;
					drawLine(1, 0x2DFF00, .7, tgt);
					cg.sndMan.playSound("laser1");
					playAtk();
				}
			}

			rotation -= 1;
			gfx.rotation += 1;
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
			trace("hit (INT): " + f);
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
