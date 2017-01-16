package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;

	public class Missile extends Flier
	{
		public var tgt:MovieClip;
		public var life:int;
		public var hitRange:Number;
		
		public function Missile(man:Manager, _xP:int, _yP:int, _rot:Number, _tgt:MovieClip)
		{
			super(man, _xP, _yP, 0, 0);
			rotation = facing = _rot;
			tgt = _tgt;
						
			filters = [glowR];
			
			rotSpd = 2;
			moveSpd = 2.5;
			
			life = 320;
			
			trailLength = 10;
			trailSaturation = 8;
			trailAlphaDelta = .1;
			
			trailColor = 0xEE422D;
			trailGlow = glowR;
			
			hitRange = 4;
			
			tgt.locks++;
			tgt.ui_warning.visible = true;

			manager.cg.gameMan.bonusLaunched = false;
		}
		
		override public function childStep():Boolean
		{
			if (--life == 0 || !tgt || !tgt.isAlive)
			{
				 clearTrail();
				 destroy();
				 return true;
			}
				
			if (tgt)
			{
				// update rotation
				var tgtAng:Number = getAngle(x, y, tgt.x, tgt.y);
				if (tgtAng > facing + 180) tgtAng -= 360;
				if (tgtAng < facing - 180) tgtAng += 360;
					
				// set to target angle if increment is too large
				if (Math.abs(tgtAng - facing) < rotSpd)
					facing = tgtAng;
				// otherwise turn towards target angle
				else
				{
					var rotInc:Number = (tgtAng - facing) * .2;// / rotSpd;
					// set speed limit
					if (Math.abs(rotInc) > rotSpd)					
						rotInc = (rotInc < 0 ? -1 : 1) * rotSpd;
					facing += rotInc;
				}
				rotation = facing;
				
				forward();
				
				if (getDist(this, tgt) < hitRange)
				{
					tgt.destroy();
				 	clearTrail();
					destroy();
				}
			}

			//base.rotation = facing;

			return life == 0;
		}
		
		override protected function childDestroy():void
		{
			life = 0;
			if (tgt && --tgt.locks == 0)
				tgt.ui_warning.visible = false;
			manager.cg.addExplosion(x, y, 90, .02, .5);
			manager.cg.eng.soundMan.playSound("SFX_explosionMissile");
		}
	}
}
