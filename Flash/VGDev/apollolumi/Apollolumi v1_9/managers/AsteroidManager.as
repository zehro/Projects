package managers
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import props.Asteroid;
	import props.Floater;

	public class AsteroidManager extends Manager
	{
		public var astVec:Vector.<MovieClip> = new Vector.<MovieClip>();

		public function AsteroidManager(_par:MovieClip)
		{
			super(_par);
		}
		
		override public function step(ld:Boolean, rd:Boolean, colVec:Vector.<Manager>, p:Point):void
		{
			var i:int;
			if (astVec.length == 0) return;
			for (i = astVec.length - 1; i >= 0; i--)
				if (astVec[i].step(colVec, p))
					astVec.splice(i, 1);			// remove the Asteroid from the vector
				else if (ld)
					astVec[i].leftMouse(p);
		}
		
		public function spawnAsteroid(_xP:int, _yP:int, _velMin:Number, _velMax:Number, _rotMin:Number, _rotMax:Number,
									  _size:int, _mineral:int, spawnOutside:Boolean = false):Asteroid
		{
			// -- if blocked, try again away from boundary
			var fleeDirX:int = _xP > 0 ? -1 : 1; var fleeX:int = 0;
			var fleeDirY:int = _yP > 0 ? -1 : 1; var fleeY:int = 0;
	
			var ast:Asteroid = new Asteroid(this, par, _xP + getRand(-20, 20),  _yP + getRand(-20, 20),
											(_xP > 0 ? -1 : 1) * getRand(_velMin, _velMax),
											(_yP > 0 ? -1 : 1) * getRand(_velMin, _velMax),
											getRand(_rotMin, _rotMax) * (Math.random() > .5 ? 1 : -1),
											_size, _mineral);
			ast.spawnGrace = spawnOutside;
			_xP = ast.x; _yP = ast.y
			par.game.cont.addChild(ast);
			astVec.push(ast);
			
			//trace("astMan: spawned asteroid at " + _xP + " " + _yP);
			//trace(astVec);

			//return ast;	// disable spawn collision prevention

			var notColliding:Boolean;
			var giveUp:int = 200;
			// keep moving around until you find an empty place
			do
			{
				notColliding = true;
				var f:Floater;
				allLoop: for each (var m:Manager in par.managerC)				// -- change to for if optimization needed
					for each (var mc:MovieClip in m.getVec())
					{
						if (!(mc is Floater)) continue;
						if (mc.x == ast.x && mc.y == ast.y)
							continue;
						f = mc as Floater;
						if (f.collidable && f.hitTestObject(ast))
						{
							notColliding = false;
							break allLoop;
						}
					}
				if (--giveUp <= 0)
					notColliding = true;
				if (!notColliding)
				{
					//trace("Ouch!");
					fleeX++; fleeY++;
					ast.x = _xP + 7 * fleeDirX * fleeX;
					ast.y = _yP + 7 * fleeDirY * fleeY;
					// don't try too far in one direction
					if (fleeX > 25)
					{
						fleeX = 0; fleeDirX = Math.random() > .5 ? 1 : -1;
						if (Math.random() > .66)
						fleeDirX = 0;
					}
					if (fleeY > 25)
					{
						fleeY = 0; fleeDirY = Math.random() > .5 ? 1 : -1;
						if (Math.random() > .66)
						fleeDirY = 0;
					}
				}
			} while (!notColliding);
			
			return ast;
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return astVec;
		}
		
		override public function destroy():void
		{
			for each (var a:Asteroid in astVec)
			{
				a.destroy();
				if (par.game.cont.contains(a))
					par.game.cont.removeChild(a);
			}
			astVec = new Vector.<MovieClip>();
		}
	}
}
