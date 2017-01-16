package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import managers.EnemyManager;
	import managers.Manager;

	public class EnemyBomb extends Enemy
	{	
		private var cool:int;
		public var accuracy:Number = 3;			// 0 is 100% accuracy
		
		private var targetDelay:int = 30;		// only check for new target every second
		
		public function EnemyBomb(_em:EnemyManager, _cg:MovieClip, _xP:int, _yP:int, _hp:Number, _ts:int)
		{
			super(_em, _cg, _xP, _yP, 0, 0, _hp, _ts);
			spawnGrace = true;
			ID = "enemy";
			SPID = "shoot";
			TITLE = "Enemy Ship";
			mass = 1.2;
			
			//moveSpd = .1;
			//maxSpd = 2;
			range = 900;
			rof = 150;
			dmg = 20;
			
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		protected function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			chooseTarget();
		}
		
		public function chooseTarget():void
		{
			if (!tgt && cg.str_CC.hp > 0 && Math.random() > .75)
				tgt = cg.str_CC;
			else
			{
				var tgtVec:Vector.<MovieClip> = manager.strMan.getVec();
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
			}
			//trace("Shooter: Targeting " + tgt);
			rotation = getAngle(x, y, tgt.x, tgt.y);
			gfx.rotation = -rotation;
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// handle velocity
			if (abs(dX) > maxSpd)
				dX = maxSpd * (dX > 0 ? 1 : -1);
			else
				abs(dX) < minSpd ? dX = 0 : dX *= friction;
			if (abs(dY) > maxSpd)
				dY = maxSpd * (dY > 0 ? 1 : -1);
			else
				abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			// choose new target if current target is destroyed
			if (!tgt || tgt.hp <= 0)
				if (--targetDelay == 0)
				{
					targetDelay = 30;
					chooseTarget();
				}
			
			rotation = getAngle(x, y, tgt.x, tgt.y);
			gfx.rotation = -rotation;

			if (cool > 0)
				cool--;
			else if (tgt)
			{
				if (((abs(x) < LIM_X) && (abs(y) < LIM_Y)) && getDist(this, tgt) < range)
				{
					if (!spawnGrace && cool == 0)
					{
						cg.sndMan.playSound("explode3");
						//trace("Shooter: SHOT!");
						cg.addProjectile(x, y, 11, rotation + getRand(-accuracy, accuracy), "bomb", 280, dmg, false);
						dY -= Math.sin(degreesToRadians(rotation)) * moveSpd * 12;		// recoil
						dX -= Math.cos(degreesToRadians(rotation)) * moveSpd * 12;
						cool = rof;
					}
					eng1.visible = eng2.visible = eng3.visible = false;
				}
				else
				{
					eng1.visible = eng2.visible = eng3.visible = true;
					forward();
				}
			}
			return hp <= 0;
		}
		
		// handles when player lasers this object
		public function leftMouse(p:Point):void
		{
			if (!hitTestPoint(p.x, p.y)) return;
			
			hp -= manager.par.laserDmg;
			if (hp <= 0)
				destroy();
		}
		
		override public function collide(f:Floater):void
		{
			cg.changeHP(-7, f);		// -- temporary		
			cg.changeHP(-9, this);		// -- temporary	
			//trace("hit (Shooter): " + f);
			handleCollision(this, f);
		}
		
		override public function destroy():void
		{
			cg.addExplosion(x, y, 3);
			cg.sndMan.playSound("explode2");
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}
