package managers
{
	import flash.display.MovieClip;
	import managers.Manager;
	import props.Missile;
	import flash.geom.Point;

	public class MissileManager extends Manager
	{
		private var missVec:Vector.<MovieClip> = new Vector.<MovieClip>();
		private var warningTimer:int;

		public function MissileManager(_cg:MovieClip)
		{
			super(_cg);
		}
		
		override public function step(ld:Boolean, rd:Boolean, p:Point):void
		{
			//trace("MissileManager step.");
			
			var minDist:Number = 900, dist:Number = 900;
			var mc:MovieClip;
			if (missVec.length != 0 && warningTimer > 0)
			{
				if (--warningTimer == 0)
					cg.eng.soundMan.playSound("sfx_missileWarning");
			}

			var i:int;
			for (i = missVec.length - 1; i >= 0; i--)
			{
				mc = missVec[i];
				if (warningTimer == 0)
				{
					if (mc.tgt)
						dist = mc.getDist(mc, mc.tgt);
					if (dist < minDist)
						minDist = dist;
				}

				if (mc.step())
					missVec.splice(i, 1);
				else if (ld)
					mc.leftMouse();
				else if (rd)
					mc.rightMouse();
			}
			
			if (dist < 900 && warningTimer == 0)
				setDelay(minDist);
		}
		
		public function restart():void
		{
			for each (var m:Missile in missVec)
			{
				m.clearTrail();
				if (m.parent && m.parent.contains(m))
					m.parent.removeChild(m);
			}
			missVec = new Vector.<MovieClip>();
		}

		public function hasThreats():Boolean
		{
			return missVec.length > 0;
		}

		override public function getVec():Vector.<MovieClip>
		{
			return missVec;
		}

		private function setDelay(d:Number):void
		{
			warningTimer = d < 25 ? 8 : Math.ceil(.189 * d + 3.27);
			//trace(warningTimer);
		}
		
		public function spawnMissile(locX:int, locY:int, rot:Number, tgt:MovieClip):void
		{			
			cg.eng.soundMan.playSound("SFX_missileLaunch");
			var missile:Missile = new Missile(this, locX, locY, rot, tgt);
			cg.cont.addChild(missile);
			
			if (missVec.length == 0)
			{
				cg.eng.soundMan.playSound("sfx_missileWarning");
				setDelay(missile.getDist(missile, tgt));
			}
			else
			{
				var i:int;			
				var minDist:Number = 900, dist:Number = 900;
				var mc:MovieClip;
				for (i = missVec.length - 1; i >= 0; i--)
				{
					mc = missVec[i];
					if (mc.tgt)
						dist = mc.getDist(mc, mc.tgt);
					if (dist < minDist)
						minDist = dist;
				}
				setDelay(minDist);
			}
			
			missVec.push(missile);
		}
		
		override public function destroy():void
		{
			for (var i:int = missVec.length - 1; i >= 0; i--)
				missVec[i].destroy();
			missVec = null;
		}
	}
}