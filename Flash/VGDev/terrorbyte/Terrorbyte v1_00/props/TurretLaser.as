package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;
	import managers.TurretManager;
	import managers.NodeManager;

	public class TurretLaser extends Node
	{
		protected var origDir:Number = 0;		// starting rotation		
		protected var manT:TurretManager;		// correctly-typed manager
		public var stun:int;					// counter for being disabled

		public var range:int = 125;				// firing range
		public var tgt:MovieClip;				// target
		
		public var cooldown:int = 0;			// time before able to fire (unable to fire if not 0)
		public var rof:int = 60;				// time to reset cooldown to after firing
		
		protected var gfxRange:MovieClip;		// to draw range circle on, etc.
		protected var gfxLaser:MovieClip;		// to draw laser
		protected var ROF_BAR:int = 40;			// width of ROF bar
		
		private var fireDisp:int;		// time to draw laser
		private var FIRE_DISP:int = 6;
		private var ptLas:Point;
		
		public function TurretLaser(man:Manager, _nodeMan:NodeManager, xP:int, yP:int, id:String, _type:String, tb:int, resist:int, tier:int, _rot:Number)
		{
			super(man, _nodeMan, xP, yP, id, tb, resist, tier);
			gotoAndStop(_type);
			
			timeBox = hackTimer;
			infoBox = infobox;
			tbQueue = tbqueue;
			
			tfTb = tf_tb;
			tfResist = tf_resist;
			tfTier = tf_tier;
			
			setup();
			
			gfxRange = new MovieClip();						// setup turret drawing graphics
			addChild(gfxRange);
			
			gfxLaser = new MovieClip();						// setup turret drawing graphics
			addChild(gfxLaser);
		
			drawRange();
			
			timeHackBase = 3.5;
			timeHackTier = 1.5;
			timeStunBase = 5;
			timeStunTier = 3.0;
			
			manT = manager as TurretManager;
		}
		
		public function setAttribute(type:String, amt:Number):void
		{
			switch (type)
			{																			// defaults
				case "rof":			rof = amt;									break;	//  60
				case "range":		range = amt; drawRange();					break;	// 125				
				case "hackBase":	// 3.5	time to hack Tier 1
					timeHackBase = amt; 					
					infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
				break;		
				case "hackTier":	// 1.5	time to hack each +1 Tier
					timeHackTier = amt; 	
					infoBox.tf_hack.text = (timeHackBase + (tier-1) * timeHackTier).toFixed(1) + " sec";
				break;		
				case "stunBase":	// 5.0	time of stun Tier 1
					timeStunBase = amt; 				
					infoBox.tf_stun.text = (timeStunBase + (tier-1) * timeStunTier).toFixed(1) + " sec";
				break;		
				case "stunTier":	// 3.0	time of stun each +1 Tier
					timeStunTier = amt; 	
					infoBox.tf_stun.text = (timeStunBase + (tier-1) * timeStunTier).toFixed(1) + " sec";
				break;		
			}
		}
					
			
		override public function restart():void
		{
			changeCooldown(0, false);
			tgt = null;
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

		override public function step():Boolean
		{
			if (!isRunning) return false;
			
			if (cooldown > 0)
				changeCooldown(-1);
			
			if (fireDisp > 0 && ptLas)
			{
				fireDisp--;
				return childStep();
			}
			else if (fireDisp == 0)
			{
				fireDisp = -1;
				ptLas = null;
				gfxLaser.graphics.clear();
			}
			
			if (nodeState == "offline")
			{
				tgt = null;
				changeCooldown(rof, false);
				return false;
			}

			if (!tgt || !tgt.isAlive)		// get new target
			{
				tgt = null;
				for each (var plane:Plane in manT.planeMan.getVec())
				{
					if (!plane.isAlive) continue;
					if (getDist(this, plane) <= range)
						if (!tgt || tgt.tgtValue > plane.tgtValue)
							tgt = plane;
				}
			}
			else if (tgt)
			{
				if (getDist(this, tgt) > range)		// check if target out of range
					tgt = null;
				else if (cooldown == 0)				// check if able to fire
				{
					manT.cg.eng.soundMan.playSound("SFX_laser");
					fireDisp = FIRE_DISP;
					changeCooldown(rof, false);
					ptLas = new Point(tgt.x, tgt.y);
					gfxLaser.graphics.clear();
					gfxLaser.graphics.lineStyle(1, 0xEE422D, .6);
					gfxLaser.graphics.moveTo(0, 0);
					gfxLaser.graphics.lineTo(ptLas.x - x, ptLas.y - y);
					gfxLaser.graphics.lineStyle(2, 0xEE422D, .3);
					gfxLaser.graphics.lineTo(0, 0);		
					tgt.destroy();
					tgt = null;
				}
			}
			
			return childStep();
		}
	}
}
