/*
	Floater.as

	Alexander Huynh
	08-30-13
	VGDev - Apollolumi
	
	Main generic object.
*/

package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;
	import flash.events.Event;
	import flash.geom.ColorTransform;

	public class Floater extends MovieClip
	{
		public var cg:MovieClip;			// should always be ContainerGame
		public var cont:MovieClip;			// should always be cg.game.cont
		public var hp:Number, hpMax:Number;
		// movement vars
		public var dX:Number, dY:Number;
		public var LIM_X:int, LIM_Y:int;
		public var spawnGrace:Boolean;		// ignore bounds checks to allow offscreen spawning
		public var ignoreBounds:Boolean;	// ignore bounds and kill if offscreen
		public var heightSP:Number;			// prevents height/width being changed by graphics
		public var widthSP:Number;
		// tractor vars
		public var isTractorTgt:Boolean, tractorable:Boolean = true;
		public var isEnemyTractorTgt:Boolean;
		public var tractSpd:int;
		public var mass:Number = 1;
		public var collidable:Boolean = true, collideWithSameType:Boolean = true;
		public var useLargestHitbox:Boolean;
		
		public var rotSpd:Number = 0;			// visual rotation effect
		public var uiDist:int = 175;			// distance from mouse at which to set ui elements visible
		
		public var TITLE:String = "Floater";	// name to display on UI
		public var ID:String = "";				// category of object (ex structure)
		public var SPID:String = "";			// special identifier (ex command center)
		private var PADDING:Number = 1;			// factor to multiply with when updating position after colliding

		protected var gfx:MovieClip;		// used to draw lines
		protected var dist:Number = 0;		// for getting closest MC to cursor
		protected var DIST:Number = 0;		// smaller of height and width
		protected var m:Point;
		protected var isMouseOver:Boolean;
		
		protected var mapIcon:MovieClip;
		protected var mapColor:ColorTransform;
		
		public function Floater(_cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _hp:Number, _ts:int = 0)
		{
			cg = _cg;
			cont = cg.game.cont;
			
			x = _xP; y = _yP;
			dX = _dX; dY = _dY;
			hp = hpMax = _hp;
			tractSpd = _ts;
			
			LIM_X = cg.LIM_X + 300;
			LIM_Y = cg.LIM_Y + 250;
			
			mapIcon = new MMIcon();
			cg.minimap.addChild(mapIcon);
			mapIcon.scaleX = mapIcon.scaleY = 1 / cg.minimap.scaleX;
			mapColor = new ColorTransform();
			
			gfx = new MovieClip();
			addChild(gfx)
			TITLE = "---";
			
			heightSP = height;
			widthSP = width;
			
			DIST = .5 * (width > height ? height : width);
			
			addEventListener(Event.REMOVED_FROM_STAGE, cleanup);
		}
		
		// don't override this function unless necessary
		public function step(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// ignore bounds check if spawned offscreen
			if (spawnGrace && (abs(x) < LIM_X) && (abs(y) < LIM_Y))
				spawnGrace = false;
			// if not being tractored
			if (!isTractorTgt)
			{
				// update position with velocity
				x += dX; y += dY;
				// keep in bounds
				if (!spawnGrace)
				{
					if (abs(x) > LIM_X)
					{
						if (ignoreBounds)
							destroy();
						else
						{
							dX *= -1;
							x = LIM_X * (x < 0 ? -1 : 1) + dX;
						}
					}
					if (abs(y) > LIM_Y)
					{
						if (ignoreBounds)							
							destroy();
						else
						{
							dY *= -1;
							y = LIM_Y * (y < 0 ? -1 : 1) + dY;
						}
					}
				}
			}
			// handle collisions
			if (collidable)
				checkColl(colVec);
				
			// handle bottom left UI info
			m = localToGlobal(new Point());
			dist = getDistN(m.x, m.y, p.x, p.y);
			if (dist < DIST && dist < cg.uiDist)
			{
				cg.uiDist = dist;
				cg.uiMC = this;
			}
			
			mapSelf();
			
			return childStep(colVec, p);
		}
		
		// returns true if this should be removed (i.e. HP is 0)
		public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			// -- override this function
			return false;
		}
		
		public function checkColl(colVec:Vector.<Manager>):void
		{
			var f:Floater;
			var distCheck:Number;
			allLoop: for each (var m:Manager in colVec)				// -- change to "for" if optimization needed
				for each (var mc:MovieClip in m.getVec())
				{
					if (!(mc is Floater) || mc == this) continue;
					f = mc as Floater;
					// ignore collisions with same types
					if (!collideWithSameType && f.ID == ID) continue;
					if (!f.collidable) continue;
					// calculate 'radius'
					if (useLargestHitbox)
					{
						distCheck = heightSP > f.heightSP ? heightSP : f.heightSP;
						if (f.widthSP > distCheck)
							distCheck = widthSP > f.widthSP ? widthSP : f.widthSP;
					}
					else
					{
						distCheck = heightSP < f.heightSP ? heightSP : f.heightSP;
						if (f.widthSP < distCheck)
							distCheck = widthSP < f.widthSP ? widthSP : f.widthSP;
					}
					// if distance is low enough
					if (getDist(this, f) < .8 * distCheck)
					{
						if (isTractorTgt)
							cg.shutTractor();
						collide(f);
						break allLoop;
					}
				}
		}
		
		public function collide(f:Floater):void
		{
			// -- override this function if needed
			//trace("hit (floater): " + f);
			handleCollision(this, f);
		}
		
 		protected function handleCollision(m1:Floater, m2:Floater):void
        {
			// prevent low-speed collisions
			if (abs(m1.dX) < .7) m1.dX = .7 * (m1.dX > 0 ? 1 : -1);
			if (abs(m1.dY) < .7) m1.dY = .7 * (m1.dY > 0 ? 1 : -1);
			if (abs(m2.dX) < .7) m2.dX = .7 * (m2.dX > 0 ? 1 : -1);
			if (abs(m2.dY) < .7) m2.dY = .7 * (m2.dY > 0 ? 1 : -1);

			//back up a step to prevent overlap
			m1.x -= m1.dX; m1.y -= m1.dY;
			m2.x -= m2.dX; m2.y -= m2.dY;

			// YEAH, SCIENCE
			var mass1:Number = m1.mass;
			var mass2:Number = m2.mass;
			var mass_sum = mass1 + mass2;
			var velX1:Number = m1.dX;
			var velY1:Number = m1.dY;
			var velX2:Number = m2.dX;
			var velY2:Number = m2.dY;
			
			var new_velX1 = (velX1 * (mass1 - mass2) + (2 * mass2 * velX2)) / mass_sum;
			var new_velX2 = (velX2 * (mass2 - mass1) + (2 * mass1 * velX1)) / mass_sum;
			var new_velY1 = (velY1 * (mass1 - mass2) + (2 * mass2 * velY2)) / mass_sum;
			var new_velY2 = (velY2 * (mass2 - mass1) + (2 * mass1 * velY1)) / mass_sum;
			
			m1.dX = new_velX1;
			m1.dY = new_velY1;
			m2.dX = new_velX2;
			m2.dY = new_velY2;
			
			// bounce the object with the lower mass more
			m1.x += m1.dX * PADDING * (mass1 < mass2 ? (m1.dX < 1 ? PADDING : 1) : 1);
			m1.y += m1.dY * PADDING * (mass1 < mass2 ? (m1.dY < 1 ? PADDING : 1) : 1);
			m2.x += m2.dX * PADDING * (mass1 > mass2 ? (m2.dX < 1 ? PADDING : 1) : 1);
			m2.y += m2.dY * PADDING * (mass1 > mass2 ? (m2.dY < 1 ? PADDING : 1) : 1);
        }
		
		public function changeHP(n:Number):void
		{
			hp += n;
			if (hp > hpMax) hp = hpMax;
			else if (hp <= 0)
				destroy();
		}
		
		// --------------------------------------------------------------------- MATH FUNCTIONS

		// more efficient than Math.abs(...)
		protected function abs(n:Number):Number
		{
			return n < 0 ? -n : n;
		}
 
 		// get the distance between 2 MovieClips
		protected function getDist(m1:MovieClip, m2:MovieClip):Number
		{
			return Math.sqrt((m2.x - m1.x) * (m2.x - m1.x) + (m2.y - m1.y) * (m2.y - m1.y));
		}
		
		// get the distance between 2 points
		public function getDistN(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			return Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
		}
		
		// get the angle between 2 points
		public function getAngle(x1:Number, y1:Number, x2:Number, y2:Number):Number
		{
			var dx:Number = x2 - x1;
			var dy:Number = y2 - y1;
			return radiansToDegrees(Math.atan2(dy,dx));
		}
		
		protected function degreesToRadians(degrees:Number):Number
		{
			return degrees * .0175;
		}
		
		protected function radiansToDegrees(radians:Number):Number
		{
			return radians * 57.296;
		}
		
		// draw a line using Flash graphics from self to a target MovieClip
		protected function drawLine(lineWidth:int, lineCol:int, lineAlp:Number, tgt:MovieClip, clearG:Boolean = true):void
		{
			if (clearG) gfx.graphics.clear();
			gfx.graphics.lineStyle(lineWidth, lineCol, lineAlp);		
			gfx.graphics.moveTo(0, 0);
			gfx.graphics.lineTo(tgt.x - x, tgt.y - y);
		}
		
		// draw a line using Flash graphics from one point to another point
		protected function drawLineN(lineWidth:int, lineCol:int, lineAlp:Number, x1:Number, y1:Number, x2:Number, y2:Number, clearG:Boolean = true):void
		{
			if (clearG) gfx.graphics.clear();
			gfx.graphics.lineStyle(lineWidth, lineCol, lineAlp);		
			gfx.graphics.moveTo(x1, y1);
			gfx.graphics.lineTo(x2, y2);
		}
		
		protected function getRand(min:Number, max:Number, useInt:Boolean = true):Number   
		{  
			if (useInt)
				return (int(Math.random() * (max - min + 1)) + min);
			return Math.random() * max + min;
		}
		
		protected function setMapIcon(col:uint, type:String):void
		{
			mapIcon.iconBase.gotoAndStop(type);
			mapColor.color = col;
			mapIcon.iconBase.transform.colorTransform = mapColor;			
		}
		
		protected function mapSelf():void
		{
			mapIcon.x = x;
			mapIcon.y = y;
		}

		public function destroy():void
		{
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);
		}
		
		private function cleanup(e:Event)
		{
			removeEventListener(Event.REMOVED_FROM_STAGE, cleanup);
			if (cg.minimap.contains(mapIcon))
				cg.minimap.removeChild(mapIcon);
		}
	}
}
