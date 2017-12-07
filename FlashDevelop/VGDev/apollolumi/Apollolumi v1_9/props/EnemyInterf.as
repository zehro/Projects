// By Neal Kaviratna

package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import managers.AsteroidManager;
	import managers.EnemyManager;
	import managers.Manager;

	public class EnemyInterf extends Enemy
	{		
		private var targetDelay:int = 30;		// only check for new target every second
	
		public function EnemyInterf(_em:EnemyManager, _cg:MovieClip, _xP:int, _yP:int, _hp:Number, _ts:int)
		{
			super(_em, _cg, _xP, _yP, 0, 0, _hp, _ts);
			spawnGrace = true;
			ID = "enemy";
			SPID = "interf";
			TITLE = "Enemy Ship";
			mass = .5;
			range = 10;
			dmg = 3;

			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			chooseTarget();
		}
		
		public function chooseTarget():void
		{
			var tgtVec:Vector.<MovieClip> = manager.astMan.getVec();
			if (tgtVec.length == 0) return;
			// find closest target
			var lowestDist:Number = getDist(this, tgtVec[0]);
			var dist:Number = 0;
			tgt = tgtVec[0] as Floater;
			var i:int, j:int = tgtVec.length;
			for (i = 0; i < j; i++)
			{
				dist = getDist(this, tgtVec[i]);
				if (dist < lowestDist)
				{
					tgt = tgtVec[i] as Floater;
					lowestDist = dist;
				}
			}
			rotation = getAngle(x, y, tgt.x, tgt.y);
			gfx.rotation = -rotation;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// handle velocity
			var dist:Number = getDist(this, tgt);
			if (dist < 100) //check how close to the target the Interferer is
			{
				if (dist < 70) // match the targets Speed when in range.
				{
					dX = tgt.dX - .2;
					dY = tgt.dY - .2;
					tgt.changeHP(-dmg);
				}
				else
				{	//slow down if this is approaching target
					if (abs(dX) > .2)
						dX += (dX < 0 ? .1 : -.1);
					if (abs(dY) > .2)
						dY += (dY < 0 ? .1 : -.1);
				}
			}
			if (abs(dX) > maxSpd)
				dX = maxSpd * (dX > 0 ? 1 : -1);
			else
				abs(dX) < minSpd ? dX = 0 : dX *= friction;
			if (abs(dY) > maxSpd)
				dY = maxSpd * (dY > 0 ? 1 : -1);
			else
				abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			if (!tgt || tgt.hp <= 0)
				chooseTarget();
			
			// keep targeting closest asteroid if target is too far away
			if (dist > 50)
				if (--targetDelay == 0)
				{
					targetDelay = 30;
					chooseTarget();
				}

			rotation = getAngle(x, y, tgt.x, tgt.y);
			gfx.rotation = -rotation;
			forward();

			return hp <= 0;
		}

		// handles when player lasers this object
		public function leftMouse(p:Point):void
		{
			if (!hitTestPoint(p.x, p.y)) return;

			hp -= cg.laserDmg;
			if (hp <= 0)
				destroy();
		}
		
		override public function collide(f:Floater):void
		{
			cg.changeHP(-8, f);
			//trace("hit (interf)");
			handleCollision(this, f);
		}
		
		override public function destroy():void
		{
			cg.addExplosion(x, y, 2);
			cg.sndMan.playSound("explode3");
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}
