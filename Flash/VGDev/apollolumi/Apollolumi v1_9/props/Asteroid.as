package props
{
	import flash.display.MovieClip;
	import flash.events.Event;
	import flash.geom.Point;
	import flash.geom.ColorTransform;
	import managers.AsteroidManager;
	import managers.Manager;

	public class Asteroid extends Floater
	{		
		private var manager:AsteroidManager;
		private var seamVec:Vector.<SeamTarget> = new Vector.<SeamTarget>();		// all seams on this asteroid
		private var seamStage:int = 3;		// number of traces left (+1 to account for 0)
		private var size:int;				// size of asteroid; 1 is minumum
		private var graceCC:int;			// prevent super-insta-kill of CC

		public var mineral:int;				// 0-4 for R, Y, G, C
		public var impurities:Number;		// percent of other minerals to drop
	
		public function Asteroid(_as:AsteroidManager, _cg:MovieClip, _xP:int, _yP:int, _dX:Number, _dY:Number, _rotSpd:Number, _size:int, _mineral:int)
		{
			super(_cg, _xP, _yP, _dX, _dY, _size * 65);
			ID = "asteroid";
			TITLE = "Asteroid";
			manager = _as;
			size = _size;
			mass = size;
			
			//					 1      2      3
			var sizeArr:Array = [1, 2,  3, 4,  5, 6];
			// every two entries indicates frames to include for that size
			// i.e. first 2 entries are range of frames for size 1

			//trace("Using range: " + sizeArr[2 * size - 2] + " " + sizeArr[2 * size - 1]);
			gotoAndStop(getRand(sizeArr[2 * size - 2], sizeArr[2 * size - 1])); 
			
			//trace("Ast: I spawned with size " + size + " and frame " + currentFrame + " at " + Math.round(x) + " " + Math.round(y));
			//trace("x and y: " + _xP + " " + _yP + "\t" + _dX + " " + _dY + "\t" + _rotSpd);
			
			rotSpd = _rotSpd * (Math.random() > .5 ? -1 : 1);
			
			// display/tint correct minerals, if any
			mineral = _mineral;
			if (mineral != -1 && getChildByName("minerals"))
				minerals.gotoAndStop(currentFrame);
			else
			{
				if (getChildByName("minerals"))
					minerals.visible = false;
			}
			
			impurities = getRand(0, .5, false);
			//trace("Spawned with impurities: " + impurities);
			
			// color the minerals
			var ct:ColorTransform = new ColorTransform;
			switch (mineral)
			{
				case 0:		// R
					//ct.color = 0xE52727;
					ct.color = modifyRGB(0xE52727, 0, impurities, impurities);
				break;
				case 1:		// Y
					ct.color = modifyRGB(0xCCCD23, 0, 0, impurities);
				break;
				case 2:		// G
					ct.color = modifyRGB(0x1E8834, impurities, 0, impurities);
				break;
				case 3:		// C
					ct.color = modifyRGB(0x2790CD, impurities, impurities, 0);
				break;
			}
			minerals.transform.colorTransform = ct;
			setMapIcon(mineral == -1 ? 0x888888 : ct.color, "o");
			mapIcon.scaleX = mapIcon.scaleY *= size * .5;
			
			// set tractor speed based on size of asteroid
			tractSpd = (size >= 5 ? 1 : 10 - size * 2);
			addEventListener(Event.ADDED_TO_STAGE, init);
		}
		
		private function init(e:Event):void
		{
			removeEventListener(Event.ADDED_TO_STAGE, init);
			
			// randomly rotate the asteroid
			var rot:Number = Math.random() * 360;
			rotation = rot;
			
			// collect targets
			var i:int, j:int = numChildren;
			var se:SeamTarget;
			for (i = 0; i < j; i++)
				if (getChildAt(i) is SeamTarget)
				{
					se = getChildAt(i) as SeamTarget;
					seamVec.push(se);
					se.rotation = -rot;
					se.hp = se.hpMax = 100 * size;
				}
		}
		
		override public function childStep(colVec:Vector.<Manager>, p:Point):Boolean
		{
			var t:Point;
			rotation += rotSpd;
			
			// only perform child checks if asteroid iteself is close enough 
			var dCheck:Boolean = getDistN(0, 0, mouseX, mouseY) < (uiDist + 30);
			for each (var st:SeamTarget in seamVec)
			{
				st.rotation -= rotSpd;
				// set seam target to visible if mouse is close enough
				if (dCheck)
				{
					t = localToGlobal(new Point(st.x, st.y));
					st.visible = isTractorTgt ? true : getDistN(mouseX, mouseY, st.x, st.y) < uiDist;
				}
				// otherwise, set seam target to invisible
				else
					st.visible = false;
			}
			if (graceCC > 0)
				graceCC--;
			
			return hp <= 0;
		}
		
		public function leftMouse(p:Point):void
		{
			// give up if no targets or not hitting mouse
			if (seamVec.length == 0) return;
			if (!hitTestPoint(p.x, p.y)) return;

			// handle sequential lasering
			var nextStage:Boolean = true;
			var i:int;
			for (i = seamVec.length - 1; i >= 0; i--)
			{
				var ret:int = seamVec[i].damage(cg.laserDmg, seamStage, seamVec[i].hitTestPoint(p.x, p.y));
				{
					if (ret == -1)
						seamVec.splice(i, 1);
					else if (ret >= seamStage)
						nextStage = false;
				}
			}
			if (nextStage)
			{
				seamStage--;
				for each (var s:SeamTarget in seamVec)
					s.reduceStage();
			}

			// quit if targets still remain
			if (seamVec.length > 0) return;
			if (mineral != -1)		// if not empty asteroid
			{
				var j:int = getRand(size, size + 2);
				for (i = 0; i < j; i++)
					cg.minMan.spawnMineral(x, y, mineral);
				// spawn other minerals
				if (impurities > .17)	// 0-50% impurity rate; do if > 17%
					for (i = 0; i < 4; i++)
						if (i != mineral)
							if (Math.random() < .25 + impurities)
								cg.minMan.spawnMineral(x, y, i, impurities);
			}
			destroy();
		}

		override public function collide(f:Floater):void
		{
			if (f is CommandCenter && graceCC == 0)
			{
				cg.changeHP(size * -6, f);
				graceCC = 15;
			}
			else
			{
				//trace("ast is showing mercy!");
				x -= dX * 2;
				y -= dY * 2;
			}
			trace("hit (ast): " + f);
			handleCollision(this, f);
		}
	
		// takes a color c and increases the values of R, G, and/or B by a percent, or set to 255 if maxed
		private function modifyRGB(c:uint, r:Number, g:Number, b:Number):uint
		{
			var red:uint = ( c >> 16 ) & 0xFF;
			var green:uint = (c >> 8) & 0xFF;
			var blue:uint = c & 0xFF;
			
			red += (255 - red) * r * 1.25;
			green += (255 - green) * g * 1.25;
			blue += (255 - blue) * b * 1.25;
			
			return ( red << 16 ) | ( green << 8 ) | blue;
		}
		
		override public function destroy():void
		{
			if (hp <= 0)
				mineral = -1;
			hp = 0;
			collidable = false;
			if (cont.contains(this))
				cont.removeChild(this);

			cg.sndMan.playSound("ast1");
			if (--size > 0)
			{
				for (var i:int = 0; i < 2; i++)
					manager.spawnAsteroid(x, y, .1, (dX + dY) * .5, rotSpd * .8, rotSpd * 1.2, size, mineral);
			}
			cg.addDebrisFX(15, x, y, 0);
		}
	}
}
