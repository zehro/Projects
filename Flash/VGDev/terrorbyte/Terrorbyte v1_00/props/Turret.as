package props
{
	import flash.display.MovieClip;
	import managers.Manager;
	import managers.TurretManager;
	import managers.NodeManager;

	public class Turret extends Node
	{
		protected var origDir:Number = 0;		// starting rotation		
		protected var manT:TurretManager;		// correctly-typed manager
		public var stun:int;					// counter for being disabled

		public var rotSpd:Number = 7.5;			// rotation speed (tracking)
		public var rotIdleSpd:Number = .3;		// rotation speed (idle)
		protected var spawnDist:Number = 20;	// offset from origin to place projectile
		
		public var range:int = 100;				// firing range
		public var tgt:MovieClip;				// target
		
		public var cooldown:int = 0;			// time before able to fire (unable to fire if not 0)
		public var rof:int = 90;				// time to reset cooldown to after firing
		public var fireAngleRange:Number = 10;	// target must be within this angle to fire
		protected var inAngleRange:Boolean;		// if the turret is facing the target
		
		protected var gfxRange:MovieClip;			// to draw range circle on, etc.
		protected var gfxAngle:MovieClip;			// to draw angle indicator
		protected var ROF_BAR:int = 40;			// width of ROF bar
		
		public function Turret(man:Manager, _nodeMan:NodeManager, xP:int, yP:int, id:String, _type:String, tb:int, resist:int, tier:int, _rot:Number)
		{
			super(man, _nodeMan, xP, yP, id, tb, resist, tier);
			gotoAndStop(_type);
			top.rotation = maskCover.rotation = origDir = _rot;		// orientate turret
			
			timeBox = hackTimer;
			infoBox = infobox;
			tbQueue = tbqueue;
			
			tfTb = tf_tb;
			tfResist = tf_resist;
			tfTier = tf_tier;
			
			setup();
			
			gfxRange = new MovieClip();						// setup turret drawing graphics
			addChild(gfxRange);
			
			gfxAngle = new MovieClip();						// setup turret drawing graphics
			addChild(gfxAngle);
		
			gfxRange.buttonMode = gfxRange.mouseEnabled = false;
			gfxAngle.buttonMode = gfxAngle.mouseEnabled = false;
		
			drawRange();
			drawFireAngle();
			
			manT = manager as TurretManager;
		}
		
		public function setAttribute(type:String, amt:Number):void
		{
			switch (type)
			{																			// defaults
				case "rof":			rof = amt;									break;	//  90
				case "range":		range = amt; drawRange(); drawFireAngle();	break;	// 100
				case "rotSpdIdle":	rotIdleSpd = amt;							break;	//  .3
				case "rotSpdTrack":	rotSpd = amt;								break;	// 7.5
				case "angleFire":	fireAngleRange = amt; drawFireAngle();		break;	//  10
				case "rotDir":		rotIdleSpd = rotIdleSpd * amt; 				break;	//   1
				
				case "hackBase":	// 3.0	time to hack Tier 1
					timeHackBase = amt; 					
					infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
				break;		
				case "hackTier":	// 1.5	time to hack each +1 Tier
					timeHackTier = amt; 	
					infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
				break;		
				case "stunBase":	// 4.0	time of stun Tier 1
					timeStunBase = amt; 				
					infoBox.tf_stun.text = (timeStunBase + (tier-1) * timeStunTier).toFixed(1) + " sec";
				break;		
				case "stunTier":	// 1.5	time of stun each +1 Tier
					timeStunTier = amt; 	
					infoBox.tf_stun.text = (timeStunBase + (tier-1) * timeStunTier).toFixed(1) + " sec";
				break;		
			}
		}
					
			
		override public function restart():void
		{
			changeCooldown(0, false);
			tgt = null;
			top.rotation = maskCover.rotation = origDir;
		}		
		
		override protected function drawRange():void
		{
			gfxRange.graphics.clear();
			gfxRange.graphics.lineStyle(1, 0xEE422D, nodeState == "offline" ? .15 : .5);
			gfxRange.graphics.drawCircle(0, 0, range);
			
			filters = nodeState == "offline" ? [] : [glowR];
		}
		
		public function changeCooldown(amt:int, relative:Boolean = true):void
		{
			relative ? cooldown += amt : cooldown = amt;
			rofBar.width = ((rof - cooldown) / rof) * ROF_BAR;
		}
		
		protected function drawFireAngle():void
		{
			gfxAngle.graphics.clear();
			gfxAngle.graphics.lineStyle(1, 0xEE422D, .3);
			gfxAngle.graphics.moveTo(0, 0);
			gfxAngle.graphics.lineTo(Math.cos(degreesToRadians(top.rotation + fireAngleRange)) * range,
									 Math.sin(degreesToRadians(top.rotation + fireAngleRange)) * range);
			gfxAngle.graphics.moveTo(0, 0);
			gfxAngle.graphics.lineTo(Math.cos(degreesToRadians(top.rotation - fireAngleRange)) * range,
									 Math.sin(degreesToRadians(top.rotation - fireAngleRange)) * range);
		}
		
		override public function step():Boolean
		{
			if (!isRunning) return false;
			
			if (nodeState == "offline")
			{
				tgt = null;
				changeCooldown(rof, false);
				return false;
			}
			
			drawFireAngle();

			if (cooldown > 0)
				changeCooldown(-1);

			if (!tgt || !tgt.isAlive)		// get new target
			{
				tgt = null;
				for each (var plane:Plane in manT.planeMan.getVec())
				{
					if (!plane.isAlive) continue;
					if (getDist(this, plane) <= range)
					{
						if (!tgt || tgt.tgtValue > plane.tgtValue)
						{
							tgt = plane;
							manager.cg.gameMan.bonusDetected = false;
						}
					}
				}
			}
			else if (tgt)
			{
				if (getDist(this, tgt) > range)				// check if target out of range
					tgt = null;
				else if (inAngleRange && cooldown == 0)		// check if able to fire
				{
					changeCooldown(rof, false);
					manT.missMan.spawnMissile(x + Math.cos(degreesToRadians(top.rotation)) * spawnDist,
											  y + Math.sin(degreesToRadians(top.rotation)) * spawnDist,
											  top.rotation, tgt);
				}
			}
			
			if (tgt)
			{
				// turn to face target
				var tgtAng:Number = getAngle(this.x, this.y, tgt.x, tgt.y);
				if (tgtAng > top.rotation + 180) tgtAng -= 360;
				if (tgtAng < top.rotation - 180) tgtAng += 360;
				
				// if turret is facing the target enough
				inAngleRange = abs(top.rotation - tgtAng) <= fireAngleRange;
					
				// set to target angle if increment is too large
				if (abs(tgtAng - top.rotation) < rotSpd * .5)
					top.rotation = tgtAng;
				// otherwise turn towards target angle
				else
				{
					var rotInc:Number = (tgtAng - top.rotation) * .2;// / rotSpd;
					// set speed limit
					if (abs(rotInc) > rotSpd)					
						rotInc = (rotInc < 0 ? -1 : 1) * rotSpd;
					top.rotation += rotInc;
				}
				maskCover.rotation = top.rotation;
			}
			else		// idle rotation
			{
				top.rotation += rotIdleSpd;
				maskCover.rotation = top.rotation;
			}
			
			return childStep();
		}
	}
}
