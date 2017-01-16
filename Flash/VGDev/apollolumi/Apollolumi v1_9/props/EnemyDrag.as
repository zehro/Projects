// By Neal Kaviratna

package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import managers.EnemyManager;
	import managers.Manager;
	import flash.utils.getQualifiedClassName;

	public class EnemyDrag extends Enemy
	{	
		public var towing:Boolean; // Whether this is targeting or not. 
		public var towingDelay:Number;
		
		private var tgtDelay:int = 30;		// only check for target once every second
		private var idle:Boolean;			// prevent ramming
		
		public function EnemyDrag(_em:EnemyManager, _cg:MovieClip, _xP:int, _yP:int, _hp:Number, _ts:int)
		{
			super(_em, _cg, _xP, _yP, 0, 0, _hp, _ts);
			spawnGrace = true;
			ID = "enemy";
			SPID = "drag";
			TITLE = "Enemy Ship";
			mass = 3;
			towing = false;
			range = 150;
			towingDelay = 10;

			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			chooseTarget();
		}
		
		public function chooseTarget():void
		{
			var tgtVec:Vector.<MovieClip> = manager.strMan.getVec();
			if (tgtVec.length == 0) return;
			
			// find closest target
			var newTgt:Floater;
			tgt = cg.str_CC;
			
			var lowestDist:Number = getDist(this, tgt);
			var dist:Number = 0;
			idle = true;
			
			var i:int, j:int = tgtVec.length;
			for (i = 0; i < j; i++)
			{
				dist = getDist(this, tgtVec[i]);
				if (dist < lowestDist)
				{
					newTgt = tgtVec[i] as Floater;
					if (newTgt.hp == 0 || newTgt.isEnemyTractorTgt || newTgt.isTractorTgt) continue;	// ignore if already being towed
					lowestDist = dist;
					tgt = newTgt;
					idle = false;
				}
			}
			
			if (tgt.isEnemyTractorTgt)	// don't stupidly ram things if you've got no target
			{
				tgtDelay = 90;
				idle = true;
				tgt = null;
				range = 255;
				return;
			}
			
			rotation = getAngle(x, y, tgt.x, tgt.y);
			gfx.rotation = -rotation;
			
			if (idle && tgt == cg.str_CC)
				idle = false;
				
			switch (tgt.TITLE)
			{
				case "Point Defense Laser": range = 380; break;
				case "Force Defense Turret":
				default: range = 255;
			}
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			spawnGrace = true;
			
			// handle velocity
			if (abs(dX) > maxSpd)
				dX = maxSpd * (dX > 0 ? 1 : -1);
			else
				abs(dX) < minSpd ? dX = 0 : dX *= friction;
			if (abs(dY) > maxSpd)
				dY = maxSpd * (dY > 0 ? 1 : -1);
			else
				abs(dY) < minSpd ? dY = 0 : dY *= friction;
				
			if (--tgtDelay == 0)
			{
				tgtDelay = 30;
				if (!tgt || tgt.isTractorTgt)
					chooseTarget();
			}
			
			if (!tgt || idle)
				return hp <= 0;
				
			if (!towing)
			{								
				rotation = getAngle(x, y, tgt.x, tgt.y);
				gfx.rotation = -rotation;
				
				// check target once more
				chooseTarget();
				if (tgt)
				{
					if (!tgt.isEnemyTractorTgt && getDist(this, tgt) < range)
					{
						this.towing = true;
						tgt.isEnemyTractorTgt = true;
						tgt.spawnGrace = true;
						this.maxSpd = ((this.maxSpd)/(tgt.mass+1)); // slow down based on the mass of the towed object
						//trace("towin!");
						cg.guiMan.warningWarn("Base being towed!", 0xFF0000, false);
						rotation = getAngle(-x, -y, -tgt.x, -tgt.y);
						gfx.rotation = -this.rotation;
					}
				}
			}
			else
			{
				if (this.towingDelay < 0)
				{
					tgt.x += this.dX;
					tgt.y += this.dY;
				}
				else
					towingDelay--;
				
				rotation = getAngle(-x, -y, -tgt.x, -tgt.y);
				gfx.rotation = -rotation;
					
				if ((abs(tgt.x) > LIM_X - 200) || (abs(tgt.y) > LIM_Y - 200))
				{
					cg.addExplosion(tgt.x, tgt.y, 2);
					tgt.destroy();
				}

				drawLine(1, 0x00FFFF, .3, tgt);

				if (!tgt || tgt.hp == 0 || tgt.isTractorTgt || getDist(this, tgt) > range + 30)
				{
					//trace("done towin!");
					this.towing = false;
					if (tgt)
					{
						tgt.isEnemyTractorTgt = false;
						tgt.spawnGrace = false;
						tgt.dX = this.dX;
						tgt.dY = this.dY;
					}
					this.maxSpd = 3;
					drawLine(1, 0x000000, .4, this);
				}
			}

			if (idle)
			{
				reverse();
				fwdL.visible = fwdR.visible = false;
				rearL.visible = rearR.visible = true;
				abL.visible = abR.visible = false;
			}
			else
			{
				forward();
				fwdL.visible = fwdR.visible = !towing;
				rearL.visible = rearR.visible = false;
				abL.visible = abR.visible = towing;
			}
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
			cg.changeHP(-6, f);		// -- temporary		
			cg.changeHP(-6, this);		// -- temporary		
			//trace("hit (dragger): " + f);
			handleCollision(this, f);
		}
		
		override public function destroy():void
		{
			if (tgt)
				tgt.isEnemyTractorTgt = false;
			cg.addExplosion(x, y, 3);
					cg.sndMan.playSound("explode3");
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
	}
}
