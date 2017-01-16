package props
{
	import flash.display.MovieClip;
	import flash.geom.Point;
	import managers.Manager;
	import managers.PlaneManager;

	public class Plane extends Flier
	{
		private var waypoints:Vector.<Point> = new Vector.<Point>();
		private var route:Vector.<Point>;
		
		private var origX:int, origY:int;				// starting location
		private var origDir:Number;						// starting rotation
		private var nextWaypoint:Point;
		private var waypointThreshold:Number = 5;		// distance needed to clear a waypoint
		
		public var tgtValue:int;			// turrets will prefer higher values
		public var vital:Boolean = true;	// if TRUE and is shot down, simulation fails
		public var safe:Boolean;			// set to TRUE once second-to-last waypoint is reached
		
		public var locks:int;		// number of missiles on this
		
		public function Plane(man:Manager, _type:String, _xP:int, _yP:int, _dir:Number, _vital:Boolean)
		{
			super(man, _xP, _yP, 0, _dir);
			origX = x; origY = y;
			gotoAndStop(_type);							// switch image to correct plane type e.g. "fighter"
			base.rotation = origDir = facing = _dir;	// rotate image to correct angle
			vital = _vital;
			//trace("Setting " + _type + "'s vital to " + vital);
						
			base.filters = [glowB];
			ui_warning.filters = [glowR];

			waypoints.push(new Point(x, y));
			resetRoute();
			
			switch (_type)
			{
				case "passenger":			// movement values FINALIZED - DO NOT CHANGE
					rotSpd = 0.6;
					moveSpd = 1.3;
					tgtValue = 2;
					trailLength = 40;
					trailSaturation = 6;
					waypointThreshold = 10;
				break;
				case "cargo":				// movement values FINALIZED - DO NOT CHANGE
					rotSpd = 0.45;
					moveSpd = 0.9;
					tgtValue = 2;
					trailLength = 40;
					trailSaturation = 6;
					waypointThreshold = 13;
				break;
				case "fighter":
				case "drone":
					rotSpd = 2;
					moveSpd = 2;
					tgtValue = 0;
				break;
				case "swept":
					rotSpd = 2.5;
					moveSpd = 1.75;
					tgtValue = 1;
					trailSaturation = 9;
				break;
				case "ultraTest":
					rotSpd = 7;
					moveSpd = 4;
					tgtValue = 3;
					trailSaturation = 12;
				break;
			}
		}
		
		public function addWaypoint(wp:Point):void
		{
			waypoints.push(wp);
			resetRoute();
		}
		
		override public function childStep():Boolean
		{
			// -- override this function
			if (!isRunning || !isAlive) return false;
						
			// update waypoint to next if current is cleared or there is no current
			if ((!nextWaypoint || getDistN(x, y, nextWaypoint.x, nextWaypoint.y) < waypointThreshold)
				 && route.length > 0)
				 {
					nextWaypoint = route.shift();
					if (route.length == 0)
					{
						trace("Plane reached safety!");
						safe = true;
						(manager as PlaneManager).checkAllSafe();
						// -- TODO call PlaneManager check if all planes are safe here
					}
					//trace(this + " setting next waypoint to be " + nextWaypoint + ".");
				 }
				
			if (nextWaypoint)
			{
				// update rotation
				var tgtAng:Number = getAngle(x, y, nextWaypoint.x, nextWaypoint.y);
				if (tgtAng > facing + 180) tgtAng -= 360;
				if (tgtAng < facing - 180) tgtAng += 360;
					
				// set to target angle if increment is too large
				if (Math.abs(tgtAng - facing) < rotSpd)
					facing = tgtAng;
				// otherwise turn towards target angle
				else
				{
					var rotInc:Number = (tgtAng - facing) * .2;// / rotSpd;
					// set speed limit
					if (Math.abs(rotInc) > rotSpd)					
						rotInc = (rotInc < 0 ? -1 : 1) * rotSpd;
					facing += rotInc;
				}
				
				forward();
			}
			//if (route.length == 1)
				//resetRoute();

			base.rotation = facing;

			return false;	//isAlive
		}
		
		override public function childRestart():void
		{
			x = origX;
			y = origY;
			base.rotation = facing = origDir;
			resetRoute();
			isAlive = visible = true;
		}		
		
		private function resetRoute():void
		{
			route = new Vector.<Point>();
			for each (var p:Point in waypoints)
				route.push(p);
		}
		
		public function getWaypoints():Vector.<Point>
		{
			return waypoints;
		}
		
		override public function destroy():void
		{
			isAlive = false;
			visible = false;
			clearTrail();
			manager.cg.addExplosion(x, y, 140, .015, 1.5);
			(manager as PlaneManager).planeDestroyed(this);
			manager.cg.eng.soundMan.playSound("SFX_explosionPlane");
		}
	}
}
