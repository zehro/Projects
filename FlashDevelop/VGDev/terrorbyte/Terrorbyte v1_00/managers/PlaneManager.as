package managers
{
	import flash.display.MovieClip;
	import managers.Manager;
	import props.Plane;
	import flash.geom.Point;

	public class PlaneManager extends Manager
	{
		private var planeVec:Vector.<MovieClip> = new Vector.<MovieClip>();
		private var wayGFX:MovieClip;
		public var failed:Boolean;

		public function PlaneManager(_cg:MovieClip, _wayGFX:MovieClip)
		{
			super(_cg);
			wayGFX = _wayGFX;
		}
		
		override public function step(ld:Boolean, rd:Boolean, p:Point):void
		{
			var i:int;
			for (i = planeVec.length - 1; i >= 0; i--)
				if (planeVec[i].step())
					planeVec.splice(i, 1);
				else if (ld)
					planeVec[i].leftMouse();
				else if (rd)
					planeVec[i].rightMouse();
		}
		
		public function setRunning(run:Boolean):void
		{
			for each (var p:Plane in planeVec)
			{
				p.isRunning = run;
				if (!run)
					p.restart();
			}
		}
		
		override public function getVec():Vector.<MovieClip>
		{
			return planeVec;
		}
		
		public function checkAllSafe():void
		{
			trace("Checking if all planes are safe");
			trace("Failed? " + failed);
			if (failed) return;
			var allSafe:Boolean = true;
			for each (var p:Plane in planeVec)
				if (p.vital && !p.safe)
				{
					trace(p.currentLabel + " is vital and not safe!");
					allSafe = false;
					break;
				}
				else
					trace(p.currentLabel + " is not vital or safe!");
			if (allSafe)
				cg.clearScenario();
		}
		
		public function planeDestroyed(p:Plane):void
		{
			if (failed) return;
			if (p.vital)
			{
				cg.failScenario();
				failed = true;
			}
		}
		
		public function addPlane(type:String, locX:int, locY:int, dir:Number, waypoints:Vector.<Point>, vital:Boolean = true):void
		{
			var plane:Plane = new Plane(this, type, locX, locY, dir, vital);
			for each (var wp:Point in waypoints)
				plane.addWaypoint(wp);
			cg.cont.addChild(plane);
			planeVec.push(plane);
			
			drawWaypoint(plane.getWaypoints());
		}
		
		public function drawWaypoint(points:Vector.<Point>):void
		{
			wayGFX.graphics.lineStyle(1, 0x3BB2E7, .25);		// TODO set dynamically
			var i:int, j:int = points.length;
			var p:Point;
			for (i = 0; i < j; i++)
			{
				p = points[i];
				if (i == 0)
					wayGFX.graphics.moveTo(p.x, p.y);
				else
					wayGFX.graphics.lineTo(p.x, p.y);
			}
		}
		
		override public function destroy():void
		{
			planeVec = null;
		}
	}
}
