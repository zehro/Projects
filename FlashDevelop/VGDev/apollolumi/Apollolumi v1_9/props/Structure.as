package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import managers.StructureManager;
	import managers.Manager;
	import props.Mineral;
	import flash.events.MouseEvent;

	public class Structure extends Floater
	{		
		protected var manager:StructureManager;
		
		public var minSpd:Number = .1;
		public var maxSpd:Number = 5;
		public var friction:Number = .98;

		public var range:Number = 500;
		public var tRange:Number = 0;
		public var tractorSpeed:int = 5;
		protected var tractorTgt:Mineral;
		
		protected var rangefinder:MovieClip;
	
		public function Structure(_sm:StructureManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int = 0)
		{
			super(_cg, _xP, _yP, _dX, _dY, _hp, _ts);
			
			rangefinder = new MovieClip();
			addChild(rangefinder)
			
			setMapIcon(0x6BD982, "s");
			manager = _sm;
			addEventListener(MouseEvent.ROLL_OVER, onOver);
			addEventListener(MouseEvent.ROLL_OUT, onOut);
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// -- override as needed
			abs(dX) < minSpd ? dX = 0 : dX *= friction;
			abs(dY) < minSpd ? dY = 0 : dY *= friction;
			
			rotation += 4;
			return hp == 0;
		}
		
		// seek out nearest mineral and try to get it
		protected function handleTractor():void
		{
			gfx.graphics.clear();
			
			if (isTractorTgt || dX != 0 || dY != 0)		// quit if you're being pulled or moving
			{
				if (tractorTgt)
				{
					tractorTgt.dragged = false;
					tractorTgt = null;
				}
				return;
			}
			
			if (!tractorTgt)
			{
				var minDist:Number = range;
				var m:Mineral = null;
				var dist:Number = 0;
				for each (var mc:MovieClip in cg.minMan.getVec())
				{
					m = mc as Mineral;
					if (m.dragged)
						continue;
					dist = getDist(this, m);
					if (dist < minDist)
					{
						minDist = dist;
						tractorTgt = m;
					}
				}
				if (tractorTgt)
				{
					tractorTgt.dragged = true;
					var rot:Number = tractorTgt.getAngle(tractorTgt.x, tractorTgt.y, x, y);
					tractorTgt.dY = Math.sin(degreesToRadians(rot)) * tractorSpeed;
					tractorTgt.dX = Math.cos(degreesToRadians(rot)) * tractorSpeed;
				}
			}
			else
			{
				if (tractorTgt.hp == 0)
				{
					tractorTgt = null;
					drawLine(1, 0x000000, 0, this);
					return;
				}
				tRange = getDist(this, tractorTgt)
				if (tRange > range)
				{
					tractorTgt.dragged = false;
					tractorTgt = null;
					drawLine(1, 0x000000, 0, this);
				}
				else if (tRange < 25)
				{
					tractorTgt.rightMouse(true);
					tractorTgt = null;
					drawLine(1, 0x000000, 0, this);
					cg.sndMan.playSound("minPickup");
				}
				else
					drawLine(1, 0x86FCF2, .7, tractorTgt);
			}
		}
		
		public function playAlert():void
		{
			if (mapIcon.alert.currentFrame == 1)
				mapIcon.alert.play();
		}
		
		public function playAtk():void
		{
			mapIcon.atk.gotoAndPlay(2);
		}
		
		// draw range using Flash graphics from self to a target MovieClip
		public function drawRange():void
		{
			rangefinder.graphics.clear();
			rangefinder.graphics.lineStyle(1, 0xFFFFFF, .25);		
			rangefinder.graphics.drawCircle(0, 0, range);
		}
		
		protected function onOver(e:MouseEvent):void
		{
			drawRange();
		}
		
		protected function onOut(e:MouseEvent):void
		{
			rangefinder.graphics.clear();
		}
		
		public function resetTurret():void
		{
			drawLine(1, 0x000000, 0, this);
			tractorTgt = null;
		}
		
		override public function destroy():void
		{
			hp = 0;
			collidable = false;
			cg.addDebrisFX(15, x, y, 2);
			cg.addExplosion(x, y, 1);
			cg.sndMan.playSound("explode3");
			cg.guiMan.warningWarn("We lost a turret!", 0xFF0000, false);
			if (cont.contains(this))
				cont.removeChild(this);
			removeEventListener(MouseEvent.ROLL_OVER, onOver);
			removeEventListener(MouseEvent.ROLL_OUT, onOut);
		}
	}
}
